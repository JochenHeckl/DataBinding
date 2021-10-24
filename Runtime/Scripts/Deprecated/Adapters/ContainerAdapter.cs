using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class ContainerAdapter<ElementType> : IBindingAdapter<IEnumerable<ElementType>>
    {
        public ContainerAdapter( GameObject elementPrefabIn, Transform containerTransformIn )
        {
            containerTransform = containerTransformIn;
            elementPrefab = elementPrefabIn;
        }

        private IEnumerable<GameObject> Children
        {
            get
            {
                for (int childIdx = 0; childIdx < containerTransform.childCount; childIdx++)
                {
                    yield return containerTransform.GetChild( childIdx ).gameObject;
                }
            }
        }


        public IEnumerable<ElementType> Value
        {
            set
            {
                if (value == null)
                {
                    // remove all children
                    foreach (var child in Children)
                    {
                        GameObject.Destroy( child );
                    }

                    return;
                }


                if (containerTransform.childCount > value.Count())
                {
                    // remove superfluous children
                    foreach (var child in Children.Skip( value.Count() ))
                    {
                        GameObject.Destroy( child );
                    }
                }

                if (containerTransform.childCount < value.Count())
                {
                    // add missing children
                    foreach (var element in value.Skip( containerTransform.childCount ))
                    {
                        GameObject.Instantiate( elementPrefab, containerTransform, false );
                    }
                }

                // update bindings
                for (int childIdx = 0; childIdx < value.Count(); childIdx++)
                {
                    var child = containerTransform.GetChild( childIdx );
                    var viewBehaviour = child.GetComponent<ViewBehaviour>();

                    if (viewBehaviour != null)
                    {
                        viewBehaviour.DataSource = value.ElementAt( childIdx );
                    }
                }

            }
        }

        private readonly Transform containerTransform;
        private readonly GameObject elementPrefab;
    }
}
