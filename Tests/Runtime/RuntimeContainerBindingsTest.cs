using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace JH.DataBinding.Tests
{
  class PlayerDataSourceTest
  {
    class TestElementTemplateDataSource : DataSourceBase<TestElementTemplateDataSource>
    {
      public Vector3 Position { get; set; }
    }

    class TestContainerBindingsDataSource : DataSourceBase<TestContainerBindingsDataSource>
    {
      public IEnumerable<TestElementTemplateDataSource> Positions { get; set; }
    }

    [UnityTest]
    public IEnumerator TestContainerBindings()
    {
      var testDataSource = new TestContainerBindingsDataSource()
      {
        Positions = new TestElementTemplateDataSource[]
        {
          new TestElementTemplateDataSource() { Position = new Vector3(1f, 0f, 0f) },
          new TestElementTemplateDataSource() { Position = new Vector3(2f, 0f, 0f) },
        },
      };

      var elementTemplateGameObject = new GameObject("ElementTemplate");
      var elementTemplateView = elementTemplateGameObject.AddComponent<View>();

      var containerGameObject = new GameObject("ContainerTestObject");
      var containerView = containerGameObject.AddComponent<View>();

      containerView.containerPropertyBindings = new ContainerPropertyBinding[]
      {
        new ContainerPropertyBinding()
        {
          SourcePath = nameof(testDataSource.Positions),
          TargetContainer = containerGameObject.transform,
          ElementTemplate = elementTemplateView,
        },
      };

      containerView.DataSource = testDataSource;

      if (!Application.isBatchMode)
      {
        // WaitForEndOfFrame does throw in batch mode,
        // so can not use it as of now.
        yield return new WaitForEndOfFrame();
        Assert.AreEqual(2, containerGameObject.transform.childCount);
      }

      containerView.DataSource = null;

      if (!Application.isBatchMode)
      {
        // WaitForEndOfFrame does throw in batch mode,
        // so can not use it as of now.
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(0, containerGameObject.transform.childCount);
      }
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
          SourcePath = nameof(TestContainerBindingsDataSource.Positions),
          TargetContainer = container.transform,
          ElementTemplate = elementTemplateView,
        },
      };

      yield return null;

      Assert.AreEqual(1, container.transform.childCount);

      containerView.DataSource = new TestContainerBindingsDataSource()
      {
        Positions = new TestElementTemplateDataSource[]
        {
          new TestElementTemplateDataSource() { Position = new Vector3(1f, 0f, 0f) },
          new TestElementTemplateDataSource() { Position = new Vector3(2f, 0f, 0f) },
        },
      };

      yield return null;
      Assert.AreEqual(2, container.transform.childCount);
    }
  }
}
