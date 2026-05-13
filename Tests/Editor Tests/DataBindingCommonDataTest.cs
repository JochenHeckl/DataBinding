using System.Reflection;
using NUnit.Framework;

namespace JH.DataBinding.Editor.Tests
{
  public class DataBindingCommonDataTest
  {
    class DataSourceWithNoProperties : DataSourceBase<DataSourceWithNoProperties> { }

    class DataSourceWithIntProperty : DataSourceBase<DataSourceWithIntProperty>
    {
      public int IntProperty { get; set; }      
    }

    class DataSourceWithStringProperty : DataSourceBase<DataSourceWithStringProperty>
    {
      public string Name { get; set; }
    }

    [Test]
    public void DetermineComponentPropertyBindingState_WhenDataSourceTypeIsNull_ReturnsMissingDataSourceAssignment()
    {
      var binding = new ComponentPropertyBinding();

      var (state, properties) = DataBindingCommonData.DetermineComponentPropertyBindingState(
        binding,
        null
      );

      Assert.AreEqual(ComponentPropertyBindingState.MissingDataSourceAssignment, state);
      Assert.AreEqual(0, properties.Length);
    }

    [Test]
    public void DetermineContainerPropertyBindingState_WhenDataSourceTypeIsNull_ReturnsMissingDataSourceAssignment()
    {
      var binding = new ContainerPropertyBinding();

      var (state, properties) = DataBindingCommonData.DetermineContainerPropertyBindingState(
        binding,
        null
      );

      Assert.AreEqual(ContainerPropertyBindingState.MissingDataSourceAssignment, state);
      Assert.AreEqual(0, properties.Length);
    }

    [Test]
    public void DetermineComponentPropertyBindingState_WhenDataSourceTypeHasNoProperties_ReturnsNoBindableProperties()
    {
      var binding = new ComponentPropertyBinding();

      var (state, properties) = DataBindingCommonData.DetermineComponentPropertyBindingState(
        binding,
        typeof(DataSourceWithNoProperties)
      );

      Assert.AreEqual(ComponentPropertyBindingState.NoBindableProperties, state);
      Assert.AreEqual(0, properties.Length);
    }

    [Test]
    public void DetermineContainerPropertyBindingState_WhenDataSourceTypeHasNoEnumerableProperties_ReturnsNoBindableProperties()
    {
      var binding = new ContainerPropertyBinding();

      var (state, properties) = DataBindingCommonData.DetermineContainerPropertyBindingState(
        binding,
        typeof(DataSourceWithIntProperty)
      );

      Assert.AreEqual(ContainerPropertyBindingState.NoBindableProperties, state);
      Assert.AreEqual(0, properties.Length);
    }

    [Test]
    public void DetermineComponentPropertyBindingState_WhenSourcePathIsNotBound_ReturnsSourceUnbound()
    {
      var binding = new ComponentPropertyBinding() { SourcePath = "NonExistentProperty" };

      var (state, properties) = DataBindingCommonData.DetermineComponentPropertyBindingState(
        binding,
        typeof(DataSourceWithStringProperty)
      );

      Assert.AreEqual(ComponentPropertyBindingState.SourceUnbound, state);
      Assert.Greater(properties.Length, 0);
    }
  }
}
