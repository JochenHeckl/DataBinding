﻿using System.Diagnostics;
using System.Reflection;

namespace de.JochenHeckl.Unity.DataBinding
{
    [DebuggerDisplay("{dataSource.GetType().Name}.{Path} => {BindingAdapter.GetType().Name}")]
    internal class OneWayComponentPropertyBinding : IComponentPropertyBinding
    {
        public object DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
                BindSource();
            }
        }

        public string SourcePath
        {
            get
            {
                return sourcePath;
            }
            set
            {
                sourcePath = value;
                BindSource();
            }
        }

        public object Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                BindTarget();
            }
        }

        public string TargetPath
        {
            get
            {
                return targetPath;
            }
            set
            {
                targetPath = value;
                BindTarget();
            }
        }

        private void BindSource()
        {
            if ( dataSource != null && !string.IsNullOrEmpty(sourcePath) )
            {
                var propertyInfo = dataSource.ResolvePublicPropertyPath(SourcePath);
                dataSourcePropertyGetter = ( propertyInfo != null ) ? propertyInfo.GetGetMethod() : null;
            }
            else
            {
                dataSourcePropertyGetter = null;
            }
        }

        private void BindTarget()
        {
            if ( target != null && !string.IsNullOrEmpty(targetPath) )
            {
                var propertyInfo = target.ResolvePublicPropertyPath(targetPath);
                targetPropertySetter = ( propertyInfo != null ) ? propertyInfo.GetSetMethod() : null;
            }
            else
            {
                targetPropertySetter = null;
            }
        }

        public void UpdateBinding()
        {
            if ( (dataSourcePropertyGetter != null) && (targetPropertySetter != null) )
            {
                var value = dataSourcePropertyGetter.Invoke(dataSource, null);
                targetPropertySetter.Invoke(target, new object[] { value });
            }
        }

        public void ReverseUpdateBinding()
        {
            throw new System.InvalidOperationException("OneWayBindings must never reverse update.");
        }

        private object dataSource;
        private string sourcePath;

        private object target;
        private string targetPath;

        private MethodInfo dataSourcePropertyGetter;
        private MethodInfo targetPropertySetter;
    }
}