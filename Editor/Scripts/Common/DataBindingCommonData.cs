using System;
using System.Linq;

namespace JH.DataBinding.Editor
{
    public static class DataBindingCommonData
    {
        public static Type[] GetValidDataSourceTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(DataSourceFilterFunc)
                .ToArray();

            bool DataSourceFilterFunc(Type x) =>
                !x.IsAbstract
                && !x.IsGenericType
                && !x.IsInterface
                && x.InheritsOrImplements(typeof(INotifyDataSourceChanged));
        }
    }
}
