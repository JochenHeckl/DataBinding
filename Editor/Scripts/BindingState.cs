namespace JH.DataBinding.Editor
{
    internal enum ComponentPropertyBindingState
    {
        MissingDataSourceAssignment,
        NoBindableProperties,
        SourceUnbound,
        TargetUnbound,
        Unassignable,
        Complete,
    }

    internal enum ContainerPropertyBindingState
    {
        MissingDataSourceAssignment,
        NoBindableProperties,
        SourceUnbound,
        TargetUnbound,
        ElementTemplateMissing,
        ElementTemplateIsNotAssignable,
        Complete,
    }
}
