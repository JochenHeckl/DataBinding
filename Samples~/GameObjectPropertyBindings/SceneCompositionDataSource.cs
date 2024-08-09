using JH.DataBinding;

namespace JH.DataBinding.Samples.GameObjectPropertyBindings
{
    public class SceneCompositionDataSource : DataSourceBase<SceneCompositionDataSource>
    {
        public bool EnabledPostprocessing {get; set;}
        public bool ShowCube {get; set;}
    }
}
