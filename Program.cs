using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using System.Xml.Linq;
using System.CodeDom.Compiler;
using System.Xml.Serialization;
using System.CodeDom;
using System.Fabric.Description;
using System.Collections;

namespace FindXMLDifference
{
    public class Program
    { 
        public static List<string> ClassNames = new List<string>();
        public static List<List<FieldItem>> fieldItems = new List<List<FieldItem>>();
        public static int counter = 0;
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;
        private static Dictionary<string, TypeBuilder> typeBuilders;


        public static void Main(string[] args)
        {
            string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            //string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\simple.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            FindDifferenceWithoutDynamic(xmlDoc.DocumentElement);
            

            Console.WriteLine("END OF MAIN >> Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void FindWithDynamic()
        {
            string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            //string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\simple.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            TraverseXmlNode(xmlDoc.DocumentElement);

            //print all classNames
            for (int i = 0; i < ClassNames.Count; i++)
            {
                Console.WriteLine("CLASSES : " + ClassNames[i]);
            }

            //print all fieldItems
            for (int i = 0; i < fieldItems.Count; i++)
            {
                for (int j = 0; j < fieldItems[i].Count; j++)
                {
                    Console.WriteLine(i + "-" + j + " " + "Class Name : " + ClassNames[i] + " | Field Type : " + fieldItems[i][j].FieldType + "|| Field Name : " + fieldItems[i][j].FieldName);
                }
                Console.WriteLine("========================================>");
            }


            handleCreation();

        }

        public static void handleCreation()
        {
            var creator = new ClassCreator("XmlAssembly");
            List<Type> types = new List<Type>();
            for (int i = 0; i < ClassNames.Count; i++)
                creator.CreateClass(ClassNames[i]);

            creator.HandleField(ClassNames, fieldItems);

            for (int i = 0; i < ClassNames.Count; i++)
                types.Add(creator.CreateType(ClassNames[i]));
            

            //print typeBuilders in ClassCreator
            foreach (var typeBuilder in creator.typeBuilders.Values)
            {
                Console.WriteLine("Type Builder : " + typeBuilder.Name);
            }

        }

        public static void FindDifferenceWithoutDynamic(XmlNode node)
        {
            string xmlLivePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            string xmlTestPath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsTest.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(definitions));
            definitions liveObj;
            definitions testObj;
            using (StreamReader reader = new StreamReader(xmlLivePath))
            {
                liveObj = (definitions)serializer.Deserialize(reader);
            }

            using (StreamReader reader = new StreamReader(xmlTestPath))
            {
                testObj = (definitions)serializer.Deserialize(reader);
            }
            
            Console.WriteLine("----------------------------------------------------");
            
            List<string> pathList = new List<string>();
            
            checkDifferences(liveObj, testObj,"definitions");

        }

        private static void checkDifferences(object liveObj, object testObj,string parent)
        {
            PropertyInfo[] liveProperties = liveObj.GetType().GetProperties(); //properties:{message,element,service,portType,...}
            PropertyInfo[] testProperties = testObj.GetType().GetProperties();
            
            for(int i = 0; i < liveProperties.Length; i++)
            {
                if (liveProperties[i].PropertyType.IsPrimitive || liveProperties[i].PropertyType.ToString().Equals("System.String"))
                {
                    //property is primitive type
                    if (liveProperties[i].GetValue(liveObj) == null && testProperties[i].GetValue(testObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + testProperties[i].Name);
                        Console.WriteLine("Live Xml has NULL value");
                        Console.WriteLine("Test Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else if (testProperties[i].GetValue(testObj) == null && liveProperties[i].GetValue(liveObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Test Xml has NULL value");
                        Console.WriteLine("Live Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else if (testProperties[i].GetValue(testObj) == null && testProperties[i].GetValue(liveObj) == null)
                    {

                    }
                    else if (!liveProperties[i].GetValue(liveObj).Equals(testProperties[i].GetValue(testObj)))
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Live Xml has value > " + liveProperties[i].GetValue(liveObj));
                        Console.WriteLine("Test Xml has value > " + testProperties[i].GetValue(testObj) + "\n\n");
                    }
                }
                else if (liveProperties[i].PropertyType.IsGenericType && liveProperties[i].PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //property is List
                    IList liveList = (IList)liveProperties[i].GetValue(liveObj);
                    IList testList = (IList)testProperties[i].GetValue(testObj);

              

                    CompareLists(liveList, testList);


                    //compareLists(liveProperties[i].GetValue(liveObj), liveProperties[i].GetValue(liveObj), liveProperties[i],liveList, testList, parent);
                }
                else // property is user defined type
                {
                    if (liveProperties[i].GetValue(liveObj) == null && testProperties[i].GetValue(testObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Live Xml has NULL value");
                        Console.WriteLine("Test Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else if (testProperties[i].GetValue(testObj) == null && liveProperties[i].GetValue(liveObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Test Xml has NULL value");
                        Console.WriteLine("Live Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else
                    {
                        if (liveProperties[i].GetValue(liveObj) != null && testProperties[i].GetValue(testObj) != null)
                        {
                            checkDifferences(liveProperties[i].GetValue(liveObj), testProperties[i].GetValue(testObj), liveProperties[i].Name);
                        }
                        else
                        {
                            //Console.WriteLine("     ===== NULL " + liveProperties[i].Name + " =====");
                        }
                    }
                }
            }

            Console.WriteLine("______________________________");
        }

        private static void CompareLists(IList liveList, IList testList)
        {
            Type listType = liveList.GetType().GetGenericArguments().First();
            PropertyInfo[] listProperties = listType.GetProperties();
            object liveItem;
            object testItem;

            for (int i = 0; i < liveList.Count; i++)
            {
                foreach (PropertyInfo property in listProperties)
                {

                    if (property.PropertyType.IsPrimitive || property.PropertyType.ToString().Equals("System.String"))
                    {
                        //property is primitive type
                        //Console.WriteLine("Primitive type");
                    }
                    else
                    {
                        liveItem = liveList[i];
                        bool flag = true;
                        for (int j = 0; j < testList.Count; j++)
                        {
                            testItem = testList[j];
                            flag = func(liveItem,testItem,listType.Name); //returns false if both objects are not equal
                            if (flag == true)
                            {
                                  break;
                            }
                        }

                        if (flag == false)
                        {
                            Console.WriteLine("There is difference in : " + property.GetValue(liveItem));
                        }
                        
                    }
                }
            }
        }

        private static bool func(object? liveObj, object? testObj, string parent)
        {
            PropertyInfo[] liveProperties = liveObj.GetType().GetProperties(); //properties:{message,element,service,portType,...}
            PropertyInfo[] testProperties = testObj.GetType().GetProperties();
            bool flag = true;
            

            for (int i = 0; i < liveProperties.Length; i++)
            {
                if (liveProperties[i].PropertyType.IsPrimitive || liveProperties[i].PropertyType.ToString().Equals("System.String"))
                {
                    if (liveProperties[i].GetValue(liveObj) == null && testProperties[i].GetValue(testObj) != null)
                        flag = false;
                        
                    else if (testProperties[i].GetValue(testObj) == null && liveProperties[i].GetValue(liveObj) != null)
                        flag = false;
                    else if (testProperties[i].GetValue(testObj) == null && testProperties[i].GetValue(liveObj) == null)
                        flag = true;
                    else if (!liveProperties[i].GetValue(liveObj).Equals(testProperties[i].GetValue(testObj)))
                        flag = false;
                }             
                else // property is user defined type
                {
                    if (liveProperties[i].GetValue(liveObj) == null && testProperties[i].GetValue(testObj) != null)
                        flag = false;
                    else if (testProperties[i].GetValue(testObj) == null && liveProperties[i].GetValue(liveObj) != null)
                        flag = false;
                    else if (liveProperties[i].GetValue(liveObj) != null && testProperties[i].GetValue(testObj) != null)
                        flag = func(liveProperties[i].GetValue(liveObj), testProperties[i].GetValue(testObj), liveProperties[i].Name);                
                }
            }
            return flag;
        }

        public static void compareLists(object liveObj,object testObj,PropertyInfo property, IList liveList, IList testList,string parent)
        {
            //liveObj and testObj is list of complexType
            //elementType is the type of the list items. Ex: List<element> -> elementType = element
            Type elementType = property.PropertyType.GetGenericArguments().First();
            PropertyInfo[] elementProperties = elementType.GetProperties();

            Console.WriteLine("-------------------------------------- 1" + parent + " " + elementType.Name+" "+liveObj.GetType()+" "+liveList.GetType());
            //print values of properties
            
            if(liveList.Count > testList.Count)
            {
                Console.WriteLine("Live Xml has more elements than Test Xml");
                //find missing elements
            }
            else if(liveList.Count < testList.Count)
            {
                Console.WriteLine("Test Xml has more elements than Live Xml");
                //find missing elements
            }


            for(int i=0;i<liveList.Count;i++)
            {
 
                Console.WriteLine("I: " + liveList[i]);
                foreach (var propertyInfo in elementProperties)
                {
                    Console.WriteLine("N: " + propertyInfo.Name);
                    Console.WriteLine("V: " + propertyInfo.GetValue(liveList[i]) + "\n");
                    //passing liveList[i] as element, testList as list of elements, propertyInfo as property of element
                    if(propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        //compared property is list
                        //call compareList again
                        IList liveList2 = (IList)propertyInfo.GetValue(property);
                        IList testList2 = (IList)propertyInfo.GetValue(property);
                        //compareLists(property, liveList2, testList2, propertyInfo.Name);
                    }
                    else if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.ToString().Equals("System.String"))
                    {
                        //comparing property is primitive
                        //compare values
                    }
                    else
                    {
                        //property is user defined type
                        //call checkDifferences again
                        //checkDifferences(propertyInfo.GetValue(liveList[i]), propertyInfo.GetValue(testList[i]), propertyInfo.Name);
                        //Console.WriteLine("Not Found >> " + propertyInfo.GetValue(liveList[i]));                  
                    }
                    
                }
            }
            
        }

        public static bool FindInList(object? v, IList testList, PropertyInfo propertyInfo)
        {
            //I have two list : liveList and testList. Get each element from liveList and search in testList

            //check is it generic or list
            
            for(int i=0;i<testList.Count;i++)
            {
                if(propertyInfo.GetValue(v) == null && propertyInfo.GetValue(testList[i]) == null)
                {
                    //Console.WriteLine("IF 1");
                    return true;
                }
                else if(propertyInfo.GetValue(v) == null && propertyInfo.GetValue(testList[i]) != null)
                {
                    //Console.WriteLine("IF 2");
                    return false;
                }
                else if (propertyInfo.GetValue(v) != null && propertyInfo.GetValue(testList[i]) == null)
                {
                    //Console.WriteLine("IF 3");
                    return false;
                }
                else if (propertyInfo.GetValue(v).Equals(propertyInfo.GetValue(testList[i])))
                {
                    //Console.WriteLine("IF 4 : " + propertyInfo.GetValue(v) + " | " + propertyInfo.GetValue(testList[i]));
                    return true;
                }              
            }
            //Console.WriteLine("IF 5 : " + propertyInfo.GetValue(v));
            return false;


        }

        public static void TraverseXmlNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                //Console.WriteLine($"Element: {node.Name}");

                string nodeName = splitString(node.Name);
                string attrName;
                string childNodeName;

                if (!checkStringExistInList(nodeName, ClassNames))
                {
                    ClassNames.Add(nodeName);
                    fieldItems.Add(new List<FieldItem>());
                }

                int indexOfCurrentClass = FindIndexFromClassNames(nodeName); //Get Index of current name of Clas Name

                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        attrName = splitString(attribute.Name);
                        
                        if (indexOfCurrentClass != -1)
                        {
                            int fieldIndex = checkFieldInList(attrName, fieldItems[indexOfCurrentClass]);

                            if (fieldIndex == -1)
                            {
                                fieldItems[indexOfCurrentClass].Add(new FieldItem(attrName, "string"));
                            }
                        }
                    }
                }

                if (node.HasChildNodes && indexOfCurrentClass != -1)
                {
                    foreach( XmlNode childNode in node.ChildNodes)
                    {
                        childNodeName = splitString(childNode.Name);
                        int fieldIndex2 = checkFieldInList(childNodeName, fieldItems[indexOfCurrentClass]);
                        
                        if (fieldIndex2 == -1) // This is not added before
                        {
                            fieldItems[indexOfCurrentClass].Add(new FieldItem(childNodeName, childNodeName));
                        }
                        else if (fieldItems[indexOfCurrentClass][fieldIndex2].FieldType == "List:" + childNodeName)
                        {
                            //Do nothing, field is indicated as "List" already
                        }
                        else if(checkDuplicate(node, childNodeName)) //check field has dup. If has, change its type as "List"
                        {
                            fieldItems[indexOfCurrentClass][fieldIndex2].FieldType = "List:" + childNodeName;
                        }

                        TraverseXmlNode(childNode);
                    } 
                }
            }
            //else if (node.NodeType == XmlNodeType.Text)
            //{
            //    Console.WriteLine($"Text: {node.Value}");
            //}
            //else if (node.NodeType == XmlNodeType.Comment)
            //{
            //    Console.WriteLine($"Comment: {node.Value}");
            //}

        }
        
