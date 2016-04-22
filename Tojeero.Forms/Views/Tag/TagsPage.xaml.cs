using System;
using Tojeero.Core;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Tag;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Tag
{
	public partial class TagsPage : BaseSearchablePage
	{
		#region Private fields

		private ITag _selectedTag;
		private string _newTag;

		#endregion

		#region Properties

		public event EventHandler<EventArgs<string>> DidClose;

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
			this.ListView.RowHeight = 50;
			this.ListView.ItemSelected += itemSelected;
			this.ViewModel.CreateTagAction = createTag;
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
			string selectedTag = null;
			if (_newTag != null)
			{
				selectedTag = _newTag;
			}
			else
			{
				var selected = this.ViewModel.ViewModel.SelectedItem;
				selectedTag = selected != null ? selected.Tag.Text : null;
			}
			this.DidClose.Fire(this, new EventArgs<string>(selectedTag));	
		}

		#endregion

		#region Parent override

		private void itemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
				this.ViewModel.ViewModel.SelectedItem = e.SelectedItem as TagViewModel;
			this.ListView.SelectedItem = null;
		}

		#endregion

		#region Utility methods

		private async void createTag(string tag)
		{
			_newTag = tag;
			await this.Navigation.PopModalAsync();
		}

		#endregion
	}
}

