using System;
using Tojeero.Forms;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(TabButton), typeof(Tojeero.Droid.Renderers.TabButtonRenderer))]

namespace Tojeero.Droid.Renderers
{
	public class TabButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
	{
		#region Private fields

		private ShapeDrawable _selectedBackground;
		private ShapeDrawable _deselectedBackground;

		#endregion

		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged(e);
			if (this.Control == null || this.Element == null)
				return;
			updateBackground();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (this.Control == null || this.Element == null)
				return;
			if (e.PropertyName == TabButton.IsSelectedProperty.PropertyName)
			{
				updateBackground();
			}
		}

		#endregion


		#region Utility methods

		private void updateBackground()
		{
			var tabButton = this.Element as TabButton;
			if (tabButton.IsSelected)
			{
				if (_selectedBackground == null)
				{
					var shape = new RoundRectShape(new float[]{ 10, 10, 10, 10, 0, 0, 0, 0 }, null, null);
					_selectedBackground = new ShapeDrawable(shape);
					_selectedBackground.Paint.Color = Colors.Secondary.ToAndroid();
				}	
				this.Control.Background = _selectedBackground;
			}
			else
			{
				if (_deselectedBackground == null)
				{
					var shape = new RoundRectShape(new float[]{ 10, 10, 10, 10, 0, 0, 0, 0 }, null, null);
					_deselectedBackground = new ShapeDrawable(shape);
					_deselectedBackground.Paint.Color = Colors.Main.ToAndroid();
				}	
				this.Control.Background = _deselectedBackground;
			}
		}

		#endregion
	}
}

