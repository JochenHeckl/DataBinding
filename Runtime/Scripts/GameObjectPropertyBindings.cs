using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH.DataBinding
{
    public class GameObjectPropertyBindings : MonoBehaviour
    {
        public int GameObjectName
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

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
