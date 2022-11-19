using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace de.JochenHeckl.Unity.DataBinding
{
    public enum PathResolveOperation
    {
        GetValue,
        SetValue
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

                var memberInfo = fragmentInstance
                    .GetType()
                    .GetMember(fragmentPath, BindingFlags.Instance);

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

            string friendlyName = type.Name;

            if (type.IsGenericType)
            {
                friendlyName = friendlyName.Split('`').First();

                friendlyName += "<";

                Type[] typeParameters = type.GetGenericArguments();

                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
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

            foreach (var fragmentPath in pathFragments.Take(pathFragments.Count() - 1))
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
            PathResolveOperation operation,
            string path
        )
        {
            if (instance == null || path == null)
            {
                yield break;
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
            setAccessors.InvokeSetOperation(target, getAccessors.InvokeGetOperation(source));
        }

        public static object InvokeGetOperation(this MethodInfo[] accessors, object instance)
        {
            var currentInstance = instance;

            foreach (var getter in accessors.Take(accessors.Length - 1))
            {
                currentInstance = getter.Invoke(currentInstance, null);
            }

            return accessors.Last().Invoke(currentInstance, null);
        }

        public static void InvokeSetOperation(
            this MethodInfo[] accessors,
            object instance,
            params object[] values
        )
        {
            var currentInstance = instance;

            foreach (var getter in accessors.Take(accessors.Length - 1))
            {
                currentInstance = getter.Invoke(currentInstance, null);
            }

            var setter = accessors.Last();
            setter.Invoke(currentInstance, values);
        }

        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            baseType = ResolveGenericTypeDefinition(baseType);

            var currentDerived = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            while (currentDerived != typeof(object))
            {
                if (baseType == currentDerived || HasAnyInterfaces(currentDerived, baseType))
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

        private static bool HasAnyInterfaces(Type type, Type interfaceType)
        {
            return type.GetInterfaces()
                .Any(childInterface =>
                {
                    if (childInterface == interfaceType)
                    {
                        return true;
                    }

                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == interfaceType;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type type)
        {
            var shouldUseGenericType = true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() != type)
            {
                shouldUseGenericType = false;
            }

            if (type.IsGenericType && shouldUseGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return type;
        }
    }
}
