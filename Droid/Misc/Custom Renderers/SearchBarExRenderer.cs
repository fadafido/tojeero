using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using Android.Widget;
using System.Linq;

[assembly:ExportRenderer(typeof(SearchBarEx), typeof(Tojeero.Droid.Renderers.SearchBarExRenderer))]

namespace Tojeero.Droid.Renderers
{
	public class SearchBarExRenderer : SearchBarRenderer
	{
		#region Parent Override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<SearchBar> e)
		{
			base.OnElementChanged(e);
			if (this.Control == null || this.Element == null)
				return;
			
			updateBackground();
			updateSearchBackground();
		}
			
		#endregion

		#region Utility methods

		private void updateBackground()
		{
			var shape = new RoundRectShape(Enumerable.Repeat(10f, 8).ToArray(), null, null);
			var background = new ShapeDrawable(shape);
			background.Paint.Color = global::Android.Graphics.Color.White;
			this.Control.Background = background;
		}

		private void updateSearchBackground()
		{
			var shape = new RectShape();
			var searchBackground = new ShapeDrawable(shape);
			searchBackground.Paint.Color = global::Android.Graphics.Color.White;

			int searchPlateId = this.Control.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
			var searchPlateView = this.Control.FindViewById(searchPlateId);
			if (searchPlateView != null) {
				searchPlateView.Background = searchBackground;
			}

			int textId = this.Control.Context.Resources.GetIdentifier("android:id/search_src_text", null, null);
			var textView = (Android.Widget.TextView) this.Control.FindViewById(textId);
			if (textView != null)
			{
				textView.TextSize = 14;
				textView.LayoutParameters.Height = LayoutParams.MatchParent;
			}
		}

		#endregion
	}
}

