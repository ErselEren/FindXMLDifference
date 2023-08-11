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
using System.Text;
using System.IO;

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
        public static StreamWriter writer = new StreamWriter("output.txt");

        public static void Main(string[] args)
        {
            string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            Console.Write("Input : ");
            string input = Console.ReadLine();

            string content = "ersel";
            System.IO.File.WriteAllText("output.txt", string.Empty);

            //FindDiffWithKeyword(xmlDoc.DocumentElement,input);
            FindDiffWithKeyword(xmlDoc.DocumentElement, "SystemAuthenticateWithExpire");

            Console.WriteLine("END OF MAIN >> Press any key to exit...");
            Console.ReadKey();
        }
        private static void writeToFile(string content)
        {
            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(content);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                File.AppendAllText("output.txt", asciiBytes[i].ToString());
            }
        }

        private static void FindDiffWithKeyword(XmlElement documentElement, string str)
        {
            
            string xmlLivePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            string xmlTestPath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsTest.xml";
            string content;
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

            List<operation> liveOperationList = liveObj.portType.operation;
            List<operation> testOperationList = testObj.portType.operation;

            output liveOutput = null;
            for(int i = 0; i < liveOperationList.Count; i++)
            {
                if (liveOperationList[i].name.Equals(str))
                {
                    liveOutput = liveOperationList[i].output;
                    break;
                }
            }

            output testOutput = null;
            for (int i = 0; i < testOperationList.Count; i++)
            {
                if (testOperationList[i].name.Equals(str))
                {
                    testOutput = testOperationList[i].output;
                    break;
                }
            }

            if(liveOutput == null && testOutput != null)
            {
                content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testOutput.GetType().Name + "\n" + "Live Output is null but Test Output is not null";
                writeToFile(content);
                Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testOutput.GetType().Name);
                Console.WriteLine("Live Output is null but Test Output is not null");
            }
            else if (liveOutput != null && testOutput == null)
            {
                content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveOutput.GetType().Name + "\n" + "Live Output is not null but Test Output is null";
                writeToFile(content);
                Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveOutput.GetType().Name);
                Console.WriteLine("Live Output is not null but Test Output is null");
            }
            else if (liveOutput != null && testOutput != null)
            {
                string liveUrl = liveOutput.Action;
                string testUrl = testOutput.Action;

                // Find the last index of "/"
                int lastIndex1 = liveUrl.LastIndexOf('/');
                int lastIndex2 = testUrl.LastIndexOf('/');

                // Extract the substring after the last "/"
                string lastString1 = liveUrl.Substring(lastIndex1 + 1);
                string lastString2 = testUrl.Substring(lastIndex2 + 1);

                element liveElement = null;
                element testElement = null;
                
                for(int i = 0; i< liveObj.types.schema.element.Count; i++)
                {
                    if (liveObj.types.schema.element[i].name.Equals(lastString1))
                    {
                        liveElement = liveObj.types.schema.element[i];
                        break;
                    }
                }

                for (int i = 0; i < testObj.types.schema.element.Count; i++)
                {
                    if (testObj.types.schema.element[i].name.Equals(lastString2))
                    {
                        testElement = testObj.types.schema.element[i];
                        break;
                    }
                }
                
                FindByTraversing(liveElement, testElement,liveObj.types.schema,testObj.types.schema,lastString1);
            }
            else
            {
                content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveOutput.GetType().Name + "\n" + "Live Output is null and Test Output is null";
                writeToFile(content);
                Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveOutput.GetType().Name);
                Console.WriteLine("Live Output is null and Test Output is null");
            }

        }

        public static void FindByTraversing(object liveObj, object testObj, schema liveSchema, schema testSchema, string parent)
        {
            PropertyInfo[] liveProperties = liveObj.GetType().GetProperties(); //properties:{message,element,service,portType,...}
            PropertyInfo[] testProperties = testObj.GetType().GetProperties();
            string content;
            for (int i = 0; i < liveProperties.Length; i++)
            {
                if (liveProperties[i].Name.Equals("_type") && liveProperties[i].GetValue(liveObj) != null)
                {

                    string input = liveProperties[i].GetValue(liveObj).ToString();
                    string prefix = "tns:";

                    if (input.StartsWith(prefix))
                    {
                        string rightPart = input.Substring(prefix.Length);

                        object liveItem = GetObjectWithName(liveSchema.element,liveSchema.complexType, rightPart);
                        object testItem = GetObjectWithName(testSchema.element,testSchema.complexType, rightPart);

                        if(liveItem == null && testItem != null)
                        {
                            content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testItem.GetType().Name + "\n" + "Live Item is null but Test Item is not null";
                            writeToFile(content);
                            Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testItem.GetType().Name);
                            Console.WriteLine("Live Item is null but Test Item is not null");
                        }
                        else if (liveItem != null && testItem == null)
                        {
                            content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveItem.GetType().Name + "\n" + "Live Item is not null but Test Item is null";
                            writeToFile(content);
                            Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveItem.GetType().Name);
                            Console.WriteLine("Live Item is not null but Test Item is null");
                        }
                        else if (liveItem != null && testItem != null)
                        {
                            FindByTraversing(liveItem, testItem,liveSchema,testSchema,rightPart);
                        }
                    }
                }

                if(liveProperties[i].PropertyType.IsPrimitive || liveProperties[i].PropertyType.ToString().Equals("System.String"))
                {
                    //property is primitive type
                    if (liveProperties[i].GetValue(liveObj) == null && testProperties[i].GetValue(testObj) != null)
                    {
                        content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testProperties[i].Name + "\n" + "Live Xml has NULL value" + "\n" + "Test Xml has value > " + testProperties[i].GetValue(testObj);
                        writeToFile(content);
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + testProperties[i].Name);
                        Console.WriteLine("Live Xml has NULL value");
                        Console.WriteLine("Test Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else if (testProperties[i].GetValue(testObj) == null && liveProperties[i].GetValue(liveObj) != null)
                    {
                        content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name + "\n" + "Test Xml has NULL value" + "\n" + "Live Xml has value > " + liveProperties[i].GetValue(liveObj);
                        writeToFile(content);
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Test Xml has NULL value");
                        Console.WriteLine("Live Xml has value > " + testProperties[i].GetValue(testObj) + "\n");
                    }
                    else if (testProperties[i].GetValue(testObj) == null && testProperties[i].GetValue(liveObj) == null)
                    {

                    }
                    else if (!liveProperties[i].GetValue(liveObj).Equals(testProperties[i].GetValue(testObj)))
                    {
                        content = "At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name + "\n" + "Live Xml has value > " + liveProperties[i].GetValue(liveObj) + "\n" + "Test Xml has value > " + testProperties[i].GetValue(testObj);
                        writeToFile(content);
                        Console.WriteLine("\nAt ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveProperties[i].Name);
                        Console.WriteLine("Live Xml has value > " + liveProperties[i].GetValue(liveObj));
                        Console.WriteLine("Test Xml has value > " + testProperties[i].GetValue(testObj) + "\n\n");
                    }
                }
                else if (liveProperties[i].PropertyType.IsGenericType && liveProperties[i].PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    List<element> liveList = (List<element>)liveProperties[i].GetValue(liveObj);
                    List<element> testList = (List<element>)testProperties[i].GetValue(testObj);
                    compareElementList(liveList, testList, liveSchema, testSchema,parent);
                }
                else
                {
                    if(liveProperties[i].GetValue(liveObj) != null && testProperties[i].GetValue(testObj) != null)
                    {          
                         FindByTraversing(liveProperties[i].GetValue(liveObj), testProperties[i].GetValue(testObj), liveSchema, testSchema,parent+" ");                     
                    }
                        
                }
            }
        }

        private static void compareElementList(List<element>? liveList, List<element>? testList, schema liveSchema, schema testSchema, string path)
        {
            List<int> liveIndexes = new List<int>();
            List<int> testIndexes = new List<int>();
            string content;
            bool flag = false;
            for(int i=0; i<liveList.Count; i++)
            {
                flag = false;
                for(int j = 0; j < testList.Count; j++)
                {
                    if (liveList[i].Equals(testList[j]))
                    {
                        flag = true;
                        liveIndexes.Add(i);
                        if (liveList[i]._type != null)
                            checkTNS(liveList[i]._type, liveSchema, testSchema);
                      
                        break;
                    }                                                     
                }

                if (!flag)
                {
                    content = "At > " + path + "\n" + "Live XML has > name = |" + liveList[i]._type + "| type = |" + liveList[i].name + "|";
                    writeToFile(content);
                    Console.WriteLine("At > " + path);
                    Console.WriteLine("Live XML has > name = |" + liveList[i]._type + "| type = |" + liveList[i].name + "|");
                }
            }


            for (int i = 0; i < testList.Count; i++)
            {
                flag = false;
                for (int j = 0; j < liveList.Count; j++)
                {
                    if (testList[i].Equals(liveList[j]))
                    {
                        flag = true;
                        testIndexes.Add(i);
                        break;
                    }           
                }

                if (!flag)
                {
                    content = "At > " + path + "\n" + "Test XML has > name = |" + testList[i]._type + "| type = |" + testList[i].name + "|";
                    writeToFile(content);
                    Console.WriteLine("At > " + path);
                    Console.WriteLine("Test XML has > name = |" + testList[i]._type + "| type = |" + testList[i].name + "|");
                }

            }

           
        }

        private static void checkTNS(string _type, schema testSchema, schema liveSchema)
        {
         
            string prefix = "tns:";
            string content;
            if (_type.StartsWith(prefix))
            {
                string rightPart = _type.Substring(prefix.Length);

                object liveItem = GetObjectWithName(liveSchema.element,liveSchema.complexType, rightPart);
                object testItem = GetObjectWithName(testSchema.element, testSchema.complexType, rightPart);

                if(liveItem == null && testItem != null)
                {
                    //Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + testItem.GetType().Name);
                    content = "At ::: " + liveItem.GetType().Name + " ||| For type ::: " + testItem.GetType().Name + "\n" + "Live Item is null but Test Item is not null";
                    writeToFile(content);
                    Console.WriteLine("Live Item is null but Test Item is not null");
                }
                else if (liveItem != null && testItem == null)
                {
                    //Console.WriteLine("At ::: " + liveObj.GetType().Name + " ||| For type ::: " + liveItem.GetType().Name);
                    content = "At ::: " + liveItem.GetType().Name + " ||| For type ::: " + liveItem.GetType().Name + "\n" + "Live Item is not null but Test Item is null";
                    writeToFile(content);
                    Console.WriteLine("Live Item is not null but Test Item is null");
                }
                else if (liveItem != null && testItem != null)
                {
                    FindByTraversing(liveItem, testItem,liveSchema,testSchema,rightPart+" ");
                }
            }
        }

        private static object GetObjectWithName(List<element> elementList, List<complexType> complexList, string rightPart)
        {
            for(int i = 0; i < elementList.Count; i++)
            {
                if (elementList[i].name.Equals(rightPart))
                {
                    return elementList[i];
                }
            }

            for (int i = 0; i < complexList.Count; i++)
            {
                if (complexList[i].name.Equals(rightPart))
                {
                    return complexList[i];
                }
            }

            return null;
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

                    bool flag = CompareLists(liveList, testList);
                    if (!flag)
                    {
                        Console.WriteLine("LIST NOT MATCH");
                    }                  
                }
                else
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
                    else if(liveProperties[i].GetValue(liveObj) != null && testProperties[i].GetValue(testObj) != null)
                    {  
                        checkDifferences(liveProperties[i].GetValue(liveObj), testProperties[i].GetValue(testObj), liveProperties[i].Name);
                    }
                }
            }
        }

        private static bool CompareLists(IList liveList, IList testList)
        {
            Type listType = liveList.GetType().GetGenericArguments().First();
            PropertyInfo[] listProperties = listType.GetProperties();
            //Console.WriteLine("Comparing list of : " + listType);

            object liveItem;
            object testItem;
            bool flag = true;
            for (int i = 0; i < liveList.Count; i++)
            {
                liveItem = liveList[i];
          
                foreach (PropertyInfo property in listProperties)
                {   
                    if (FindInOtherList(liveItem, testList, property) == false)
                    {
                        Console.WriteLine("Difference found in " + property.GetValue(liveItem));
                        return false;
                    }
                    
                }
               
            }
            return flag;
        }

        private static bool FindInOtherList(object? liveItem, IList testList, PropertyInfo property)
        {
            object testItem;
            for(int i=0;i< testList.Count; i++)
            {
                testItem = testList[i];
                if (property.PropertyType.IsPrimitive || property.PropertyType.ToString().Equals("System.String"))
                {
                    if (property.GetValue(liveItem) == null && property.GetValue(testItem) == null)
                    {
                        return true;
                    }
                    else if (property.GetValue(liveItem).Equals(property.GetValue(testItem)))
                    {
                        return true;
                    }
                }
                else if(func(liveItem, testItem, property.Name)==true)
                    return true;
            }

            return false;
        }

        private static bool func(object? liveObj, object? testObj, string parent)
        {
            PropertyInfo[] liveProperties = liveObj.GetType().GetProperties();
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
                else if (liveProperties[i].PropertyType.IsGenericType && liveProperties[i].PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //property is List
                    IList liveList = (IList)liveProperties[i].GetValue(liveObj);
                    IList testList = (IList)testProperties[i].GetValue(testObj);
                    //Console.WriteLine("here 1");
                    bool temp = CompareLists(liveList, testList);
                    return temp;
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

    }
}