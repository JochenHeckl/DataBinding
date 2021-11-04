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
        [SerializeField] public bool expandView;
#endif

        [SerializeField] private Transform targetContainer;
        [SerializeField] private View elementTemplate;
        [SerializeField] private string sourcePath;

        private object dataSource;

        private MethodInfo dataSourcePropertyGetter;
        
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

        public Transform TargetContainer
        {
            get
            {
                return targetContainer;
            }
            set
            {
                targetContainer = value;
            }
        }

        public View ElementTemplate
        {
            get
            {
                return elementTemplate;
            }
            set
            {
                elementTemplate = value;
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

        public void UpdateBinding()
        {
            if ( (dataSourcePropertyGetter != null) && (targetContainer != null) )
            {
                var elements = dataSourcePropertyGetter.Invoke( dataSource, null ) as IEnumerable<object>;
                var elementCount = elements.Count();

                while ( elementCount > targetContainer.childCount )
                {
                    UnityEngine.Object.Instantiate(elementTemplate, targetContainer);
                }

                while ( elementCount < targetContainer.childCount )
                {
                    UnityEngine.Object.Destroy(targetContainer.GetChild(elementCount));
                }

                var childIndex = 0;

                foreach( var element in elements )
                {
                    var view = targetContainer.GetChild(childIndex++).GetComponent<View>();
                    view.DataSource = element;
                }
            }
        }
    }
}