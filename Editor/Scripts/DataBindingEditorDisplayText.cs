using System;
using System.IO;

namespace JH.DataBinding.Editor
{
    internal class DataBindingEditorDisplayText : IDataBindingEditorDisplayText
    {
        public string AssetPathLabel => "Asset Path";

        public string BindingElementTemplateIsNotAssignableMessageText =>
            "Please specify an element template with a View component";

        public string BindingElementTemplateMissingMessageText =>
            "Please specify an element template ( == a prefab with a View component)";

        public string BindingMissingDataSourceAssignment =>
            "Assign a DataSource to the hosting view.";

        public string BindingNoBindableProperties =>
            "The DataSource does not have bindable properties.";

        public string BindingSourceUnboundMessageText => "Select the binding source path";

        public string BindingTargetUnboundMessageText =>
            "<size=+2><color=red>The Binding target is incomplete</color></size>";

        public string BindingUnassignableMessageText =>
            "The binding source is not assignable to the binding target";

        public string ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component =>
            "<size=+1><color=green>✔</color> <color=#b0b0f0>{0}</color> <b>{1}</b> binds to <b>{2}</b> ({3})</size>";

        public string ComponentPropertyBindingsText => "Component Property Bindings";

        public string CondenseButtonText => "↸";

        public string ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template =>
            "<color=green>✔</color> <color=lightblue>{0}</color> <b>{1}</b> expands into <b>{2}</b> ({3})";

        public string ContainerPropertyBindingsText => "Container Property Bindings";

        public string CreateDataSourceTypeToolTipText =>
            "Enter the name of the DataSource to create.";

        public string CreateDefaultDataSourceText => "Create default DataSource";

        public string CreateNewDataSourceTitle => "Create new DataSource";

        public string DataSourceTypeInspectorFilterLabel => "DataSource Type Filter";

        public string DataSourceTypeText => "DataSource Type";

        public string EditDataSourceText => "Edit";

        public string EditDataSourceTooltip => "Edit the sourcecode of the selected Datasource.";

        public string EditorErrorMessageText =>
            "An internal error occured setting up the editor view. Please file an Issue.";

        public string HeavyCheckMark => "✔";

        public string InvalidPathContent =>
            "Please select a path inside the project tree structure.";

        public string InvalidPathTitle => "Invalid Path";

        public string MissingDataSourcesErrorText =>
            $"No <b>data source</b> was found in this project.{Environment.NewLine}{Environment.NewLine}Please define at least one class that implements <b>INotifyDataSourceChanged</b> that you want this View to bind to.{Environment.NewLine}{Environment.NewLine}The simplest way to create a <b>data source</b> is by deriving from <b>DataSourceBase</b>.";

        public string MoveDownButtonText => "▼";

        public string MoveUpButtonText => "▲";

        public string NewDataSourceText => "New";

        public string NewDataSourceTooltip => "Create a new empty datasource.";

        public string NoDataSourcesText => "No data sources found.";

        public string OKLabel => "OK";

        public string OpenFileBrowserButtonText => "…";

        public string ReportErrorButtonText => "Report Error 🔗";

        public string SourcePathText => "Source Path";

        public string TargetComponentText => "Target Component";

        public string TargetGameObjectText => "Target GameObject";

        public string TargetPathText => "Target Path";
    }
}
