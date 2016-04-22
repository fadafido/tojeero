/*using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly:ExportRenderer(typeof(SelectableButton), typeof(Tojeero.iOS.Renderers.SelectableButtonRenderer))]

namespace Tojeero.iOS.Renderers
{
	public class SelectableButtonRenderer : ButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (this.Element == null)
				return;

			updateOpacity();
			updateImage();
		}	

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == Button.IsEnabledProperty.PropertyName)
			{
				updateOpacity();
			}
			else if (e.PropertyName == SelectableButton.IsSelectedProperty.PropertyName ||
				e.PropertyName == SelectableButton.SelectedImageProperty.PropertyName ||
				e.PropertyName == SelectableButton.DeselectedImageProperty.PropertyName)
			{
				updateImage();
			}
		}

		private void updateImage()
		{
			var button = (SelectableButton)this.Element;
			var imageSource = button.IsSelected ? button.SelectedImage : button.DeselectedImage;
			button.Image = imageSource;
		}

		private void updateOpacity()
		{
			//this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
		}
	}
}
*/

