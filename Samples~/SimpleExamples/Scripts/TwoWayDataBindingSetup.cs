using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class TwoWayDataBindingSetup : MonoBehaviour
    {
        public ViewBehaviour twoWayDataBindingView;
        public Sprite OneSprite;
        public Sprite OtherSprite;

        public void Start()
        {
            var viewModel = new TwoWayDataBindingViewModel()
            {
                TwoWayProperty = 1.0,
                DropDownOptions = new List<Dropdown.OptionData>()
                {
                    new Dropdown.OptionData() { image = OneSprite, text = "One Option" },
                    new Dropdown.OptionData() { image = OtherSprite, text = "Other Option" },
                }
            };

            viewModel.ReactingToViewSelectionChangedDoneRight = new Action<int>( ( x ) =>
            {
                viewModel.ExplicitSelectionMessage = $"Selected {viewModel.DropDownOptions[x].text} the right way!";
                viewModel.NotifyViewModelChanged();
            } );

            twoWayDataBindingView.DataSource = viewModel;
        }
    }
}