using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Editor.Tests
{
    class EditorDataSourceTest
    {
        class TestElementTemplateDataSource : DataSourceBase<TestElementTemplateDataSource>
        {
            public Vector3 Position { get; set; }
        }

        class TestContainerBindingsDataSource : DataSourceBase<TestContainerBindingsDataSource>
        {
            public IEnumerable<TestElementTemplateDataSource> Positions { get; set; }
        }

        [Test]
        public void TestContainerBindings()
        {
            var testDataSource = new TestContainerBindingsDataSource()
            {
                Positions = new TestElementTemplateDataSource[]
                {
                    new TestElementTemplateDataSource() { Position = new Vector3(1f, 0f, 0f) },
                    new TestElementTemplateDataSource() { Position = new Vector3(2f, 0f, 0f) }
                }
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
                    ElementTemplate = elementTemplateView
                }
            };

            containerView.DataSource = testDataSource;
            Assert.AreEqual(2, containerGameObject.transform.childCount);

            containerView.DataSource = null;
            Assert.AreEqual(0, containerGameObject.transform.childCount);
        }
    }
}
