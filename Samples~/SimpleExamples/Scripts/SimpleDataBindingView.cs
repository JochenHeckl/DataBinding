using UnityEngine;

namespace de.JochenHeckl.Unity.DataBinding.Example
{
    public class SimpleDataBindingView : ViewBehaviour
    {
		public Sprite sprite;

        protected void Start()
        {
            DataSource = new SimpleDataBindingViewModel()
			{
                SomeBindableText = "This is bound text!",
                SomeBindableTextColor = Color.red,
				SomeBindableImage = sprite,

                SomeBindableView = new PrefabViewModel()
                {
                    PrefabViewText = "This is text on a bound prefab"
                }
			};
        }
    }
}
