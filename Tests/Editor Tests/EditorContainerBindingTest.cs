using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace JH.DataBinding.Editor.Tests
{
  public class EditorContainerBindingTest
  {
    class ItemDataSource : DataSourceBase<ItemDataSource> { }

    class ContainerDataSource : DataSourceBase<ContainerDataSource>
    {
      public IEnumerable<ItemDataSource> Items { get; set; }
    }

    [UnityTest]
    public IEnumerator PreoccupiedContainer_WhenDatasourceWithTwoItemsAssigned_HasTwoChildrenInNextFrame()
    {
      var elementTemplateGameObject = new GameObject("ElementTemplate");
      var elementTemplateView = elementTemplateGameObject.AddComponent<View>();

      var containerGameObject = new GameObject("Container");
      var containerView = containerGameObject.AddComponent<View>();

      var preoccupiedItem = new GameObject("PreoccupiedItem");
      preoccupiedItem.AddComponent<View>();
      preoccupiedItem.transform.SetParent(containerGameObject.transform);

      containerView.containerPropertyBindings = new ContainerPropertyBinding[]
      {
        new ContainerPropertyBinding()
        {
          SourcePath = nameof(ContainerDataSource.Items),
          TargetContainer = containerGameObject.transform,
          ElementTemplate = elementTemplateView,
        },
      };

      var dataSource = new ContainerDataSource()
      {
        Items = new ItemDataSource[] { new ItemDataSource(), new ItemDataSource() },
      };

      containerView.DataSource = dataSource;

      yield return null;

      Assert.AreEqual(2, containerGameObject.transform.childCount);
    }
  }
}
