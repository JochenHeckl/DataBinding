using System;

using UnityEngine;

namespace JH.DataBinding.Examples.GettingStarted
{
    public class CubeViewDataSource : DataSourceBase<CubeViewDataSource>
    {
        public Vector3 CubeScale { get; set; } = Vector3.one;
        public Color CubeColor { get; set; } = Color.grey;
    }
}
