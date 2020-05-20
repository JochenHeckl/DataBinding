namespace de.JochenHeckl.Unity.DataBinding
{
    internal interface IComponentAdapter<ComponentType>
    {
        ComponentType Component
        {
            get;
            set;
        }
    }
}
