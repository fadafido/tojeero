using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class HomePage : ContentPage
	{
		public HomePage()
		{
			InitializeComponent();
			Tools.Logger.Log(new InvalidOperationException(), "Sample error message.", LoggingLevel.Error, true);
		}
	}
}

