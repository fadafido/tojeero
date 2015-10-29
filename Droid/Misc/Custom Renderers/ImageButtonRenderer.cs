using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Tojeero.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Graphics;
using Tojeero.Forms.Renderers;
using Android.Views;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Tojeero.Forms.Renderers
{
	public partial class ImageButtonRenderer :  ViewRenderer<ImageButton, Android.Widget.ImageButton>
	{
		#region Private fields and properties

		private Bitmap _image;
		private Bitmap _selectedImage;

		#endregion

		#region Parent override

		protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{
				if (base.Control == null)
				{
					Android.Widget.ImageButton button = new Android.Widget.ImageButton(base.Context);
					button.Touch += buttonTouched;
					button.SetBackgroundColor(Android.Graphics.Color.Transparent);
					button.Tag = this;
					base.SetNativeControl(button);
				}
			}

			await this.UpdateImage();
			this.UpdateIsEnabled();
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Console.WriteLine("PROPERTY CHANGED: " + e.PropertyName);
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == ImageButton.ImageProperty.PropertyName)
			{
				_image = null;
			}

			if (e.PropertyName == ImageButton.SelectedImageProperty.PropertyName)
			{
				_selectedImage = null;
			}

			if (e.PropertyName == ImageButton.ImageProperty.PropertyName ||
			    e.PropertyName == ImageButton.SelectedImageProperty.PropertyName ||
			    e.PropertyName == ImageButton.IsSelectedProperty.PropertyName)
			{
				await this.UpdateImage();
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
			if (disposing && this.Control != null)
			{
				this.Control.Touch -= buttonTouched;
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Events

		void buttonTouched (object sender, TouchEventArgs e)
		{
			if (e.Event.Action == MotionEventActions.Down)
				this.Control.Alpha = (float)this.Element.Opacity / 2;
			else if (e.Event.Action == MotionEventActions.Up)
			{
				this.Control.Alpha = (float)this.Element.Opacity;
				if (this.Element != null && this.Element.Command != null)
					this.Element.Command.Execute(null);
			}
		}

		#endregion

		#region Utility methods

		private async Task UpdateImage()
		{
			var btn = this.Element;
			Bitmap target = null;
			if (!this.Element.IsSelected)
			{
				target = _image = _image != null ? _image : await getBitmapAsync(this.Element.Image);
			}
			else
			{
				target = _selectedImage = _selectedImage != null ? _selectedImage : await getBitmapAsync(this.Element.SelectedImage);
			}
			this.Control.SetImageBitmap(target);
		}

		private void UpdateIsEnabled()
		{
			this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
		}

		/// <summary>
		/// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
		/// </summary>
		/// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
		/// <returns>A loaded <see cref="Bitmap"/>.</returns>
		private async Task<Bitmap> getBitmapAsync(ImageSource source)
		{			
			var handler = GetHandler(source);
			var returnValue = (Bitmap)null;

			if (handler != null)
				returnValue = await handler.LoadImageAsync(source, this.Context);

			return returnValue;
		}

		#endregion
	}
}

