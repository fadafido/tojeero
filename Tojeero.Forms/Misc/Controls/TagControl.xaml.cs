using System;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
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

