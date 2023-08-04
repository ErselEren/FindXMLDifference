using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;

public static class DynamicClassCreator
{

  public static Type MapAndGetDynamicType(List<PropertyBuilder> propertyBuilderList, List<MethodBuilder> getMethodBuilderList, List<MethodBuilder> setMethodBuilderList, TypeBuilder typeBuilder)
  {
    // Map the get and set methods to the property
    for (int i = 0; i < propertyBuilderList.Count; i++)
    {
      propertyBuilderList[i].SetGetMethod(getMethodBuilderList[i]);
      propertyBuilderList[i].SetSetMethod(setMethodBuilderList[i]);
    }

    // Create the type
    var dynamicType = typeBuilder.CreateType();

    return dynamicType;
  }

  public static void HandleILGeneratorForSetter(ILGenerator ilGenerator, MethodBuilder setMethodBuilder, FieldBuilder fieldBuilder)
  {
    ilGenerator = setMethodBuilder.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_0);
    ilGenerator.Emit(OpCodes.Ldarg_1);
    ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
    ilGenerator.Emit(OpCodes.Ret);
  }

  public static MethodBuilder DefineSetMethodBuilder(TypeBuilder typeBuilder, string propname,Type type)
  {
    string mainString = "set_";
    StringBuilder sb = new StringBuilder();
    sb.Append(mainString);
    sb.Append(propname);
    string resultString = sb.ToString();

    var setMethodBuilder = typeBuilder.DefineMethod(
        "set_DynamicProperty",
        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
        null,
        new[] { type });

    return setMethodBuilder;
  }

  public static ILGenerator HandleILGeneratorforGetter(MethodBuilder getMethodBuilder, FieldBuilder fieldBuilder)
  {
      var ilGenerator = getMethodBuilder.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
      ilGenerator.Emit(OpCodes.Ret);

      return ilGenerator;
  }

  public static MethodBuilder DefineGetMethodBuilder(TypeBuilder typeBuilder, string propname, Type type)
  {
    //add "get_" to propname
    string mainString = "get_";
    StringBuilder sb = new StringBuilder();
    sb.Append(mainString);
    sb.Append(propname);
    string resultString = sb.ToString();

    var getMethodBuilder = typeBuilder.DefineMethod(
        resultString,
        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
        type,
        Type.EmptyTypes);

    return getMethodBuilder;
  }

  public static PropertyBuilder DefinePropertyBuilderAndGet(TypeBuilder typeBuilder,string propname,Type type)
  {
      var propertyBuilder = typeBuilder.DefineProperty(
        propname,
        PropertyAttributes.HasDefault,
        type,
        null);

    return propertyBuilder;

  }

  public static TypeBuilder CreateTypeBuilder()
  {
    // Create an assembly name
    var assemblyName = new AssemblyName("DynamicAssembly");

    // Create a dynamic assembly
    var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

    // Create a dynamic module in the assembly
    var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

    var typeBuilder = moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public);

    return typeBuilder;
  }

  public static FieldBuilder CreateFieldBuilder(TypeBuilder typeBuilder,string propname, Type type)
  {
    var fieldBuilder = typeBuilder.DefineField(propname, type, FieldAttributes.Private);
    return fieldBuilder;
  }

}
