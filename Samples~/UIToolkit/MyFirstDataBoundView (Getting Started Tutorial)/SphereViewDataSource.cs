using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Examples.GettingStarted
{
    public class SphereViewModel : DataSourceBase<SphereViewModel>
    {
        public Vector3 SphereOffset { get; set; } = Vector3.zero;
    }
}
