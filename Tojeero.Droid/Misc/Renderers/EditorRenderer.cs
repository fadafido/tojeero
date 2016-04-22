using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables.Shapes;
using System.Linq;
using Android.Graphics.Drawables;

[assembly:ExportRenderer(typeof(Xamarin.Forms.Editor), typeof(Tojeero.Droid.Renderers.EditorRenderer))]
namespace Tojeero.Droid.Renderers
{
	public class EditorRenderer : Xamarin.Forms.Platform.Android.EditorRenderer
	{
		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Editor> e)
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

