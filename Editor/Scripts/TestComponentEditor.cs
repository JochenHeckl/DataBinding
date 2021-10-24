using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace de.JochenHeckl.Unity.DataBinding.Editor
{
	[CustomEditor( typeof( TestComponent ), true )]
	public class TestComponentEditor : UnityEditor.Editor
	{
		private const string visualTreeAssetFile = "Packages/de.jochenheckl.unity.dataBinding/Editor/UI/TestComponentEditor.uxml";

		private TestComponent testComponent;
		private VisualTreeAsset sharedViewVisualTreeAsset;
		private VisualElement editorRootElement;

		public void OnEnable()
		{
			testComponent = target as TestComponent;

			if ( sharedViewVisualTreeAsset == null )
			{
				sharedViewVisualTreeAsset = AssetDatabase
					.LoadAssetAtPath<VisualTreeAsset>( visualTreeAssetFile );
			}
		}

		public override VisualElement CreateInspectorGUI()
		{
			editorRootElement = new VisualElement();
			editorRootElement.Clear();

			sharedViewVisualTreeAsset?.CloneTree( editorRootElement );

			return editorRootElement;
		}
	}
}