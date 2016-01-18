using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Cirrious.CrossCore;
using Tojeero.Core.Services;

namespace Tojeero.Forms
{
	public partial class BootstrapPage : ContentPage
	{
		#region Properties

		private BootstrapViewModel _viewModel;
		public BootstrapViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					_viewModel = value;
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region Constructors

		public BootstrapPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<BootstrapViewModel>();
			InitializeComponent();

			var localizationService = Mvx.Resolve<ILocalizationService>();
			this.languagesPicker.StringFormat = (language) =>
			{
					return localizationService.GetNativeLanguageName(language);
			};
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.BootstrapCommand.Execute(null);
		}

		#endregion
	}
}

