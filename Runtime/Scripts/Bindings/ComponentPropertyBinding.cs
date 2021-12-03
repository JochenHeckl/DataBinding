using System;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [Serializable]
    public class ComponentPropertyBinding
    {

#if UNITY_EDITOR
        [SerializeField] public bool showExpanded;
#endif

        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private Component targetComponent;
        [SerializeField] private string sourcePath;
        [SerializeField] private string targetPath;

        private object _dataSource;

        private MethodInfo[] _dataSourcePropertyAccessors;
        private MethodInfo[] _targetPropertyAccessors;

        public object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
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
            if ( _dataSource != null && !string.IsNullOrEmpty( sourcePath ) )
            {
                _dataSourcePropertyAccessors = _dataSource
                    .ResolvePublicPropertyPath( PathResolveOperation.GetValue, SourcePath )
                    .ToArray();
            }
            else
            {
                _dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        private void BindTarget()
        {
            if ( targetComponent != null && !string.IsNullOrEmpty( targetPath ) )
            {
                _targetPropertyAccessors = targetComponent
                    .ResolvePublicPropertyPath( PathResolveOperation.SetValue, targetPath )
                    .ToArray();
            }
            else
            {
                _targetPropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        public void UpdateBinding()
        {
            if ( _dataSourcePropertyAccessors == null )
            {
                return;
            }

            if ( _targetPropertyAccessors == null )
            {
                BindTarget();
            }

            if ( _dataSourcePropertyAccessors.Any() && _targetPropertyAccessors.Any() )
            {
                var value = _dataSourcePropertyAccessors.InvokeGetOperation( _dataSource );
                _targetPropertyAccessors.InvokeSetOperation( targetComponent, value );
            }
        }
    }
}