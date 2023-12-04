namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal interface IDataBindingEditorDisplayText
    {
        string HeavyCheckmark { get; }
        string MissingDataSourcesErrorText { get; }
        string MissingDataSourcesInfoText { get; }
        string DataSourceTypeText { get; }
        string EditSourceText { get; }
        string NoSourceCodeAvailableToolTip { get; }
        string SourcePathText { get; }
        string EditorErrorMessageText { get; }
        string ComponentPropertyBindingsText { get; }
        string BindingSourceUnboundMessageText { get; }
        string BindingTargetUnboundMessageText { get; }
        string BindingUnassignableMessageText { get; }
        string TargetGameObjectText { get; }
        string TargetComponentText { get; }
        string ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component { get; }
        string ContainerPropertyBindingsText { get; }
        string MoveUpButtonText { get; }
        string MoveDownButtonText { get; }
        string ExpandButtonText { get; }
        string CondenseButtonText { get; }
        string BindingElementTemplateMissingMessageText { get; }
        string ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template { get; }
    }
}
