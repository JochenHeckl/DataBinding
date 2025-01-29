using System.Collections;
using JH.DataBinding;
using JH.DataBinding.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewEditorTest
{
    [Test]
    public void TestNoDataSourcesDefined()
    {
        GameObject view = new GameObject("View");
        var viewComponent = view.AddComponent<View>();

        viewComponent.componentPropertyBindings = new ComponentPropertyBinding[]
        {
            new ComponentPropertyBinding(),
        };

        var editor = Editor.CreateEditor(viewComponent) as ViewEditor;
        Assert.IsNotNull(editor, "Failed to create view editor.");

        // Clear out datasource type in case it was autoassigned
        viewComponent.dataSourceType = null;
        // var inspectorGUI = editor.CreateInspectorGUI();

        // TODO: This does not work, because the UIToolkitBindings do not get evaluated
        // and the listview is still empty
        // inspectorGUI.Bind(editor.serializedObject);

        // var componentPropertyBindings = inspectorGUI.Q<ComponentPropertyBindingListView>();
        // Assert.AreEqual(componentPropertyBindings.itemsSource.Count, 1);

        // var binding = componentPropertyBindings.GetRootElementForIndex(0);
        // Assert.AreEqual(
        //     binding.Q<PropertyField>().label,
        //     DataBindingCommonData.EditorDisplayText.BindingMissingDataSourceAssignment
        // );

        GameObject.DestroyImmediate(view);
    }
}
