using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tojeero.Forms
{
	public partial class TagControl : Grid
	{
		public Action<TagControl> DeleteTagAction { get; set; }

		public TagControl()
		{
			InitializeComponent();
		}

		void deleteTagTapped(object sender, EventArgs args)
		{
			if (DeleteTagAction != null)
				DeleteTagAction(this);
		}
	}
}

