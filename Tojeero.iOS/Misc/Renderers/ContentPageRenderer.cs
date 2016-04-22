using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Linq;
using System.Collections.Generic;

[assembly:ExportRenderer(typeof(ContentPage), typeof(Tojeero.iOS.Renderers.ContentPageRenderer))]
namespace Tojeero.iOS.Renderers
{
	public class ContentPageRenderer : PageRenderer
	{
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			var itemsInfo = (this.Element as ContentPage).ToolbarItems;
			if (itemsInfo == null || this.NavigationController == null)
				return;
			var navigationItem = this.NavigationController.TopViewController.NavigationItem;
			var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[]{ }).ToList();
			var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[]{ }).ToList();

			List<UIBarButtonItem> itemsToMove = new List<UIBarButtonItem>();
			rightNativeButtons.ForEach(nativeItem =>
				{
					var info = GetButtonInfo(itemsInfo, nativeItem.Title);
					//HACK we put special priority for the buttons that we want to appear on the left of toolbar
					if(info != null && info.Priority >= 10)
					{
						itemsToMove.Add(nativeItem);
					}
				});

			foreach (var item in itemsToMove)
			{
				rightNativeButtons.Remove(item);
				leftNativeButtons.Add(item);	
			}
			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

		private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
				return null;

			return items.ToList().Where(itemData => name.Equals(itemData.Name)).FirstOrDefault();
		}
	}
}

