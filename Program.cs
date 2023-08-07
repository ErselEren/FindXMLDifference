using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Xml.Linq;

namespace FindXMLDifference
{
    public class Program
    { 
      
        public static List<string> ClassNames = new List<string>();
        public static List<List<FieldItem>> fieldItems = new List<List<FieldItem>>();
        public static int counter = 0;


        public static void Main(string[] args)
        {

            //testDynamicClassGenerator();
            //testCreateDynamicClass();
            //DeserializeObject("simple.xml");
            //test1();
            //test2();

            string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\GetLiveSportsLive.xml";
            //string xmlFilePath = @"C:\Users\ersel\source\repos\FindXMLDifference\simple.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            Console.WriteLine("--------------------------");
            
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
                    Console.WriteLine(i+"-"+j+" " + "Class Name : " + ClassNames[i] + " | Field Type : " + fieldItems[i][j].FieldType + "|| Field Name : " + fieldItems[i][j].FieldName);
                }
                Console.WriteLine("========================================>");
            }

            Console.WriteLine("END OF MAIN >> Press any key to exit...");
            Console.ReadKey();
        }


        public static void testDynamicClassGenerator()
        {
          string propname1 = "FirstProperty";
      
          var typeBuilder = DynamicClassCreator.CreateTypeBuilder();
          var fieldBuilder = DynamicClassCreator.CreateFieldBuilder(typeBuilder,propname1,propname1.GetType());
          var propertyBuilder = DynamicClassCreator.DefinePropertyBuilderAndGet(typeBuilder,propname1, propname1.GetType());
      
          var getMethodBuilder = DynamicClassCreator.DefineGetMethodBuilder(typeBuilder,propname1, propname1.GetType());
          var ilGenerator = DynamicClassCreator.HandleILGeneratorforGetter(getMethodBuilder, fieldBuilder);
          var setMethodBuilder = DynamicClassCreator.DefineSetMethodBuilder(typeBuilder,propname1,propname1.GetType());
          DynamicClassCreator.HandleILGeneratorForSetter(ilGenerator, setMethodBuilder, fieldBuilder);

          var fieldBuilder2 = DynamicClassCreator.CreateFieldBuilder(typeBuilder, "SecondProp",typeof(int));
          var propertyBuilder2 = DynamicClassCreator.DefinePropertyBuilderAndGet(typeBuilder, "SecondProp", typeof(int));
          var getMethodBuilder2 = DynamicClassCreator.DefineGetMethodBuilder(typeBuilder, "SecondProp",typeof(int));
          var ilGenerator2 = DynamicClassCreator.HandleILGeneratorforGetter(getMethodBuilder2, fieldBuilder2);
          var setMethodBuilder2 = DynamicClassCreator.DefineSetMethodBuilder(typeBuilder, "SecondProp", typeof(int));
          DynamicClassCreator.HandleILGeneratorForSetter(ilGenerator2, setMethodBuilder2, fieldBuilder2);

          List<MethodBuilder> getterList = new List<MethodBuilder>();
          List<MethodBuilder> setterList = new List<MethodBuilder>();
          List<PropertyBuilder> propertyList = new List<PropertyBuilder>();
          getterList.Add(getMethodBuilder);
          getterList.Add(getMethodBuilder2);

          setterList.Add(setMethodBuilder);
          setterList.Add(setMethodBuilder2);

          propertyList.Add(propertyBuilder);
          propertyList.Add(propertyBuilder2);

          var dynamicType = DynamicClassCreator.MapAndGetDynamicType(propertyList,getterList,setterList,typeBuilder);

          // Create an instance of the dynamic class
          var instance = Activator.CreateInstance(dynamicType);


          // Set and get the property value
          dynamicType.GetProperty(propname1).SetValue(instance, "Hello, " + propname1+"!");
          var result = dynamicType.GetProperty(propname1).GetValue(instance);

          dynamicType.GetProperty("SecondProp").SetValue(instance, 14);
          var result2 = dynamicType.GetProperty("SecondProp").GetValue(instance);

          Console.WriteLine("1>>"+result);
          Console.WriteLine("2>>" + result2);

          Console.WriteLine(dynamicType.GetProperty("SecondProp").GetType());
          Console.WriteLine(dynamicType.GetProperty("SecondProp").PropertyType);
          Console.WriteLine(dynamicType.GetProperty("SecondProp").GetValue(instance).GetType());

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

        //private static void DeserializeObject(string filename)
        //{
        //  Console.WriteLine("Reading with Stream");
        //  // Create an instance of the XmlSerializer.
        //  XmlSerializer serializer =
        //  new XmlSerializer(typeof(OrderedItem));

        //  // Declare an object variable of the type to be deserialized.
        //  OrderedItem i;

        //  using (Stream reader = new FileStream(filename, FileMode.Open))
        //  {
        //    // Call the Deserialize method to restore the object's state.
        //    i = (OrderedItem)serializer.Deserialize(reader);
        //  }

        //  // Write out the properties of the object.
        //  Console.WriteLine(
        //  i.ItemName + "\t" +
        //  i.Description + "\t" +
        //  i.UnitPrice + "\t" +
        //  i.Quantity + "\t" +
        //  i.LineTotal);
        //}

        public static string SerializeObject<T>(T dataObject)
        {
          if (dataObject == null)
          {
            return string.Empty;
          }
          try
          {
            using (StringWriter stringWriter = new System.IO.StringWriter())
            {
              var serializer = new XmlSerializer(typeof(T));
              serializer.Serialize(stringWriter, dataObject);
              return stringWriter.ToString();
            }
          }
          catch (Exception ex)
          {
            return string.Empty;
          }
        }


        public static T DeserializeObject<T>(string xml) where T : new()
        {
              if (string.IsNullOrEmpty(xml))
              {
                return new T();
              }
              try
              {
                using (var stringReader = new StringReader(xml))
                {
                  var serializer = new XmlSerializer(typeof(T));
                  return (T)serializer.Deserialize(stringReader);
                }
              }
              catch (Exception ex)
              {
                return new T();
              }
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