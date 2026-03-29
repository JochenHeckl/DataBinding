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
    public IEnumerator PreoccupiedContainerWithOneItem_WhenDatasourceWithTwoItemsAssigned_HasTwoChildrenInNextFrame()
    {
      var elementTemplate = new GameObject("Element Template");
      var elementTemplateView = elementTemplate.AddComponent<View>();

      var container = new GameObject("Container");
      var containerView = container.AddComponent<View>();

      var initialItem = new GameObject("Initial Item");
      initialItem.AddComponent<View>();
      initialItem.transform.SetParent(container.transform);

      containerView.containerPropertyBindings = new ContainerPropertyBinding[]
      {
        new ContainerPropertyBinding()
        {
          SourcePath = nameof(ContainerDataSource.Items),
          TargetContainer = container.transform,
          ElementTemplate = elementTemplateView,
        },
      };

      Assert.AreEqual(1, container.transform.childCount);

      var dataSource = new ContainerDataSource() { Items = new ItemDataSource[] { new(), new() } };
      containerView.DataSource = dataSource;

      Assert.AreEqual(2, container.transform.childCount);

      yield return null;
    }
  }
}
