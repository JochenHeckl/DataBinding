using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

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
        private readonly string addContainerBindingButtonName = "AddContainerBindingButton";
        private readonly string propertyBindingContainerName = "PropertyBindingContainer";
        private readonly string containerBindingContainerName = "ContainerBindingContainer";
        

        private readonly string viewModelTypeValueName = "ViewModelTypeValue";

		private static VisualTreeAsset sharedViewVisualTreeAsset;
		private static StyleSheet sharedStyleSheetAsset;
		private static Type[] validDataSources;

		private VisualElement editorRootElement;
		private View view;

        public void OnEnable()
		{
			view = target as View;

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

            var addContainerBindingButton = editorRootElement.Q<Button>(addContainerBindingButtonName);
            addContainerBindingButton.clicked += HandleAddContainerPropertyBinding;
            

            var propertyBindingContainer = editorRootElement.Q<VisualElement>( propertyBindingContainerName );

            if ( view.componentPropertyBindings.Length > 0 )
            {
                foreach ( var binding in view.componentPropertyBindings )
                {
                    CreateComponentPropertyBindingVisualElement(binding, propertyBindingContainer);
                }
            }
            else
            {
                propertyBindingContainer.Add(new Label("This view does not contain property bindings."));
            }

            var containerBindingContainer = editorRootElement.Q<VisualElement>(containerBindingContainerName);

            if ( view.containerPropertyBindings.Length > 0 )
            {
                foreach ( var binding in view.containerPropertyBindings )
                {
                    CreateContainerPropertyBindingVisualElement(binding, containerBindingContainer);
                }
            }
            else
            {
                containerBindingContainer.Add(new Label("This view does not contain container bindings."));
            }
        }

		private void CreateComponentPropertyBindingVisualElement( ComponentPropertyBinding binding, VisualElement bindingContainer )
		{
			var propertyBindingVisualElement = new ComponentPropertyBindingVisualElement(
				view.dataSourceType.Type,
				binding,
				StoreAndUpdateView,
                binding.expandView,
                ( view.componentPropertyBindings.First() != binding ) ? MoveBindingUp : (Action<ComponentPropertyBinding>) null,
                ( view.componentPropertyBindings.Last() != binding ) ? MoveBindingDown : (Action<ComponentPropertyBinding>) null,
                () => ToggleExpansion( binding ),
                () => RemoveBinding( binding )
				);

            bindingContainer.Add( propertyBindingVisualElement );
		}

        private void CreateContainerPropertyBindingVisualElement(ContainerPropertyBinding binding, VisualElement bindingContainer)
        {
            var containerBindingVisualElement = new ContainerPropertyBindingVisualElement(
                view.dataSourceType.Type,
                binding,
                StoreAndUpdateView,
                binding.expandView,
                (view.containerPropertyBindings.First() != binding) ? MoveBindingUp : (Action<ContainerPropertyBinding>)null,
                (view.containerPropertyBindings.Last() != binding) ? MoveBindingDown : (Action<ContainerPropertyBinding>)null,
                () => ToggleExpansion(binding),
                () => RemoveBinding(binding)
                );

            bindingContainer.Add(containerBindingVisualElement);
        }

        private void ToggleExpansion( ComponentPropertyBinding binding )
		{
            binding.expandView = !binding.expandView;
            StoreAndUpdateView();
        }

        private void ToggleExpansion(ContainerPropertyBinding binding)
        {
            binding.expandView = !binding.expandView;
            StoreAndUpdateView();
        }

        private void MoveBindingUp( ComponentPropertyBinding binding )
        {
            var elementIndex = Array.IndexOf( view.componentPropertyBindings, binding );

            if ( elementIndex >= 1 )
            {
                var predecessor = view.componentPropertyBindings[elementIndex - 1];

                view.componentPropertyBindings =
                    view.componentPropertyBindings.Take( elementIndex - 1 )
                    .Append( binding )
                    .Append( predecessor )
                    .Concat( view.componentPropertyBindings.Skip( elementIndex + 1 ) )
                    .ToArray();
            }

            StoreAndUpdateView();
        }

        private void MoveBindingUp(ContainerPropertyBinding binding)
        {
            var elementIndex = Array.IndexOf(view.containerPropertyBindings, binding);

            if ( elementIndex >= 1 )
            {
                var predecessor = view.containerPropertyBindings[elementIndex - 1];

                view.containerPropertyBindings =
                    view.containerPropertyBindings.Take(elementIndex - 1)
                    .Append(binding)
                    .Append(predecessor)
                    .Concat(view.containerPropertyBindings.Skip(elementIndex + 1))
                    .ToArray();
            }

            StoreAndUpdateView();
        }

        private void MoveBindingDown( ComponentPropertyBinding binding )
        {
            var elementIndex = Array.IndexOf( view.componentPropertyBindings, binding );

            if ( elementIndex >= 0 && elementIndex < (view.componentPropertyBindings.Length - 1) )
            {
                var successor = view.componentPropertyBindings[elementIndex + 1];

                view.componentPropertyBindings =
                    view.componentPropertyBindings.Take( elementIndex )
                    .Append( successor )
                    .Append( binding )
                    .Concat( view.componentPropertyBindings.Skip( elementIndex + 2 ) )
                    .ToArray();
            }

            StoreAndUpdateView();
        }

        private void MoveBindingDown(ContainerPropertyBinding binding)
        {
            var elementIndex = Array.IndexOf(view.containerPropertyBindings, binding);

            if ( elementIndex >= 0 && elementIndex < (view.containerPropertyBindings.Length - 1) )
            {
                var successor = view.containerPropertyBindings[elementIndex + 1];

                view.containerPropertyBindings =
                    view.containerPropertyBindings.Take(elementIndex)
                    .Append(successor)
                    .Append(binding)
                    .Concat(view.containerPropertyBindings.Skip(elementIndex + 2))
                    .ToArray();
            }

            StoreAndUpdateView();
        }

        private void RemoveBinding( ComponentPropertyBinding binding )
		{
			view.componentPropertyBindings =
				view.componentPropertyBindings.Where( x => x != binding )
				.ToArray();

			StoreAndUpdateView();
		}

        private void RemoveBinding( ContainerPropertyBinding binding )
        {
            view.containerPropertyBindings =
                view.containerPropertyBindings.Where(x => x != binding)
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
				&& x.InheritsOrImplements( typeof( INotifyDataSourceChanged ) );

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

        private void HandleAddContainerPropertyBinding()
        {
            view.containerPropertyBindings = view.containerPropertyBindings
                .Append(new ContainerPropertyBinding() { DataSource = view.DataSource })
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