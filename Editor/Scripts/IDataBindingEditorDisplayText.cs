namespace JH.DataBinding.Editor
{
    public interface IDataBindingEditorDisplayText
    {
        string AssetPathLabel { get; }
        string CreateNewDataSourceTitle { get; }
        string CreateDataSourceTypeToolTipText { get; }
        string DataSourceTypeInspectorFilterLabel { get; }
        string DataSourceTypeText { get; }
        string EditDataSourceTooltip { get; }
        string EditDataSourceText { get; }
        string HeavyCheckMark { get; }
        string InvalidPathContent { get; }
        string InvalidPathTitle { get; }
        string MissingDataSourcesErrorText { get; }
        string NewDataSourceText { get; }
        string NewDataSourceTooltip { get; }
        string OKLabel { get; }
        string SourcePathText { get; }
        string EditorErrorMessageText { get; }
        string ComponentPropertyBindingsText { get; }
        string BindingMissingDataSourceAssignment { get; }
        string BindingNoBindableProperties { get; }
        string BindingSourceUnboundMessageText { get; }
        string BindingTargetUnboundMessageText { get; }
        string BindingUnassignableMessageText { get; }
        string TargetGameObjectText { get; }
        string TargetComponentText { get; }
        string TargetPathText { get; }
        string ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component { get; }
        string ContainerPropertyBindingsText { get; }
        string MoveUpButtonText { get; }
        string MoveDownButtonText { get; }
        string OpenFileBrowserButtonText { get; }
        string CondenseButtonText { get; }
        string BindingElementTemplateMissingMessageText { get; }
        string BindingElementTemplateIsNotAssignableMessageText { get; }
        string ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template { get; }
        string ReportErrorButtonText { get; }
        string CreateDefaultDataSourceText { get; }
    }
}
