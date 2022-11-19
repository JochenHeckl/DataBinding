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
    }
}
