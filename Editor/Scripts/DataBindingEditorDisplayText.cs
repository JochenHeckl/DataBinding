using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
    internal class DataBindingEditorDisplayText : IDataBindingEditorDisplayText
    {
        public string HeavyCheckmark => "✔";
        public string MissingDataSourcesErrorText =>
            "No <b>DataSource</b>s were found in this project.";

        public string MissingDataSourcesInfoText =>
            "Please define at least one class that implements <b>INotifyDataSourceChanged</b> that you want this View to bind to.";

        public string DataSourceTypeText => "DataSource Type";

        public string EditSourceText => "…";

        public string NoSourceCodeAvailableText => "!";
        public string NoSourceCodeAvailableToolTip =>
            "To enable tooling to find your DataSource, name the source file according to the class name (class AAA => AAA.cs).";

        public string SourcePathText => "Source Path";

        public string EditorErrorMessageText =>
            "An internal error happended setting up the editor view. Please file an Issue.";
        public string EditorErrorMessageLinkText =>
            "An internal error happended setting up the editor view. Please file an Issue.";

        public string ComponentPropertyBindings => "Component Property Bindings";

        public string BindingSourceUnboundMessageText => "Select a binding source";
        public string BindingTargetUnboundMessageText => "Select a binding target";
        public string BindingUnassignableMessageText =>
            "The binding source is not assignable to the binding target";

        public string TargetGameObjectText => "Target GameObject";

        public string TargetComponentText => "Target Component";

        public string ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component =>
            "<color=green>✔</color> <color=blue>{0}</color> <b>{1}</b> binds to <b>{2}</b> ({3})";
    }
}
