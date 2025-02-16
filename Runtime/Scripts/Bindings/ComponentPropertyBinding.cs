using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding
{
    [Serializable]
    public class ComponentPropertyBinding
    {
        [SerializeField]
        [BindingSourcePath]
        private string sourcePath;

        [SerializeField]
        private GameObject targetGameObject;

        [SerializeField]
        [BindingTargetComponent]
        private Component targetComponent;

        [SerializeField]
        [BindingTargetPath]
        private string targetPath;

        private object dataSource;

        private MethodInfo[] dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
        private MethodInfo[] targetPropertyAccessors = Array.Empty<MethodInfo>();

        public object DataSource
        {
            get => dataSource;
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;

                    BindSource();
                }
            }
        }

        public string SourcePath
        {
            get => sourcePath;
            set
            {
                if (sourcePath != value)
                {
                    sourcePath = value;

                    BindSource();
                }
            }
        }

        public GameObject TargetGameObject
        {
            get => targetGameObject;
            set
            {
                if (targetGameObject != value)
                {
                    targetGameObject = value;
                    targetComponent = null;

                    BindTarget();
                }
            }
        }
        public Component TargetComponent
        {
            get => targetComponent;
            set
            {
                if (targetComponent != value)
                {
                    targetComponent = value;
                    targetGameObject = value.gameObject;

                    BindTarget();
                }
            }
        }

        public string TargetPath
        {
            get => targetPath;
            set
            {
                if (targetPath != value)
                {
                    targetPath = value;

                    BindTarget();
                }
            }
        }

        private void BindSource()
        {
            if (dataSource != null && !String.IsNullOrEmpty(sourcePath))
            {
                dataSourcePropertyAccessors = dataSource
                    .ResolvePublicPropertyPath(SourcePath, PathResolveOperation.GetValue)
                    .ToArray();
            }
            else
            {
                dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        private void BindTarget()
        {
            if (targetComponent != null && !String.IsNullOrEmpty(targetPath))
            {
                targetPropertyAccessors = targetComponent
                    .ResolvePublicPropertyPath(targetPath, PathResolveOperation.SetValue)
                    .ToArray();
            }
            else
            {
                targetPropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        public void UpdateBinding()
        {
            if (dataSourcePropertyAccessors.Length == 0)
            {
                return;
            }

            if (targetPropertyAccessors.Length == 0)
            {
                BindTarget();
            }

            if (targetPropertyAccessors.Length == 0)
            {
                return;
            }

            var value = dataSourcePropertyAccessors.InvokeGetAccessChain(dataSource);
            targetPropertyAccessors.InvokeSetAccessChain(targetComponent, value);
        }
    }
}
