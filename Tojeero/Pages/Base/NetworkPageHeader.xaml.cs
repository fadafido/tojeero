using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tojeero.Forms
{
	public partial class NetworkPageHeader : StackLayout
	{
		#region Constructors

		public NetworkPageHeader()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

		public Thickness NoNetworkPadding
		{
			get
			{
				return this.NoNetworkStackLayout.Padding;
			}
			set
			{
				this.NoNetworkStackLayout.Padding = value;
			}
		}

		#endregion

		#region Bindable properties

		public static BindableProperty TintProperty =
			BindableProperty.Create<NetworkPageHeader, Color>(o => o.Tint, Color.White, propertyChanged: OnTintChanged);

		public Color Tint
		{
			get { return (Color)GetValue(TintProperty); }
			set { SetValue(TintProperty, value); }
		}

		private static void OnTintChanged(BindableObject bindable, Color oldvalue, Color newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.LoadingLabel.TextColor = newvalue;
			header.ActivityIndicator.Color = newvalue;
		}

		#endregion
	}
}

