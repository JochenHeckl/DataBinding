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
    }
}
