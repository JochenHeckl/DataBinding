using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class BindableBehaviour : MonoBehaviour
    {
        public bool Active
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive( value ); }
        }
    }
}
