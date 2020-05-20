using UnityEngine;
using System.Linq;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class ContainerDataBindingSetup : MonoBehaviour
    {
		public ViewBehaviour containerDataBindingView;

        private void Start()
        {
            viewModel = new ContainerDataBindingViewModel()
            {
                TextItems = new TextItemViewModel[]
				{
					new TextItemViewModel() { Text = "Red", Color = Color.red  },
					new TextItemViewModel() { Text = "Green", Color = Color.green },
					new TextItemViewModel() { Text = "Blue", Color = Color.blue  },
				}
			};

            viewModel.AddItemCommand = new AddTextItemCommand( viewModel );

            containerDataBindingView.DataSource = viewModel;
        }

        public void OnGUI()
        {
            if ( GUILayout.Button( "Add Random Text Item" ) )
            {
                viewModel.TextItems = viewModel.TextItems.Union( viewModel.TextItems.Skip( Random.Range( 0, viewModel.TextItems.Count() ) ) ).ToArray();
                viewModel.NotifyViewModelChanged();
            }
        }


        private ContainerDataBindingViewModel viewModel;
    }
}