using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor.Graphs;

using UnityEngine.UIElements;

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

        public string ComponentPropertyBindingsText => "Component Property Bindings";

        public string BindingSourceUnboundMessageText => "Select the binding source path";
        public string BindingTargetUnboundMessageText => "Select the binding target";
        public string BindingUnassignableMessageText =>
            "The binding source is not assignable to the binding target";

        public string TargetGameObjectText => "Target GameObject";

        public string TargetComponentText => "Target Component";

        public string ComponentPropertyBindingCondensedLabelFormat_Type_Source_Target_Component =>
            "<color=green>✔</color> <color=blue>{0}</color> <b>{1}</b> binds to <b>{2}</b> ({3})";

        public string ContainerPropertyBindingsText => "Container Property Bindings";

        public string MoveUpButtonText => "▲";
        public string MoveDownButtonText => "▼";

        public string ExpandButtonText => "…";

        public string CondenseButtonText => "↸";

        public string BindingElementTemplateMissingMessageText =>
            "Please specify a an element template ( == a prefab with a View component)";
        public string BindingElementTemplateIsNotAssignableMessageText =>
            "Please specify an element template with a View component";
        public string ContainerPropertyBindingCondensedLabelFormat_Type_Source_Target_Template =>
            "<color=green>✔</color> <color=blue>{0}</color> <b>{1}</b> expands into <b>{2}</b> ({3})";
    }
}
