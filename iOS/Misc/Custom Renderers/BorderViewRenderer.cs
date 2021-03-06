﻿using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;


[assembly:ExportRenderer(typeof(BorderView), typeof(Tojeero.iOS.Renderers.BorderViewRenderer))]

namespace Tojeero.iOS.Renderers
{
	public class BorderViewRenderer : ViewRenderer
	{
		#region Parent override

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				if (base.Control == null)
				{
					base.SetNativeControl(new UIView());
				}
				updateBorderWidth();
				updateBorderColor();
				updateBorderRadius();
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (this.Element == null || this.Control == null)
				return;
			if (e.PropertyName == BorderView.BorderWidthProperty.PropertyName)
			{
				updateBorderWidth();
			}
			else if (e.PropertyName == BorderView.BorderColorProperty.PropertyName)
			{
				updateBorderColor();
			}
			else if (e.PropertyName == BorderView.RadiusProperty.PropertyName)
			{
				updateBorderRadius();
			}
		}

		#endregion

		#region Utility methods

		private void updateBorderWidth()
		{
			var borderView = this.Element as BorderView;
			this.Control.Layer.BorderWidth = borderView.BorderWidth;
		}

		private void updateBorderColor()
		{
			var borderView = this.Element as BorderView;
			this.Control.Layer.BorderColor = borderView.BorderColor.ToUIColor().CGColor;
		}

		private void updateBorderRadius()
		{
			var borderView = this.Element as BorderView;
			this.Control.Layer.CornerRadius = (nfloat)borderView.Radius;
		}

		#endregion
	}
}

