﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public partial class TagListCell : ViewCell
	{
		public TagListCell()
		{
			InitializeComponent();
		}

		protected override void OnBindingContextChanged()
		{
			//If I don't set this to false manually when binding context changes, the checkmark is being visible for a short second
//			checkmark.IsVisible = false;
			base.OnBindingContextChanged();
		}
	}
}