        public static bool checkDuplicate(XmlNode root, string str)
        {
            List<string> childs = new List<string>();
            foreach (XmlNode childNode in root.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    childs.Add(splitString(childNode.Name));
                }
            }

            var duplicates = childs.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            foreach(String childNode in duplicates)
            {
                if (str.Equals(childNode))
                {
                    return true;
                }
            }

            return false;

        }

        private static string splitString(string str)
        {
            string[] substrings = str.Split(':');
            
            if (substrings.Length > 1)
                return substrings[1];
            else
                return substrings[0];
        }

        private static int checkFieldInList(string name, List<FieldItem> fieldItems)
        {
            for(int i = 0; i < fieldItems.Count; i++)
            {
                if (fieldItems[i].FieldName == name)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int FindIndexFromClassNames(string name)
        {
            for(int i = 0; i < ClassNames.Count; i++)
            {
                if (ClassNames[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }
   
        public static bool checkStringExistInList(string str, List<string> list)
        {
            foreach (string item in list)
            {
                if (item.Equals(str))
                {
                    return true;
                }
            }
            return false;
        }

        //public static void testDynamicClassGenerator()
        //{
        //    string propname1 = "FirstProperty";

        //    var typeBuilder = DynamicClassCreator.CreateTypeBuilder("MyClass");
        //    var fieldBuilder = DynamicClassCreator.CreateFieldBuilder(typeBuilder, propname1, propname1.GetType());
        //    var propertyBuilder = DynamicClassCreator.DefinePropertyBuilderAndGet(typeBuilder, propname1, propname1.GetType());

        //    var getMethodBuilder = DynamicClassCreator.DefineGetMethodBuilder(typeBuilder, propname1, propname1.GetType());
        //    var ilGenerator = DynamicClassCreator.HandleILGeneratorforGetter(getMethodBuilder, fieldBuilder);
        //    var setMethodBuilder = DynamicClassCreator.DefineSetMethodBuilder(typeBuilder, propname1, propname1.GetType());
        //    DynamicClassCreator.HandleILGeneratorForSetter(ilGenerator, setMethodBuilder, fieldBuilder);

        //    var fieldBuilder2 = DynamicClassCreator.CreateFieldBuilder(typeBuilder, "SecondProp", typeof(int));
        //    var propertyBuilder2 = DynamicClassCreator.DefinePropertyBuilderAndGet(typeBuilder, "SecondProp", typeof(int));
        //    var getMethodBuilder2 = DynamicClassCreator.DefineGetMethodBuilder(typeBuilder, "SecondProp", typeof(int));
        //    var ilGenerator2 = DynamicClassCreator.HandleILGeneratorforGetter(getMethodBuilder2, fieldBuilder2);
        //    var setMethodBuilder2 = DynamicClassCreator.DefineSetMethodBuilder(typeBuilder, "SecondProp", typeof(int));
        //    DynamicClassCreator.HandleILGeneratorForSetter(ilGenerator2, setMethodBuilder2, fieldBuilder2);

        //    List<MethodBuilder> getterList = new List<MethodBuilder>();
        //    List<MethodBuilder> setterList = new List<MethodBuilder>();
        //    List<PropertyBuilder> propertyList = new List<PropertyBuilder>();
        //    getterList.Add(getMethodBuilder);
        //    getterList.Add(getMethodBuilder2);

        //    setterList.Add(setMethodBuilder);
        //    setterList.Add(setMethodBuilder2);

        //    propertyList.Add(propertyBuilder);
        //    propertyList.Add(propertyBuilder2);

        //    var dynamicType = DynamicClassCreator.MapAndGetDynamicType(propertyList, getterList, setterList, typeBuilder);

        //    // Create an instance of the dynamic class
        //    var instance = Activator.CreateInstance(dynamicType);


        //    // Set and get the property value
        //    dynamicType.GetProperty(propname1).SetValue(instance, "Hello, " + propname1 + "!");
        //    var result = dynamicType.GetProperty(propname1).GetValue(instance);

        //    dynamicType.GetProperty("SecondProp").SetValue(instance, 14);
        //    var result2 = dynamicType.GetProperty("SecondProp").GetValue(instance);

        //    Console.WriteLine("1>>" + result);
        //    Console.WriteLine("2>>" + result2);

        //    Console.WriteLine(dynamicType.GetProperty("SecondProp").GetType());
        //    Console.WriteLine(dynamicType.GetProperty("SecondProp").PropertyType);
        //    Console.WriteLine(dynamicType.GetProperty("SecondProp").GetValue(instance).GetType());

        //}

        //public static void handleDynamic()
        //{
        //    var creator = new ClassCreator("DynamicAssembly");

        //    // Create the classes without fields first
        //    creator.CreateClass("Element");
        //    creator.CreateClass("complexType");

        //    // Define the fields in the second pass
        //    creator.DefineFields();

        //    // Now, you can create types and use them
        //    Type elementType = creator.CreateType("Element");
        //    Type complexType = creator.CreateType("complexType");

        //    // Use the types as needed

        //    // Create an instance of "complexType"
        //    var complexInstance = Activator.CreateInstance(complexType);
        //    // Set the "sequence" field of "complexType" to "SomeSequence"
        //    complexType.GetField("sequence").SetValue(complexInstance, "SomeSequence");
        //    // Set the "complexContent" field of "complexType" to "SomeComplexContent"
        //    complexType.GetField("complexContent").SetValue(complexInstance, "SomeComplexContent");

        //    // Create an instance of "Element"
        //    var elementInstance = Activator.CreateInstance(elementType);
        //    // Set the "complexType" field of "Element" to the instance of "complexType" created above
        //    elementType.GetField("complexType").SetValue(elementInstance, complexInstance);

        //    // Access and print the values of the fields in "Element"
        //    var elementComplexType = elementType.GetField("complexType").GetValue(elementInstance);
        //    var sequenceValue = complexType.GetField("sequence").GetValue(elementComplexType);
        //    var complexContentValue = complexType.GetField("complexContent").GetValue(elementComplexType);

        //    Console.WriteLine($"Element.complexType.sequence: {sequenceValue}");
        //    Console.WriteLine($"Element.complexType.complexContent: {complexContentValue}");
        //}
    }
}