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
        [SerializeField]
        public bool showExpanded;
#endif

        [SerializeField]
        private GameObject targetGameObject;

        [SerializeField]
        private Component targetComponent;

        [SerializeField]
        private string sourcePath;

        [SerializeField]
        private string targetPath;

        private object dataSource;

        private MethodInfo[] dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
        private MethodInfo[] targetPropertyAccessors = Array.Empty<MethodInfo>();

        public object DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                BindSource();
            }
        }

        public string SourcePath
        {
            get => sourcePath;
            set
            {
                sourcePath = value;
                BindSource();
            }
        }

        public GameObject TargetGameObject
        {
            get => targetGameObject;
            set
            {
                targetGameObject = value;
                targetComponent = null;

                BindTarget();
            }
        }
        public Component TargetComponent
        {
            get => targetComponent;
            set
            {
                targetComponent = value;
                BindTarget();
            }
        }

        public string TargetPath
        {
            get => targetPath;
            set
            {
                targetPath = value;
                BindTarget();
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
