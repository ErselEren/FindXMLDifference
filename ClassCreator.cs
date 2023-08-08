using System.Reflection.Emit;
using System.Reflection;

namespace FindXMLDifference
{
    public class ClassCreator
    {
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        public Dictionary<string, TypeBuilder> typeBuilders;

        public ClassCreator(string assemblyName)
        {
            var assemblyNameObj = new AssemblyName(assemblyName);
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyNameObj, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            typeBuilders = new Dictionary<string, TypeBuilder>();
        }

        public TypeBuilder CreateClass(string className)
        {
            if (typeBuilders.TryGetValue(className, out var existingTypeBuilder))
            {
                return existingTypeBuilder;
            }

            var typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class);
            typeBuilders.Add(className, typeBuilder);
            return typeBuilder;
        }

        public void HandleField(List<string> ClassNames, List<List<FieldItem>> fieldItems)
        {
            //iterate through the typeBuilders dictionary
            //for each class in dictionary, iterate through its fields
            List<TypeBuilder> tempTypesBuilders = new List<TypeBuilder>();
            

            for (int i = 0; i < typeBuilders.Count; i++)
            {
                var tempType = typeBuilders[typeBuilders.Keys.ElementAt(i)];
                tempTypesBuilders.Add(tempType);

            }




        }

        public void DefineFields()
        {
            foreach (var typeBuilder in typeBuilders.Values)
            {
                // Here you can define the fields for each class
                if (typeBuilder.Name == "Element")
                {
                    var complexTypeBuilder = typeBuilders["complexType"];
                    typeBuilder.DefineField("complexType", complexTypeBuilder, FieldAttributes.Public);
                }
                else if (typeBuilder.Name == "complexType")
                {
                    // Define fields for complexType class
                    typeBuilder.DefineField("sequence", typeof(string), FieldAttributes.Public);
                    typeBuilder.DefineField("complexContent", typeof(string), FieldAttributes.Public);
                }
                // Add more conditionals for other classes and their fields as needed.
            }
        }

        public Type CreateType(string className)
        {
            if (!typeBuilders.TryGetValue(className, out var typeBuilder))
            {
                throw new ArgumentException($"Class '{className}' has not been defined.");
            }

            return typeBuilder.CreateType();
        }
    }
}
