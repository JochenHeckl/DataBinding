using UnityEngine;

namespace JH.DataBinding.Examples.ContainerBindingTest
{
public class ItemDataSource : DataSourceBase<ItemDataSource>
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Color Color { get; set; }
}

}