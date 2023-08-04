using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;

namespace FindXMLDifference
{
  public class Program
  {
    public static void Main(string[] args)
    {

      //testDynamicClassGenerator();
      //testCreateDynamicClass();
      //DeserializeObject("simple.xml");
      //test1();
      //test2();
      //test3();
      test4();
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

   

    //public static void test1()
    //{
    //  bool isWellFormed = false;
    //  string xml =@"
    //    <msg>
    //       <id>1</id>
    //       <action>stop</action>
    //    </msg>
    //    ";
    //  var xmlDocument = new XmlDocument();
    //  xmlDocument.LoadXml(xml);
    //  if (isWellFormed)
    //  {
    //    xmlDocument.RemoveChild(xmlDocument.FirstChild);
      
    //  }

    //  var serializedXmlNode = JsonConvert.SerializeXmlNode(
    //              xmlDocument,
    //              Newtonsoft.Json.Formatting.Indented,
    //              true
    //              );
    //  var theDesiredObject = JsonConvert.DeserializeObject<TheModel>(serializedXmlNode);

    //  //print theDesiredObject
    //  System.Console.WriteLine("theDesiredObject.Id: " + theDesiredObject.Id);
    //  System.Console.WriteLine("theDesiredObject.Action: " + theDesiredObject.Action);

    //}

    public static void test2()
    {
      // Create a new ExpandoObject
      dynamic expando = new ExpandoObject();

      // Add properties dynamically
      expando.FirstName = "John";
      expando.LastName = "Doe";
      expando.Age = 30;

      // Access the properties
      string firstName = expando.FirstName;
      string lastName = expando.LastName;
      int age = expando.Age;

      // Check if a property exists before accessing it
      if (expando.ContainsKey("FirstName"))
      {
        string firstNameAgain = expando.FirstName;
        Console.WriteLine($"First Name: {firstNameAgain}");
      }

      // Remove a property dynamically
      if (expando.ContainsKey("Age"))
      {
        expando.Age = null; // Removing the 'Age' property
      }

      // Try to access the 'Age' property after removal
      // This will throw a RuntimeBinderException as 'Age' property does not exist anymore
      try
      {
        int ageAfterRemoval = expando.Age;
      }
      catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
      }

      // Adding a new property after removal
      expando.Email = "john.doe@example.com";

      // Accessing the new property
      string email = expando.Email;

      Console.WriteLine($"First Name: {firstName}");
      Console.WriteLine($"Last Name: {lastName}");
      Console.WriteLine($"Email: {email}");

      // Output:
      // First Name: John
      // Last Name: Doe
      // Email: john.doe@example.com
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
  
    public static void test3()
    {
      dynamic myDynamicObject = new ExpandoObject();

      myDynamicObject.Name = "John";
      myDynamicObject.Age = 30;
      myDynamicObject.IsStudent = true;

      // Access and modify properties
      Console.WriteLine($"Name: {myDynamicObject.Name}, Age: {myDynamicObject.Age}, IsStudent: {myDynamicObject.IsStudent}");

      myDynamicObject.Age = 31; // Changing the value of the Age property

      Console.WriteLine($"Updated Age: {myDynamicObject.Age}");
    }
    
    public static void test4()
    {
      // Create a new ExpandoObject
      dynamic expando = new ExpandoObject();

      // Add properties dynamically
      expando.FirstName = "John";
      expando.LastName = "Doe";
      expando.Age = 30;

      // Add a List<int> property dynamically
      List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
      expando.Numbers = numbers;

      // Access the properties
      string firstName = expando.FirstName;
      string lastName = expando.LastName;
      int age = expando.Age;

      // Access the List<int> property
      List<int> numbersList = expando.Numbers;

      // Check if a property exists before accessing it
      IDictionary<string, object> expandoDict = expando as IDictionary<string, object>;
      if (expandoDict.ContainsKey("FirstName"))
      {
        Console.WriteLine("FirstName exists");
        string firstNameAgain = expando.FirstName;
        Console.WriteLine($"First Name: {firstNameAgain}");
      }

      // Output the List<int> property
      Console.WriteLine("Numbers List:");
      foreach (int num in numbersList)
      {
        Console.WriteLine(num);
      }

      Console.WriteLine($"First Name: {firstName}");
      Console.WriteLine($"Last Name: {lastName}");
      Console.WriteLine($"Age: {age}");
    }

  }

}