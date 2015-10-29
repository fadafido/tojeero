using System;
using Xamarin.Forms.Platform.iOS;
using Tojeero.Forms;
using UIKit;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using CoreGraphics;
using Tojeero.Forms.Renderers;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Tojeero.Forms.Renderers
{
	public partial class ImageButtonRenderer : ViewRenderer<ImageButton, UIButton>
	{
		//
		// Methods
		//
		protected override void Dispose(bool disposing)
		{
			if (base.Control != null)
			{
				base.Control.TouchUpInside -= new EventHandler(this.OnButtonTouchUpInside);
			}
			base.Dispose(disposing);
		}

		private void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
		{
			if (this.Element != null && this.Element.Command != null)
			{
				this.Element.Command.Execute(null);
			}
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				if (base.Control == null)
				{
					base.SetNativeControl(new UIButton(UIButtonType.System));
					base.Control.TouchUpInside += new EventHandler(this.OnButtonTouchUpInside);
				}
				await this.UpdateImage();
				await this.UpdateSelectedImage();
				this.UpdateIsEnabled();
				this.UpdateIsSelected();
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
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
				return;
			}
		}
			
		private async Task UpdateImage()
		{
			var btn = this.Element;
			await setImageAsync(btn.Image, this.Control, UIControlState.Normal);
		}

		private async Task UpdateSelectedImage()
		{
			var btn = this.Element;
			await setImageAsync(btn.SelectedImage, this.Control, UIControlState.Selected);
		}

		private void UpdateIsEnabled()
		{
			this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
		}

		private void UpdateIsSelected()
		{
			this.Control.Selected = this.Element.IsSelected;
		}

		private async static Task setImageAsync(ImageSource source, UIButton targetButton, UIControlState state = UIControlState.Normal)
		{
			UIImage target = null;
			var handler = GetHandler(source);
			if (handler != null)
			{
				using (var image = await handler.LoadImageAsync(source))
				{
					target = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
				};
			}
				
			targetButton.SetImage(target, state);
		}
	}
}

