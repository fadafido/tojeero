using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class TagsPage : BaseSearchablePage
	{
		#region Properties

		public new TagsViewModel ViewModel
		{
			get
			{
				return base.ViewModel as TagsViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Constructors

		public TagsPage()
			: base()
		{
			InitializeComponent();
			this.ViewModel = MvxToolbox.LoadViewModel<TagsViewModel>();
			this.ToolbarItems.Add(new ToolbarItem("Close", "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
			this.SearchBar.Placeholder = "Search for tags";
			this.ListView.RowHeight = 30;
			this.ListView.SeparatorVisibility = SeparatorVisibility.Default;
			this.ListView.BackgroundColor = Color.White;
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion
	}
}

