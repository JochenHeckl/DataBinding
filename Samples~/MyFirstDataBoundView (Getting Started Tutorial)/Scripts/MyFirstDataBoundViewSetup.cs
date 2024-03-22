using JH.DataBinding;

using UnityEngine;

namespace JH.DataBinding.Examples.GettingStarted
{
    public class MyFirstDataBoundViewSetup : MonoBehaviour
    {
        public View view;
        private PlaceholderApplicationLogic _placeholderApplicationLogic;

        public void Start()
        {
            _placeholderApplicationLogic = new PlaceholderApplicationLogic();
            _placeholderApplicationLogic.Initialize();

            view.DataSource = _placeholderApplicationLogic.CubeViewDataSource;
        }

        // Update is called once per frame
        public void Update()
        {
            _placeholderApplicationLogic.Update(Time.time);
        }
    }
}
