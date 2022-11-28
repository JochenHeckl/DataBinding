using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace de.JochenHeckl.Unity.DataBinding.Tests
{
    internal class RuntimeContainerBindingsTest
    {
        //class TestElementTemplateDataSource : DataSourceBase<TestElementTemplateDataSource>
        //{
        //    public Vector3 Position { get; set; }
        //}

        //class TestContainerBindingsDataSource : DataSourceBase<TestContainerBindingsDataSource>
        //{
        //    public IEnumerable<TestElementTemplateDataSource> Positions { get; set; }
        //}

        //[Test]
        //public void TestContainerBindings()
        //{
        //    var testDataSource = new TestContainerBindingsDataSource()
        //    {
        //        Positions = new TestElementTemplateDataSource[]
        //        {
        //            new TestElementTemplateDataSource() { Position = new Vector3(1f, 0f, 0f) },
        //            new TestElementTemplateDataSource() { Position = new Vector3(2f, 0f, 0f) }
        //        }
        //    };

        //    var elementTemplateGameObject = new GameObject("ElementTemplate");
        //    var elementTemplateView = elementTemplateGameObject.AddComponent<View>();

        //    var containerGameObject = new GameObject("ContainerTestObject");
        //    var containerView = containerGameObject.AddComponent<View>();

        //    containerView.containerPropertyBindings = new ContainerPropertyBinding[]
        //    {
        //        new ContainerPropertyBinding()
        //        {
        //            SourcePath = nameof(testDataSource.Positions),
        //            TargetContainer = containerGameObject.transform,
        //            ElementTemplate = elementTemplateView
        //        }
        //    };

        //    containerView.DataSource = testDataSource;

        //    Assert.AreEqual(containerGameObject.transform.childCount, 2);
        //}

        //// A UnityTest behaves like a coroutine in PlayMode
        //// and allows you to yield null to skip a frame in EditMode
        //[UnityTest]
        //public IEnumerator PlayModeSampleTestWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // yield to skip a frame
        //    yield return null;
        //}
    }
}
