using System;
using Xamarin.Forms;

namespace Tojeero.Forms.Toolbox
{
	public static class ViewToolbox
	{
		public static Page FindParentPage(this VisualElement view)
		{
			if (view == null)
				return null;

			VisualElement parent = view.ParentView;
			while (parent != null && !(parent is Page))
			{
				parent = parent.ParentView;
			}
			return parent as Page;
		}
	}
}

