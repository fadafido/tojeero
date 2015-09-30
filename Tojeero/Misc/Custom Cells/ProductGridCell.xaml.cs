using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Tojeero.Forms
{
	public partial class ProductGridCell : FastGridCell
	{
		public ProductGridCell()
		{
			
		}
		#region implemented abstract members of FastGridCell

		protected override void InitializeCell()
		{
			InitializeComponent();
		}

		protected override void SetupCell(bool isRecycled)
		{
			
		}

		#endregion
		
	}
}

