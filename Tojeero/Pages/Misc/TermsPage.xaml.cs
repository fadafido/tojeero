using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class TermsPage : ContentPage
	{
		#region Constructors

		public TermsPage()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<TermsViewModel>();
			InitializeComponent();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
		}

		#endregion

		#region Properties

		private TermsViewModel _viewModel;

		public TermsViewModel ViewModel
		{ 
			get
			{
				return _viewModel; 
			}
			set
			{
				_viewModel = value; 
				this.BindingContext = _viewModel;
			}
		}

		#endregion
	}
}

