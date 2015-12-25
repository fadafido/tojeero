using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using Android.Widget;

[assembly:ExportRenderer(typeof(SearchBarEx), typeof(Tojeero.Droid.Renderers.SearchBarExRenderer))]

namespace Tojeero.Droid.Renderers
{
	public class SearchBarExRenderer : SearchBarRenderer
	{
		#region Private fields

		private ShapeDrawable _background = null;
		private ShapeDrawable _searchBackground = null;

		#endregion
		
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
			if (_background == null)
			{
				var shape = new RoundRectShape(new float[]{ 10, 10, 10, 10, 10, 10, 10, 10 }, null, null);
				_background = new ShapeDrawable(shape);
				_background.Paint.Color = global::Android.Graphics.Color.White;
			}	
			this.Control.Background = _background;
		}

		private void updateSearchBackground()
		{
			if (_searchBackground == null)
			{
				var shape = new RectShape();
				_searchBackground = new ShapeDrawable(shape);
				_searchBackground.Paint.Color = global::Android.Graphics.Color.White;
			}	

			int searchPlateId = this.Control.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
			var searchPlateView = this.Control.FindViewById(searchPlateId);
			if (searchPlateView != null) {
				searchPlateView.Background = _searchBackground;
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

