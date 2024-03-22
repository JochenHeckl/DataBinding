using System.Linq;

using UnityEngine;

namespace JH.DataBinding.Examples.GettingStarted
{
    /// <summary>
    /// This class is here as a replacement for whatever
    /// application logic your application might implement.
    /// You application might be arbitrarily complex and expose
    /// many data sources - static data sources as well as dynamic ones.
    /// This application is about changing the scale and color of a cube.
    /// That's it for this tutorial.
    /// So the sole data source exposed is a simple CubeViewDataSource.
    /// </summary>
    public class PlaceholderApplicationLogic
    {
        public CubeViewDataSource CubeViewDataSource { get; set; }
        private float _nextCubeUpdateTimeSeconds;

        public void Initialize()
        {
            CubeViewDataSource = new CubeViewDataSource();
            _nextCubeUpdateTimeSeconds = 0f;
        }

        public void Update(float simulationTimeSeconds)
        {
            if (_nextCubeUpdateTimeSeconds < simulationTimeSeconds)
            {
                _nextCubeUpdateTimeSeconds += 3.0f;

                CubeViewDataSource.NotifyChanges(x =>
                {
                    x.CubeScale = Vector3.one + Random.insideUnitSphere;
                    x.CubeColor = Random.ColorHSV(0, 1, 0, 1);
                });
            }
        }
    }
}
