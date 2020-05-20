using System.Linq;
using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    internal class AddTextItemCommand : ICommand
    {

        public AddTextItemCommand( ContainerDataBindingViewModel viewModelIn )
        {
            viewModel = viewModelIn;
        }

        public bool CanExecute()
        {
            return viewModel.TextItems.Count() < 6;    
        }

        public void Execute()
        {
            var itemToCopy = viewModel.TextItems.Skip( Random.Range( 0, viewModel.TextItems.Count() ) ).First();

            var newItem = new TextItemViewModel()
            {
                Text = itemToCopy.Text,
                Color = itemToCopy.Color
            };

            viewModel.TextItems = viewModel.TextItems.Union( Enumerable.Repeat<TextItemViewModel>( newItem, 1 ) ).ToArray();

            viewModel.NotifyViewModelChanged();
        }

        private ContainerDataBindingViewModel viewModel;
    }
}
