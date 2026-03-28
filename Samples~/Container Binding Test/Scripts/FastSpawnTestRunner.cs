using System.Linq;
using UnityEngine;

namespace JH.DataBinding.Examples.ContainerBindingTest
{
    public class FastSpawnTestRunner : MonoBehaviour
    {
        public View containerView;

        [SerializeField]
        private int count = 10;

        [SerializeField]
        private float maxDistance = 5f;

        [ContextMenu(nameof(Start))]
        private void Start()
        {
            containerView.DataSource = new ContainerViewDataSource()
            {
                Items = Enumerable.Range(0, Random.Range(1, count))
                .Select(_ => MakeRandomItem()).ToArray()
            };
            }

        private ItemDataSource MakeRandomItem()
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
