using de.JochenHeckl.Unity.DataBinding;

using UnityEngine;

public class CubeViewDataSource : DataSourceBase<CubeViewDataSource>
{
    public Vector3 CubeScale { get; set; } = Vector3.one;
    public Color CubeColor { get; set; } = Color.grey;
}