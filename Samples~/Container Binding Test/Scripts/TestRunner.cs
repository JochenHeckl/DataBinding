using System.Linq;

using UnityEngine;

namespace JH.DataBinding.Examples.ContainerBindingTest
{
  public class TestRunner : MonoBehaviour
  {
    public View containerView;
    public int maxItemCount = 10;
    public int maxDistance = 5;

    public int UpdateIntervalSec = 3;

    private ContainerViewDataSource dataSource;
    private float nextUpdateSec = 0f;

    void Start()
    {
      dataSource = new();
      containerView.DataSource = dataSource;

      RandomizeItems();
    }

    void FixedUpdate()
    {
        if( Time.time >= nextUpdateSec )
        {
          nextUpdateSec = Time.time + UpdateIntervalSec;
          
          RandomizeItems();
        }
    }

    private void RandomizeItems()
    {
      dataSource.NotifyChanges(PopulateItems);
    }

    private void PopulateItems(ContainerViewDataSource dataSource)
    {
      dataSource.Items = Enumerable.Range(
         0, Random.Range(1, maxItemCount))
         .Select(MakeRandomItem).ToArray();
    }

    private ItemDataSource MakeRandomItem(int arg1, int arg2)
    {
      return new ItemDataSource
      {
        Position = Random.insideUnitSphere * maxDistance,
        Rotation = Random.rotation,
        Scale = new Vector3(
              Random.Range(0.5f, 2.0f),
              Random.Range(0.5f, 2.0f),
              Random.Range(0.5f, 2.0f)
          ),
        Color = Random.ColorHSV(0.2f, 0.8f)
      };
    }
  }

}