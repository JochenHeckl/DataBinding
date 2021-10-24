using System;
using System.Reflection;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [Serializable]
    public class ComponentPropertyBinding
    {
        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private Component targetComponent;
        [SerializeField] private string sourcePath;
        [SerializeField] private string targetPath;

        private object dataSource;

        private MethodInfo dataSourcePropertyGetter;
        private MethodInfo targetPropertySetter;

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

        public GameObject TargetGameObject
        {
            get
            {
                return targetGameObject;
            }
            set
            {
                targetGameObject = value;
                targetComponent = null;
                
                BindTarget();
            }
        }
        public Component TargetComponent
        {
            get
            {
                return targetComponent;
            }
            set
            {
                targetComponent = value;
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
            if ( dataSource != null && !string.IsNullOrEmpty( sourcePath ) )
            {
                var propertyInfo = dataSource.ResolvePublicPropertyPath( SourcePath );
                dataSourcePropertyGetter = (propertyInfo != null) ? propertyInfo.GetGetMethod() : null;
            }
            else
            {
                dataSourcePropertyGetter = null;
            }
        }

        private void BindTarget()
        {
            if ( targetComponent != null && !string.IsNullOrEmpty( targetPath ) )
            {
                var propertyInfo = targetComponent.ResolvePublicPropertyPath( targetPath );
                targetPropertySetter = (propertyInfo != null) ? propertyInfo.GetSetMethod() : null;
            }
            else
            {
                targetPropertySetter = null;
            }
        }

        public void UpdateBinding()
        {
            if ( targetPropertySetter == null )
            {
                BindTarget();
            }

            if ( (dataSourcePropertyGetter != null) && (targetPropertySetter != null) )
            {
                var value = dataSourcePropertyGetter.Invoke( dataSource, null );
                targetPropertySetter.Invoke( targetComponent, new object[] { value } );
            }
        }
    }
}