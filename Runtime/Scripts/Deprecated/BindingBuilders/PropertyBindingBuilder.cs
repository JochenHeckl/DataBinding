using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    public abstract class PropertyBindingBuilder : MonoBehaviour
    {
        [BindingPath]
        public string sourcePath;
        
        public GameObject targetGameObject;

        public object GetDataSource()
        {
            var view = GetComponentInParent<ViewBehaviour>();

            if ( view != null )
            {
                return view.DataSource;
            }

            return null;
        }

        public Type GetDataSourceType()
        {
            var view = GetComponentInParent<ViewBehaviour>();

            if ( view != null )
            {
                if ( view.DataSource != null )
                {
                    return view.DataSource.GetType();
                }

#if UNITY_EDITOR
                if ( view.dataSourceType != null )
                {
                    return view.dataSourceType.Type;
                }
#endif // UNITY_EDITOR
            }

            return null;
        }

        public Type GetSourcePropertyType()
        {
            if ( sourcePath != null )
            {
                var dataSourceType = GetDataSourceType();

                if ( dataSourceType != null )
                {
                    var boundProperty = dataSourceType.GetProperties().FirstOrDefault(x => x.Name == sourcePath);

                    if ( boundProperty != null )
                    {
                        return boundProperty.PropertyType;
                    }
                }
            }

            return null;
        }
    }
}