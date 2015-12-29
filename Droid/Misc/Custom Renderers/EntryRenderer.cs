using System;
using Xamarin.Forms;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using System.Linq;

[assembly:ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(Tojeero.Droid.Renderers.EntryRenderer))]
namespace Tojeero.Droid.Renderers
{
	public class EntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
	{
		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Entry> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;
			updateBackground();
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

