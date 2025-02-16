using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JH.DataBinding
{
    [Serializable]
    public class ContainerPropertyBinding
    {
        [SerializeField]
        [EnumerableBindingSourcePath]
        private string sourcePath;

        [SerializeField]
        private Transform targetContainer;

        [SerializeField]
        private View elementTemplate;

        private object dataSource;

        private MethodInfo[] dataSourcePropertyAccessors;

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

        public Transform TargetContainer
        {
            get => targetContainer;
            set => targetContainer = value;
        }

        public View ElementTemplate
        {
            get => elementTemplate;
            set => elementTemplate = value;
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

        public void UpdateBinding()
        {
            if (
                (dataSourcePropertyAccessors != null)
                && (ElementTemplate != null)
                && (TargetContainer != null)
            )
            {
                var boundInstances = (
                    (
                        dataSourcePropertyAccessors.InvokeGetAccessChain(dataSource)
                        as IEnumerable<INotifyDataSourceChanged>
                    ) ?? Array.Empty<INotifyDataSourceChanged>()
                ).ToArray();

                AddMissingChildren(boundInstances.Length);
                RemoveSuperfluousChildren(boundInstances.Length);

                for (var childIndex = 0; childIndex < boundInstances.Length; childIndex++)
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    var view = TargetContainer.GetChild(childIndex).GetComponent<View>();
                    view.DataSource = boundInstances[childIndex];
                }
            }
        }

        private void AddMissingChildren(int numRequiredChildren)
        {
            while (TargetContainer.childCount < numRequiredChildren)
            {
                UnityEngine.Object.Instantiate(ElementTemplate, TargetContainer);
            }
        }

        private void RemoveSuperfluousChildren(int numberOfChildrenToKeep)
        {
            for (
                var childIndex = numberOfChildrenToKeep;
                childIndex < TargetContainer.childCount;
                ++childIndex
            )
            {
                UnityEngine.Object.Destroy(TargetContainer.GetChild(childIndex).gameObject);
            }
        }
    }
}
