using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

[assembly:ExportRenderer(typeof(Tojeero.Forms.Picker), typeof(Tojeero.Droid.Renderers.PickerRenderer))]
namespace Tojeero.Droid.Renderers
{	
	public class PickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
	{
		#region Private fields

		private ShapeDrawable _background = null;

		#endregion

		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Tojeero.Forms.Picker)this.Element;
			this.Control.SetTextColor(element.TextColor.ToAndroid());
			updateBackground();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;
			
			if (e.PropertyName == Tojeero.Forms.Picker.TextColorProperty.PropertyName)
			{
				var element = (Tojeero.Forms.Picker)this.Element;
				this.Control.SetTextColor(element.TextColor.ToAndroid());
			}
		}

		#endregion

		#region Utility methods

		private void updateBackground()
		{
			if (_background == null)
			{
				var shape = new RoundRectShape(new float[]{ 10, 10, 10, 10, 10, 10, 10, 10 }, null, null);
				_background = new ShapeDrawable(shape);
				_background.Paint.Color = global::Android.Graphics.Color.White;
			}	
			this.Control.Background = _background;
		}

		#endregion
	}
}

