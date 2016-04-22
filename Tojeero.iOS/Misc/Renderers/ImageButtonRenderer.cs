using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Tojeero.Forms;
using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Tojeero.iOS.Renderers
{
	public partial class ImageButtonRenderer : ViewRenderer<ImageButton, UIButton>
	{
		#region Parent override

		protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				if (base.Control == null)
				{
					base.SetNativeControl(new UIButton(UIButtonType.Custom));
					base.Control.TouchUpInside += new EventHandler(this.buttonTapped);
				}
				await this.UpdateBackgroundImage();
				await this.UpdateImage();
				await this.UpdateSelectedImage();
				this.UpdateIsEnabled();
				this.UpdateIsSelected();
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == ImageButton.BackgroundImageProperty.PropertyName)
			{
				await this.UpdateBackgroundImage();
			}
			if (e.PropertyName == ImageButton.ImageProperty.PropertyName)
			{
				await this.UpdateImage();
			}
			if (e.PropertyName == ImageButton.SelectedImageProperty.PropertyName)
			{
				await this.UpdateSelectedImage();
			}
			if (e.PropertyName == ImageButton.IsSelectedProperty.PropertyName)
			{
				this.UpdateIsSelected();
			}
			if (e.PropertyName == ImageButton.IsEnabledProperty.PropertyName)
			{
				this.UpdateIsEnabled();
			}
		}

		#endregion

		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if (base.Control != null)
			{
				base.Control.TouchUpInside -= new EventHandler(this.buttonTapped);
			}
			base.Dispose(disposing);
		}

		#endregion
			
		#region Events

		private void buttonTapped(object sender, EventArgs eventArgs)
		{
			if (this.Element != null && this.Element.Command != null)
			{
				this.Element.Command.Execute(null);
			}
		}

		#endregion


		#region Utility methods

		private async Task UpdateBackgroundImage()
		{
			var btn = this.Element;
			await setImageAsync(btn.BackgroundImage, UIControlState.Normal, true);
		}

		private async Task UpdateImage()
		{
			var btn = this.Element;
			await setImageAsync(btn.Image, UIControlState.Normal);
		}

		private async Task UpdateSelectedImage()
		{
			var btn = this.Element;
			await setImageAsync(btn.SelectedImage, UIControlState.Selected);
		}

		private void UpdateIsEnabled()
		{
			this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
		}

		private void UpdateIsSelected()
		{
			this.Control.Selected = this.Element.IsSelected;
		}

		private async Task setImageAsync(ImageSource source, UIControlState state = UIControlState.Normal, bool isBackground = false)
		{
			UIImage target = null;
			var handler = GetHandler(source);
			if (handler != null)
			{
				using (var image = await handler.LoadImageAsync(source))
				{
					target = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
				}
			}
				
			if(!isBackground)
				this.Control.SetImage(target, state);
			else
				this.Control.SetBackgroundImage(target, state);
		}

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }
        #endregion
    }
}

