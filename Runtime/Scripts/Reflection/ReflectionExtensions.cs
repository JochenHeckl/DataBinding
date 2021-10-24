using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace de.JochenHeckl.Unity.DataBinding
{
    public static class ReflectionExtensions
    {
        public static ValueType ResolveInstanceMember<ValueType>( this object instance, string path )
        {
            if( instance == null )
            {
                return default;
            }

            var pathFragments = path.Split('.');
            var fragmentInstance = instance;

            foreach ( var fragmentPath in pathFragments.Take(pathFragments.Length - 1) )
            {
                if ( fragmentInstance == null )
                {
                    throw new InvalidOperationException(string.Format("Binding path {0} does not match dataSource of type {1}", path, instance.GetType().Name));
                }

                var memberInfo = fragmentInstance.GetType().GetMember(fragmentPath, BindingFlags.Instance);

                fragmentInstance = memberInfo.GetValue(0);
            }

            return (ValueType) fragmentInstance.GetType().GetMember(pathFragments.Last(), BindingFlags.Instance).GetValue(0);
        }

        public static string GetFriendlyName( this Type type )
        {
            if ( type == null )
            {
                return "None";
            }

            string friendlyName = type.Name;

            if ( type.IsGenericType )
            {
                friendlyName = friendlyName.Split('`').First();

                friendlyName += "<";

                Type[] typeParameters = type.GetGenericArguments();

                for ( int i = 0; i < typeParameters.Length; ++i )
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += ( i == 0 ? typeParamName : "," + typeParamName );
                }

                friendlyName += ">";
            }

            if ( type.IsNested )
            {
                return type.DeclaringType.GetFriendlyName() + "." + friendlyName;
            }

            return friendlyName;
        }

        public static PropertyInfo ResolvePublicPropertyPath( this object instance, string path )
        {
            if ( instance == null )
            {
                return null;
            }

            var pathFragments = path.Split('.');
            var fragmentInstance = instance;

            foreach ( var fragmentPath in pathFragments.Take(pathFragments.Count() - 1) )
            {
                if ( fragmentInstance == null )
                {
                    throw new InvalidOperationException(string.Format("Binding path {0} does not match dataSource of type {1}", path, instance.GetType().Name));
                }

                var propertyInfo = fragmentInstance.GetType().GetProperty(fragmentPath, BindingFlags.Instance | BindingFlags.Public);

                var boundPropertyGetter = propertyInfo.GetGetMethod();
                fragmentInstance = boundPropertyGetter.Invoke(fragmentInstance, null);
            }

            return fragmentInstance.GetType().GetProperty(pathFragments.Last(), BindingFlags.Instance | BindingFlags.Public );
        }

        public static bool InheritsOrImplements( this Type type, Type baseType )
        {
            baseType = ResolveGenericTypeDefinition(baseType);

            var currentDerived = type.IsGenericType
                                   ? type.GetGenericTypeDefinition()
                                   : type;

            while ( currentDerived != typeof(object) )
            {
                if ( baseType == currentDerived || HasAnyInterfaces( currentDerived, baseType ) )
                {
                    return true;
                }

                currentDerived = currentDerived.BaseType != null
                               && currentDerived.BaseType.IsGenericType
                                   ? currentDerived.BaseType.GetGenericTypeDefinition()
                                   : currentDerived.BaseType;

                if ( currentDerived == null )
                {
                    return false;
                }
            }
            return false;
        }

        private static bool HasAnyInterfaces( Type type, Type interfaceType )
        {
            return type.GetInterfaces()
                .Any(childInterface =>
                {
                    if( childInterface == interfaceType )
                    {
                        return true;
                    }

                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == interfaceType;
                });
        }

        private static Type ResolveGenericTypeDefinition( Type type )
        {
            var shouldUseGenericType = true;

            if ( type.IsGenericType && type.GetGenericTypeDefinition() != type )
            {
                shouldUseGenericType = false;
            }

            if ( type.IsGenericType && shouldUseGenericType )
            {
                type = type.GetGenericTypeDefinition();
            }

            return type;
        }
    }
}
