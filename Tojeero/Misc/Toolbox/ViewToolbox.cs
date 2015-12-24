using System;
using Xamarin.Forms;

namespace Tojeero.Forms.Toolbox
{
	public static class ViewToolbox
	{
		public static T FindParentPage<T>(this VisualElement view) where T : Page
		{
			if (view == null)
				return null;

			VisualElement parent = view.ParentView;
			while (parent != null && !(parent is T))
			{
				parent = parent.ParentView;
			}
			return parent as T;
		}
	}
}

