using UnityEngine;

namespace JH.DataBinding
{
    public class GameObjectPropertyAccess : MonoBehaviour
    {
        public bool GameObjectEnabled
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public int GameObjectLayer
        {
            get { return gameObject.layer; }
            set { gameObject.layer = value; }
        }
    }
}
