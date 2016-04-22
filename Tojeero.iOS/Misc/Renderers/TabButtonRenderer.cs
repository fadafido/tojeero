using System;
using Xamarin.Forms;
using Tojeero.Forms;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Tojeero.Forms.Controls;

[assembly:ExportRenderer(typeof(TabButton), typeof(Tojeero.iOS.Renderers.TabButtonRenderer))]

namespace Tojeero.iOS.Renderers
{
	public enum RoundedCorner
	{
		TopLeft = 1,
		TopRight = 1 << 1,
		BottomRight = 1 << 2,
		BottomLeft = 1 << 3
	}

	public class TabButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
	{
		private const float _cornerRadius = 20;
		private UIImage _selectedImage = null;
		private UIImage _deselectedImage = null;

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;
			updateBackground();
		}
			
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (this.Control == null || this.Element == null)
				return;
			if (e.PropertyName == TabButton.IsSelectedProperty.PropertyName)
			{
				updateBackground();
			}
		}

		private void updateBackground()
		{
			var button = this.Element as TabButton;
			if (_selectedImage == null)
			{
				var image = UIImage.FromFile("selectedTab.png");
				_selectedImage = image.StretchableImage(10, 0);
				this.Control.SetBackgroundImage(_selectedImage, UIControlState.Selected);
			}
			if (_deselectedImage == null)
			{
				var image = UIImage.FromFile("deselectedTab.png");
				_deselectedImage = image.StretchableImage(10, 0);
				this.Control.SetBackgroundImage(_deselectedImage, UIControlState.Normal);
			}
			this.Control.Selected = button.IsSelected;
		}
			

		static void addPath(CGContext context, CGRect rect, float radius, RoundedCorner cornerMask)
		{
			context.MoveTo(rect.X, rect.Y + radius);
			context.AddLineToPoint(rect.X, rect.Y + rect.Height - radius);
			if (((int)cornerMask & (int)RoundedCorner.BottomLeft) != 0) {
				context.AddArc(rect.X + radius, rect.Y + rect.Height - radius, 
					radius, (nfloat)Math.PI, (nfloat)Math.PI / 2, true);
			}
			else {
				context.AddLineToPoint(rect.X, rect.Y + rect.Height);
				context.AddLineToPoint(rect.X + radius, rect.Y + rect.Height);
			}

			context.AddLineToPoint(rect.X + rect.Width - radius, 
				rect.Y + rect.Height);

			if (((int)cornerMask & (int)RoundedCorner.BottomRight) != 0) {
				context.AddArc(rect.X + rect.Width - radius, 
					rect.Y + rect.Height - radius, radius, (nfloat)Math.PI / 2, 0.0f, true);
			}
			else {
				context.AddLineToPoint(rect.X + rect.Width, rect.Y + rect.Height);
				context.AddLineToPoint(rect.X + rect.Width, rect.Y + rect.Height - radius);
			}

			context.AddLineToPoint(rect.X + rect.Width, rect.Y + radius);

			if (((int)cornerMask & (int)RoundedCorner.TopRight) != 0) {
				context.AddArc(rect.X + rect.Width - radius, rect.Y + radius, 
					radius, 0, -(nfloat)Math.PI / 2, true);
			}
			else {
				context.AddLineToPoint(rect.X + rect.Width, rect.Y);
				context.AddLineToPoint(rect.X + rect.Width - radius, rect.Y);
			}

			context.AddLineToPoint(rect.X + radius, rect.Y);

			if (((int)cornerMask & (int)RoundedCorner.TopLeft) != 0) {
				context.AddArc(rect.X + radius, rect.Y + radius, radius, 
					-(nfloat)Math.PI / 2, (nfloat)Math.PI, true);
			}
			else {
				context.AddLineToPoint(rect.X, rect.Y);
				context.AddLineToPoint(rect.X, rect.Y + radius);
			}

			context.ClosePath();
		}
	}
}

