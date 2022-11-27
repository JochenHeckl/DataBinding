using System;

using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Examples.GettingStarted
{
    public class CubeViewModel : DataSourceBase<CubeViewModel>
    {
        public Vector3 CubeScale { get; set; } = Vector3.one;
        public Color CubeColor { get; set; } = Color.grey;

        public SphereViewModel[] Spheres { get; set; }
    }
}
