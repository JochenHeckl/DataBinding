using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH.DataBinding.Editor
{
    internal class InternalErrorReport : VisualElement
    {
        internal InternalErrorReport(Exception exception)
        {
            this.ApplyErrorTextStyle();

            var errorLabel = new TextElement();
            errorLabel.text = DataBindingCommonData.EditorDisplayText.EditorErrorMessageText;
            errorLabel.ApplyInternalErrorSectionStyle();
            Add(errorLabel);

            var message = new TextElement();
            message.text = exception.Message;
            message.ApplyInternalErrorSectionStyle();
            Add(message);

            var reportBugLink = new Button(() => HandleReportError(exception));
            reportBugLink.text = DataBindingCommonData.EditorDisplayText.ReportErrorButtonText;
            reportBugLink.ApplyReportErrorButtonStyle();
            Add(reportBugLink);
        }

        private static void HandleReportError(Exception exception)
        {
            var link =
                $"https://github.com/JochenHeckl/DataBinding/issues/new?assignees=&labels=&template=bug_report.md&title={exception.Message}";

            Application.OpenURL(link);
        }
    }
}
