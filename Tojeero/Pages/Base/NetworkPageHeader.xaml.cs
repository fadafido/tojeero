using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Windows.Input;

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

		#region Tint

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

		#region IsLoading

		public static BindableProperty IsLoadingProperty =
			BindableProperty.Create<NetworkPageHeader, bool>(o => o.IsLoading, false, propertyChanged: OnIsLoadingChanged);

		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			set { SetValue(IsLoadingProperty, value); }
		}

		private static void OnIsLoadingChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.ActivityIndicator.IsRunning = newvalue;
			header.LoadingContainer.IsVisible = newvalue;
		}

		#endregion

		#region LoadingText

		public static BindableProperty LoadingTextProperty = BindableProperty.Create<NetworkPageHeader, string>(o => o.LoadingText, "", propertyChanged: OnLoadingTextChanged);

		public string LoadingText
		{
			get { return (string)GetValue(LoadingTextProperty); }
			set { SetValue(LoadingTextProperty, value); }
		}

		private static void OnLoadingTextChanged(BindableObject bindable, string oldvalue, string newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.LoadingLabel.Text = newvalue;
		}

		#endregion

		#region FailureMessage

		public static BindableProperty FailureMessageProperty = BindableProperty.Create<NetworkPageHeader, string>(o => o.FailureMessage, "", propertyChanged: OnFailureMessageChanged);

		public string FailureMessage
		{
			get { return (string)GetValue(FailureMessageProperty); }
			set { SetValue(FailureMessageProperty, value); }
		}

		private static void OnFailureMessageChanged(BindableObject bindable, string oldvalue, string newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.FailureLabel.Text = newvalue;
			header.FailureContainer.IsVisible = !string.IsNullOrEmpty(newvalue);
		}

		#endregion

		#region IsNetworkAvailable


		public static BindableProperty IsNetworkAvailableProperty = BindableProperty.Create<NetworkPageHeader, bool>(o => o.IsNetworkAvailable, false, propertyChanged: OnIsNetworkAvailableChanged);

		public bool IsNetworkAvailable
		{
			get { return (bool)GetValue(IsNetworkAvailableProperty); }
			set { SetValue(IsNetworkAvailableProperty, value); }
		}

		private static void OnIsNetworkAvailableChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.TryAgainButton.IsEnabled = newvalue;
		}

		#endregion

		#endregion

		#region Commands

		public static readonly BindableProperty TryAgainCommandProperty = BindableProperty.Create<InfiniteListView, ICommand>(bp => bp.LoadMoreCommand, default(ICommand), propertyChanged: OnTryAgainCommandChanged);

		public ICommand TryAgainCommand
		{
			get { return (ICommand) GetValue(TryAgainCommandProperty); }
			set { SetValue(TryAgainCommandProperty, value); }
		}

		private static void OnTryAgainCommandChanged(BindableObject bindable, ICommand oldvalue, ICommand newvalue)
		{
			var header = bindable as NetworkPageHeader;
			header.TryAgainButton.Command = newvalue;
		}

		#endregion
	}
}

