using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JH.DataBinding
{
  public enum PathResolveOperation
  {
    GetValue,
    SetValue,
  }

  public static class ReflectionExtensions
  {
    public static PropertyInfo FindProperty(this Type type, string name)
    {
      return Array.Find(type.GetProperties(), (x) => x.Name == name);
    }

    public static ValueType ResolveInstanceMember<ValueType>(this object instance, string path)
    {
      if (instance == null)
      {
        return default;
      }

      var pathFragments = path.Split('.');
      var fragmentInstance = instance;

      foreach (var fragmentPath in pathFragments.Take(pathFragments.Length - 1))
      {
        if (fragmentInstance == null)
        {
          throw new InvalidOperationException(
            string.Format(
              "Binding path {0} does not match dataSource of type {1}",
              path,
              instance.GetType().Name
            )
          );
        }

        var memberInfo = fragmentInstance.GetType().GetMember(fragmentPath, BindingFlags.Instance);

        fragmentInstance = memberInfo.GetValue(0);
      }

      return (ValueType)
        fragmentInstance
          .GetType()
          .GetMember(pathFragments.Last(), BindingFlags.Instance)
          .GetValue(0);
    }

    public static string GetFriendlyName(this Type type)
    {
      if (type == null)
      {
        return "None";
      }

      string friendlyName = TypeAliases.TryGetValue(type, out var alias) ? alias : type.Name;

      if (type.IsGenericType)
      {
        friendlyName = friendlyName.Split('`')[0];

        friendlyName += "<";

        Type[] typeParameters = type.GetGenericArguments();

        for (int i = 0; i < typeParameters.Length; ++i)
        {
          Type typeParam = typeParameters[i];
          string typeParamName = TypeAliases.TryGetValue(typeParam, out alias)
            ? alias
            : typeParam.Name;
          friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
        }

        friendlyName += ">";
      }

      if (type.IsNested)
      {
        return type.DeclaringType.GetFriendlyName() + "." + friendlyName;
      }

      return friendlyName;
    }

    [Obsolete(
      "This is just here to make some old code survive compilation. It's bugged. Do NOT use it."
    )]
    public static PropertyInfo ResolvePublicPropertyPath(this object instance, string path)
    {
      if (instance == null)
      {
        return null;
      }

      var pathFragments = path.Split('.');
      var fragmentInstance = instance;

      foreach (var fragmentPath in pathFragments.Take(pathFragments.Length - 1))
      {
        if (fragmentInstance == null)
        {
          throw new InvalidOperationException(
            string.Format(
              "Binding path {0} does not match dataSource of type {1}",
              path,
              instance.GetType().Name
            )
          );
        }

        var propertyInfo = fragmentInstance
          .GetType()
          .GetProperty(fragmentPath, BindingFlags.Instance | BindingFlags.Public);

        var boundPropertyGetter = propertyInfo.GetGetMethod();
        fragmentInstance = boundPropertyGetter.Invoke(fragmentInstance, null);
      }

      return fragmentInstance
        .GetType()
        .GetProperty(pathFragments.Last(), BindingFlags.Instance | BindingFlags.Public);
    }

    public static IEnumerable<MethodInfo> ResolvePublicPropertyPath(
      this object instance,
      string path,
      PathResolveOperation operation
    )
    {
      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      var pathFragments = path.Split('.').ToArray();
      var fragmentInstanceType = instance.GetType();

      foreach (var fragmentPath in pathFragments.Take(pathFragments.Length - 1))
      {
        var currentPathPropertyInfo = fragmentInstanceType.FindProperty(fragmentPath);
        fragmentInstanceType = currentPathPropertyInfo.PropertyType;

        yield return currentPathPropertyInfo.GetMethod;
      }

      var finalFragment = pathFragments.Last();
      var propertyInfo = fragmentInstanceType.FindProperty(finalFragment);

      if (propertyInfo == null)
      {
        throw new InvalidOperationException($"Failed to resolve property path {path}.");
      }

      switch (operation)
      {
        case PathResolveOperation.GetValue:
          yield return propertyInfo.GetMethod;
          break;
        case PathResolveOperation.SetValue:
          yield return propertyInfo.SetMethod;
          break;
      }
    }

    public static void SyncValue(
      this object source,
      object target,
      MethodInfo[] getAccessors,
      MethodInfo[] setAccessors
    )
    {
      setAccessors.InvokeSetAccessChain(target, getAccessors.InvokeGetAccessChain(source));
    }

    public static object InvokeGetAccessChain(this MethodInfo[] accessors, object instance)
    {
      if (accessors.Length == 0)
      {
        throw new ArgumentException(
          "Accessors array must contain at least one element.",
          nameof(accessors)
        );
      }

      if (instance == null)
      {
        return null;
      }

      var currentInstance = instance;

      foreach (var getter in accessors.Take(accessors.Length - 1))
      {
        currentInstance = getter.Invoke(currentInstance, null);
      }

      return accessors[^1].Invoke(currentInstance, null);
    }

    public static void InvokeSetAccessChain(
      this MethodInfo[] accessors,
      object instance,
      params object[] values
    )
    {
      if (accessors.Length == 0)
      {
        throw new ArgumentException(
          "Accessors array must contain at least one element.",
          nameof(accessors)
        );
      }

      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var currentInstance = instance;

      foreach (var getter in accessors.Take(accessors.Length - 1))
      {
        currentInstance = getter.Invoke(currentInstance, null);
      }

      var setter = accessors[^1];
      setter.Invoke(currentInstance, values);
    }

    public static bool InheritsOrImplements(this Type type, Type baseType)
    {
      baseType = ResolveGenericTypeDefinition(baseType);

      var currentDerived = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

      while (currentDerived != typeof(object))
      {
        if (
          baseType == currentDerived
          || currentDerived.ImplementsGenericInterfaceDefinition(baseType)
        )
        {
          return true;
        }

        currentDerived =
          currentDerived.BaseType != null && currentDerived.BaseType.IsGenericType
            ? currentDerived.BaseType.GetGenericTypeDefinition()
            : currentDerived.BaseType;

        if (currentDerived == null)
        {
          return false;
        }
      }
      return false;
    }

    private static bool ImplementsGenericInterfaceDefinition(this Type type, Type interfaceType)
    {
      foreach (var implementedInterface in type.GetInterfaces())
      {
        var candidate = implementedInterface.IsGenericType
          ? implementedInterface.GetGenericTypeDefinition()
          : implementedInterface;

        if (candidate == interfaceType)
          return true;
      }

      return false;
    }

    private static Type ResolveGenericTypeDefinition(Type type)
    {
      if (type is null)
      {
        throw new ArgumentNullException(nameof(type), "Given type must not be null.");
      }

      return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }

    private static readonly Dictionary<Type, string> TypeAliases = new()
    {
      { typeof(sbyte), "sbyte" },
      { typeof(byte), "byte" },
      { typeof(short), "short" },
      { typeof(ushort), "ushort" },
      { typeof(int), "int" },
      { typeof(uint), "uint" },
      { typeof(long), "long" },
      { typeof(ulong), "ulong" },
      { typeof(float), "float" },
      { typeof(double), "double" },
      { typeof(bool), "bool" },
      { typeof(char), "char" },
      { typeof(string), "string" },
      { typeof(object), "object" },
    };
  }
}
