using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding
{
    public class ButtonCommandAdapter : IBindingAdapter<ICommand>
    {
        public ButtonCommandAdapter( Button buttonToBindIn )
        {
            buttonToBind = buttonToBindIn;
        }

        ICommand IBindingAdapter<ICommand>.Value
        {
            set
            {
                buttonToBind.onClick.RemoveAllListeners();

                if (value != null)
                {
                    buttonToBind.interactable = value.CanExecute();
                    buttonToBind.onClick.AddListener( () => value.Execute() );
                }
                else
                {
                    buttonToBind.interactable = false;
                }
            }
        }

        private readonly Button buttonToBind;
    }
}
