using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Experimental
{
    [Serializable]
    public class VisualElementPropertyBinding
    {
        private INotifyDataSourceChanged _dataSource;

        [SerializeField]
        private string _sourcePath;

        private VisualElement _rootVisualElement;

        [SerializeField]
        private string _targetVisualElementQuery;

        [SerializeField]
        private string _targetPath;

        private MethodInfo[] _dataSourcePropertyAccessors;
        private MethodInfo[] _targetPropertyAccessors;

        private VisualElement _targetInstance;

        public INotifyDataSourceChanged DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                BindSource();
            }
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                BindSource();
            }
        }

        public VisualElement RootVisualElement
        {
            get { return _rootVisualElement; }
            set
            {
                _rootVisualElement = value;
                BindTarget();
            }
        }

        public string TargetVisualElementQuery
        {
            get { return _targetVisualElementQuery; }
            set
            {
                _targetVisualElementQuery = value;
                BindTarget();
            }
        }

        public string TargetPath
        {
            get { return _targetPath; }
            set
            {
                _targetPath = value;
                BindTarget();
            }
        }

        private void BindSource()
        {
            if ((_dataSource != null) && !string.IsNullOrEmpty(_sourcePath))
            {
                _dataSourcePropertyAccessors = _dataSource
                    .ResolvePublicPropertyPath(SourcePath, PathResolveOperation.GetValue)
                    .ToArray();
            }
            else
            {
                _dataSourcePropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        private void BindTarget()
        {
            if (
                (_rootVisualElement != null)
                && !string.IsNullOrEmpty(_targetVisualElementQuery)
                && !string.IsNullOrEmpty(_targetPath)
            )
            {
                _targetInstance = RootVisualElement.Q(_targetVisualElementQuery);
                _targetPropertyAccessors = _targetInstance
                    .ResolvePublicPropertyPath(TargetPath, PathResolveOperation.SetValue)
                    .ToArray();
            }
            else
            {
                _targetPropertyAccessors = Array.Empty<MethodInfo>();
            }
        }

        public void UpdateBinding()
        {
            if (
                (_dataSource != null)
                && (_targetInstance != null)
                && (_dataSourcePropertyAccessors != null)
                && _dataSourcePropertyAccessors.Any()
                && (_targetPropertyAccessors != null)
                && _targetPropertyAccessors.Any()
            )
            {
                _dataSource.SyncValue(
                    _targetInstance,
                    _dataSourcePropertyAccessors,
                    _targetPropertyAccessors
                );
            }
        }
    }
}
