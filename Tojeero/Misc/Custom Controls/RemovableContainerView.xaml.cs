using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tojeero.Forms
{
	public partial class RemovableContainerView : Grid
	{
		#region Constructors

		public RemovableContainerView()
			:base()
		{
			InitializeComponent();
			setupRootContent();
		}

		#endregion

		#region Properties

		private View _rootContent;

		public View RootContent
		{
			get
			{
				return _rootContent;
			}
			set
			{
				if (_rootContent != value)
				{
					_rootContent = value;
					setupRootContent();
				}
			}
		}

		#endregion

		#region Utility methods

		void setupRootContent()
		{
			if (this.rootContainer != null)
			{
				this.rootContainer.Children.Clear();
				if(this.RootContent != null)
					this.rootContainer.Children.Add(this.RootContent);
			}
		}

		#endregion
	}
}

