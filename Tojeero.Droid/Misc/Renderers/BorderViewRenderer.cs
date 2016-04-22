using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System.ComponentModel;
using Android.Graphics;
using System.Linq;
using Android.Graphics.Drawables.Shapes;
using Tojeero.Forms.Controls;


[assembly:ExportRenderer(typeof(BorderView), typeof(Tojeero.Droid.Renderers.BorderViewRenderer))]

namespace Tojeero.Droid.Renderers
{
	public class BorderViewRenderer : VisualElementRenderer<BorderView>
	{
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.Background.Dispose();
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BorderView> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null && e.OldElement == null)
			{
				this.UpdateBackground();
			}
		}

		private void UpdateBackground()
		{		
			var strokeWidth = Xamarin.Forms.Forms.Context.ToPixels(this.Element.BorderWidth);
			var radius = Xamarin.Forms.Forms.Context.ToPixels(this.Element.Radius);
			GradientDrawable background = new GradientDrawable();
			background.SetColor(this.Element.BackgroundColor.ToAndroid());
			background.SetCornerRadius(radius);
			background.SetStroke((int)strokeWidth, this.Element.BorderColor.ToAndroid());
			this.Background = background;
		}
	}
}

