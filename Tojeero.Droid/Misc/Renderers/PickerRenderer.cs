using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using System.Linq;
using Picker = Tojeero.Forms.Controls.Picker;

[assembly:ExportRenderer(typeof(Picker), typeof(Tojeero.Droid.Renderers.PickerRenderer))]
namespace Tojeero.Droid.Renderers
{	
	public class PickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
	{
		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Picker)this.Element;
			this.Control.SetTextColor(element.TextColor.ToAndroid());
			updateBackground();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;
			
			if (e.PropertyName == Picker.TextColorProperty.PropertyName)
			{
				var element = (Picker)this.Element;
				this.Control.SetTextColor(element.TextColor.ToAndroid());
			}
		}

		#endregion

		#region Utility methods

		private void updateBackground()
		{
			var shape = new RoundRectShape(Enumerable.Repeat(5f, 8).ToArray(), null, null);
			var background = new ShapeDrawable(shape);
			background.Paint.Color = global::Android.Graphics.Color.White;
			this.Control.Background = background;
		}

		#endregion
	}
}

