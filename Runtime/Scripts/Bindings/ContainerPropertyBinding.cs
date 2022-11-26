using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    [Serializable]
    public class ContainerPropertyBinding
    {
#if UNITY_EDITOR
        [SerializeField]
        public bool showExpanded;
#endif

        [SerializeField]
        private Transform targetContainer;

        [SerializeField]
        private View elementTemplate;

        [SerializeField]
        private string sourcePath;

        private object dataSource;

        private MethodInfo[] _dataSourcePropertyAccessors;

        public object DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                BindSource();
            }
        }

        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                sourcePath = value;
                BindSource();
            }
        }

        public Transform TargetContainer
        {
            get { return targetContainer; }
            set { targetContainer = value; }
        }

        public View ElementTemplate
        {
            get { return elementTemplate; }
            set { elementTemplate = value; }
        }

        private void BindSource()
        {
            if (dataSource != null && !string.IsNullOrEmpty(sourcePath))
            {
                _dataSourcePropertyAccessors = dataSource
                    .ResolvePublicPropertyPath(PathResolveOperation.GetValue, SourcePath)
                    .ToArray();
            }
            else
            {
                _dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        public void UpdateBinding()
        {
            if ((_dataSourcePropertyAccessors != null) && (targetContainer != null))
            {
                var elements =
                    _dataSourcePropertyAccessors.InvokeGetOperation(dataSource)
                    as IEnumerable<INotifyDataSourceChanged>;

                if (elements == null)
                {
                    foreach (Transform child in targetContainer)
                    {
                        UnityEngine.Object.Destroy(child.gameObject);
                    }

                    return;
                }

                var elementCount = elements.Count();

                while (elementCount > targetContainer.childCount)
                {
                    UnityEngine.Object.Instantiate(elementTemplate, targetContainer);
                }

                while (elementCount < targetContainer.childCount)
                {
                    UnityEngine.Object.Destroy(targetContainer.GetChild(elementCount));
                }

                var childIndex = 0;

                foreach (var element in elements)
                {
                    var view = targetContainer.GetChild(childIndex++).GetComponent<View>();
                    view.DataSource = element;
                }
            }
        }
    }
}
