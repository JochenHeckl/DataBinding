using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace JH.DataBinding.Samples.GameObjectPropertyBindings
{
    public class NewBehaGameObjectPropertyBindingsSetup : MonoBehaviour
    {
        public float changePropertiesIntervalSeconds;
        public View view;
        private float nextChangeSec;
        private SceneCompositionDataSource dataSource;

        void Start()
        {
            dataSource = new SceneCompositionDataSource();
            view.DataSource = dataSource;
            nextChangeSec = changePropertiesIntervalSeconds;
        }

        void Update()
        {
            if (Time.time > nextChangeSec)
            {
                nextChangeSec += changePropertiesIntervalSeconds;
                
                dataSource.NotifyChanges(
                    (x) =>
                    {
                        x.Name = "This name was set using DataBindings";
                        x.EnabledPostprocessing = Random.Range(0, 2) != 0;
                        x.ShowCube = Random.Range(0, 2) != 0;
                    }
                );
            }
        }
    }
}
