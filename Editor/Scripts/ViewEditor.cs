using System;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
	[CustomEditor( typeof( View ), true )]
	public class ViewEditor : UnityEditor.Editor
	{
		private readonly string viewEditorUXMLFile
			= "Packages/de.jochenheckl.unity.dataBinding/Editor/UI/ViewEditor.uxml";

		private readonly string viewEditorStyleSheetFile
			= "Packages/de.jochenheckl.unity.dataBinding/Editor/UI/ViewEditor.uss";

		private readonly string addPropertyBindingButtonName = "AddPropertyBindingButton";
		private readonly string propertyBindingContainerName = "PropertyBindingContainer";
		
		private readonly string viewModelTypeValueName = "ViewModelTypeValue";

		private static VisualTreeAsset sharedViewVisualTreeAsset;
		private static StyleSheet sharedStyleSheetAsset;
		private static Type[] validDataSources;

		private VisualElement editorRootElement;
		private View view;

		public void OnEnable()
		{
			view = target as View;

			if ( view.componentPropertyBindings == null )
			{
				view.componentPropertyBindings = Array.Empty<ComponentPropertyBinding>();
			}

			if ( sharedViewVisualTreeAsset == null )
			{
				sharedViewVisualTreeAsset
					= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( viewEditorUXMLFile );
			}

			if ( sharedStyleSheetAsset  == null )
			{
				sharedStyleSheetAsset
					= AssetDatabase.LoadAssetAtPath<StyleSheet>( viewEditorStyleSheetFile);
			}


			if ( validDataSources == null )
			{
				validDataSources = GetValidDataSourceTypes();
			}
		}

		public override VisualElement CreateInspectorGUI()
		{
			editorRootElement = new VisualElement();

			MakeEditorView();

			return editorRootElement;
		}

		private void MakeEditorView()
		{
			editorRootElement.Clear();

			editorRootElement.styleSheets.Add( sharedStyleSheetAsset );

			sharedViewVisualTreeAsset.CloneTree( editorRootElement );

			var viewModelTypeValue = editorRootElement.Q<DropdownField>( viewModelTypeValueName );

			viewModelTypeValue.choices = validDataSources
				.Select( x => x.GetFriendlyName() )
				.ToList();

			viewModelTypeValue.value = validDataSources
				.Select( x => x.GetFriendlyName() )
				.FirstOrDefault( x => x == view.dataSourceType.Type.GetFriendlyName() );

			viewModelTypeValue.RegisterValueChangedCallback( HandleDataSourceTypeChanged );

			var addPropertyBindingButton = editorRootElement.Q<Button>( addPropertyBindingButtonName );
			addPropertyBindingButton.clicked += HandleAddComponentPropertyBinding;

			var propertyBindingContainer = editorRootElement.Q<VisualElement>( propertyBindingContainerName );

			foreach ( var binding in view.componentPropertyBindings )
			{
				CreateComponentPropertyBinding( binding, propertyBindingContainer );
			}
		}

		private void CreateComponentPropertyBinding( ComponentPropertyBinding binding, VisualElement propertyBindingContainer )
		{
			var propertyBindingVisualElement = new ComponentPropertyBindingVisualElement(
				view.dataSourceType.Type,
				binding,
				StoreAndUpdateView,
				() => RemoveBinding( binding )
				);

			propertyBindingContainer.Add( propertyBindingVisualElement );
		}

		private void RemoveBinding( ComponentPropertyBinding binding )
		{
			view.componentPropertyBindings =
				view.componentPropertyBindings.Where( x => x != binding )
				.ToArray();

			StoreAndUpdateView();
		}

		private static Type[] GetValidDataSourceTypes()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			Func<Type, bool> dataSourceFilterFunc = ( x ) =>
				!x.IsAbstract
				&& !x.IsGenericType
				&& !x.IsInterface
				&& x.InheritsOrImplements( typeof( IDataSource ) );

			return assemblies
				.Where( x => !x.IsDynamic )
				.SelectMany( x => x.ExportedTypes )
				.Where( dataSourceFilterFunc )
				.ToArray();
		}

		private void HandleDataSourceTypeChanged( ChangeEvent<string> changeEvent )
		{
			view.dataSourceType.Type = validDataSources.FirstOrDefault( x => x.Name == changeEvent.newValue );
		}

		private void HandleAddComponentPropertyBinding()
		{
			view.componentPropertyBindings = view.componentPropertyBindings
				.Append( new ComponentPropertyBinding() { DataSource = view.DataSource } )
				.ToArray();

			StoreAndUpdateView();
		}

		private void StoreAndUpdateView()
		{
			EditorUtility.SetDirty( view );
			AssetDatabase.SaveAssetIfDirty( view );

			MakeEditorView();

			editorRootElement.MarkDirtyRepaint();
		}
	}
}