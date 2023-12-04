namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal enum ComponentPropertyBindingState
    {
        SourceUnbound,
        TargetUnbound,
        Unassignable,
        Complete,
    }

    internal enum ContainerPropertyBindingState
    {
        SourceUnbound,
        TargetUnbound,
        ElementTemplateMissing,
        ElementTemplateIsNotAssignable,
        Complete,
    }

    internal enum VisualElementPropertyBindingState
    {
        RootVisualElementUnboud,
        SourceUnbound,
        TargetElementUnbound,
        TargetPropertyUnbound,
        Unassignable,
        Complete,
    }
}
