using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Core;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class TagsPage : BaseSearchablePage
	{
		#region Private fields

		private ITag _selectedTag;

		#endregion

		#region Properties

		public event EventHandler<EventArgs<ITag>> DidClose;

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
			this.SearchBar.Placeholder = "Search for tags";
			this.ListView.RowHeight = 30;
			this.ListView.ItemSelected += itemSelected;
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
				{
					await this.Navigation.PopModalAsync();
				}));
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			this.DidClose.Fire(this, new EventArgs<ITag>(this.ViewModel.ViewModel.SelectedItem));
		}

		#endregion

		#region Parent override

		private void itemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
				this.ViewModel.ViewModel.SelectedItem = e.SelectedItem as ITag;
			this.ListView.SelectedItem = null;
		}

		#endregion
	}
}

