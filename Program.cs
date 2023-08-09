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
            //Console.WriteLine(node.Name);
            //Assembly assembly = typeof(Program).Assembly;
            //Type type = Type.GetType("FindXMLDifference.Definitions");

            //// create an object with the name of the node
            //var instance = Activator.CreateInstance(type);
            //instance.GetType().GetProperty("name").SetValue(instance, "Ersel");

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

            //List<message> messages = new List<message>();
            //List<element> elements = new List<element>();
            //Console.WriteLine(messages.GetType().GetGenericArguments().First());
            //Console.WriteLine(elements.GetType());
            //element element = new element();
            
            List<string> pathList = new List<string>();
            
            checkDifferences(liveObj, testObj,"definitions");

        }

        private static void checkDifferences(object liveObj, object testObj,string parent)
        {
            // iterate through all properties of the definitions object.
            //get properties of definitions


            PropertyInfo[] properties = liveObj.GetType().GetProperties(); //properties:{message,element,service,portType,...}
            foreach (PropertyInfo property in properties) // property : List<message>/element/service/portType
            {
                //Console.WriteLine(">>"+property);
                if (property.PropertyType.IsPrimitive || property.PropertyType.ToString().Equals("System.String"))
                {
                    if (property.GetValue(liveObj) == null && property.GetValue(testObj) != null)
                    {
                        Console.WriteLine("\nAt ::: "+liveObj.GetType().Name + " ||| For type ::: " + property.Name);
                        Console.WriteLine("Live Xml has NULL value");
                        Console.WriteLine("Test Xml has value > " + property.GetValue(testObj) + "\n");
                    }
                    else if(property.GetValue(testObj) == null && property.GetValue(liveObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + property.Name);
                        Console.WriteLine("Test Xml has NULL value");
                        Console.WriteLine("Live Xml has value > " + property.GetValue(testObj) + "\n");
                    }
                    else if(property.GetValue(testObj) == null && property.GetValue(liveObj) == null)
                    {

                    }
                    else if(!property.GetValue(liveObj).Equals(property.GetValue(testObj)))
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + property.Name);
                        Console.WriteLine("Live Xml has value > " + property.GetValue(liveObj));
                        Console.WriteLine("Test Xml has value > " + property.GetValue(testObj) + "\n\n");
                    }                  
                }
                else if(property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {

                    IList liveList = (IList)property.GetValue(liveObj);
                    IList testList = (IList)property.GetValue(testObj);

                    Console.WriteLine("LIST CHECK HERE > " + property.Name);
                    
                    compareLists(property,liveList,testList,parent);

                    int counter = 0;
                    //foreach(var item in liveList) // item is each index of the list: which is message for definitions
                    //{
                    //    //Console.WriteLine(counter++ + " " + item + " " + elementType);
                    //    //foreach (var propertyInfo in elementProperties)
                    //    //{
                    //    //    Console.WriteLine(counter+" "+propertyInfo.Name);
                    //    //    //get value of of propert of propertyInfo with the name of propertyInfo.Name
                    //    //    Console.WriteLine(counter+" "+propertyInfo.GetValue(item));
                    //    //    counter++;
                    //    //}
                        
                    //}


                }
                else
                {
                    // property is user defined type

                    if(property.GetValue(liveObj) == null && property.GetValue(testObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + property.Name);
                        Console.WriteLine("Live Xml has NULL value");
                        Console.WriteLine("Test Xml has value > " + property.GetValue(testObj) + "\n");
                    }
                    else if (property.GetValue(testObj) == null && property.GetValue(liveObj) != null)
                    {
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + property.Name);
                        Console.WriteLine("Test Xml has NULL value");
                        Console.WriteLine("Live Xml has value > " + property.GetValue(testObj) + "\n");
                    }
                    else
                    {
                        if(property.GetValue(liveObj) != null && property.GetValue(testObj) != null)
                        {
                            checkDifferences(property.GetValue(liveObj), property.GetValue(testObj),property.Name);                          
                        }
                        else
                        {
                            Console.WriteLine("     ===== NULL " + property.Name + " =====");
                        }
                    }

                    
         

                }
            }
        }

        public static void compareLists(PropertyInfo property, IList liveList, IList testList,string parent)
        {
            
            Type elementType = property.PropertyType.GetGenericArguments().First();
            PropertyInfo[] elementProperties = elementType.GetProperties();

            Console.WriteLine("-------------------------------------- 1" + parent);
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
                    //Console.WriteLine("N: " + propertyInfo.Name);
                    //get value of of propert of propertyInfo with the name of propertyInfo.Name
                    //Console.WriteLine("V: " + propertyInfo.GetValue(liveList[i]) + "\n");
                    //passing liveList[i] as element, testList as list of elements, propertyInfo as property of element
                    if (FindInList(liveList[i], testList, propertyInfo))
                    {
                        Console.WriteLine("Found >> " + propertyInfo.GetValue(liveList[i]));
                    }
                    else
                    {
                        Console.WriteLine("Not Found >> " + propertyInfo.GetValue(liveList[i]));
                    }
                    
                }
            }
            
            
            
            

            //foreach (var item in liveList) // item is each index of the list: which is message for definitions
            //{
            //    Console.WriteLine("I: "+item);
            //    foreach (var propertyInfo in elementProperties)
            //    {
            //        Console.WriteLine("N: "+ propertyInfo.Name);
            //        //get value of of propert of propertyInfo with the name of propertyInfo.Name
            //        Console.WriteLine("V: "+propertyInfo.GetValue(item) + "\n");
            //    }
            //}
            Console.WriteLine("-------------------------------------- 2");

            //schema has list of element
            //item is 'element' instance
            //elementType variable is 'element'
            //elementProperties is list of properties of element : complexType, name, minOccur, maxOccur, type, nillable
            //in foreach loop, each propertyInfo is type of field.
            //propertyInfo GetValue(item) is value of that field

            

            
        }

        public static bool FindInList(object? v, IList testList, PropertyInfo propertyInfo)
        {
            //I have two list : liveList and testList. Get each element from liveList and search in testList

            //check is it generic or list
            
            for(int i=0;i<testList.Count;i++)
            {
                if(propertyInfo.GetValue(v) == null && propertyInfo.GetValue(testList[i]) == null)
                {
                    return true;
                }
                else if(propertyInfo.GetValue(v) == null && propertyInfo.GetValue(testList[i]) != null)
                {
                    return false;
                }
                else if (propertyInfo.GetValue(v) != null && propertyInfo.GetValue(testList[i]) == null)
                {
                    return false;
                }
                else if (propertyInfo.GetValue(v).Equals(propertyInfo.GetValue(testList[i])))
                {
                    return true;
                }              
            }
            return false;

            //if it is primitive type, check equality
            //if it is user defined type, call checkDifferences

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