using JH.DataBinding;

namespace JH.DataBinding.Samples.GameObjectPropertyBindings
{
    public class SceneCompositionDataSource : DataSourceBase<SceneCompositionDataSource>
    {
        public string Name {get; set;}
        public bool EnabledPostprocessing {get; set;}
        public bool ShowCube {get; set;}
    }
}
