using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Tag
{
	public class TagsViewModel : BaseSearchViewModel<TagViewModel>
	{
		#region Private fields and properties

		private readonly ITagManager _manager;

		#endregion

		#region Constructors

		public TagsViewModel(ITagManager manager)
			: base()
		{
			_manager = manager;
		}

		#endregion

		#region Properties

		public Action<string> CreateTagAction { get; set; }

		public BaseSelectableCollectionViewModel<TagViewModel> ViewModel
		{ 
			get
			{
				return base.ViewModel as BaseSelectableCollectionViewModel<TagViewModel>; 
			}
			set
			{
				base.ViewModel = value;
			}
		}

		public override int MinSearchCharacters
		{
			get
			{
				return 1;
			}
		}

		private bool _createTagVisible;

		public bool CreateTagVisible
		{ 
			get
			{
				return _enableTagCreation && _createTagVisible;  
			}
			set
			{
				_createTagVisible = value; 
				RaisePropertyChanged(() => CreateTagVisible); 
			}
		}

		public string CreateTagButtonTitle
		{
			get
			{
				return string.Format("Create \"{0}\" tag.", this.SearchQuery);
			}
		}

		private bool _enableTagCreation = false;

		public bool EnableTagCreation
		{ 
			get
			{
				return _enableTagCreation; 
			}
			set
			{
				_enableTagCreation = value; 
				RaisePropertyChanged(() => EnableTagCreation); 
				RaisePropertyChanged(() => CreateTagVisible); 
			}
		}

		#endregion

		#region Parent override

		public override string SearchQuery
		{
			get
			{
				return base.SearchQuery;
			}
			set
			{
				base.SearchQuery = value != null ? value.Trim() : "";
				this.CreateTagVisible = false;
				RaisePropertyChanged(() => CreateTagButtonTitle);
			}
		}

		protected override BaseCollectionViewModel<TagViewModel> GetBrowsingViewModel()
		{
			var viewModel = new BaseSelectableCollectionViewModel<TagViewModel>(new TagsQuery(_manager), Constants.TagsPageSize);
			viewModel.Placeholder = AppResources.MessageNoTags;
			return viewModel;
		}

		protected override BaseCollectionViewModel<TagViewModel> GetSearchViewModel(string searchQuery)
		{
			var viewModel = new BaseSelectableCollectionViewModel<TagViewModel>(new SearchTagsQuery(searchQuery, _manager), Constants.TagsPageSize);
			viewModel.Placeholder = AppResources.MessageNoTags;
			return viewModel;
		}

		protected override void HandleLoadingNextPageFinished(object sender, EventArgs e)
		{
			base.HandleLoadingNextPageFinished(sender, e);
			var query = this.SearchQuery != null ? this.SearchQuery : null;
			if (!string.IsNullOrEmpty(query) && query.Length >= 2)
			{
				this.CreateTagVisible = this.ViewModel.Collection.Where(t => t.Tag != null && t.Tag.Text == query).Count() == 0;
			}
			else
			{
				this.CreateTagVisible = false;
			}
		}

		protected override void HandleReloadFinished(object sender, EventArgs e)
		{
			base.HandleReloadFinished(sender, e);
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _createTagCommand;

		public System.Windows.Input.ICommand CreateTagCommand
		{
			get
			{
				_createTagCommand = _createTagCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						if (this.CreateTagVisible)
							CreateTagAction.Fire(this.SearchQuery);
					});
				return _createTagCommand;
			}
		}

		#endregion

		#region Queries

		private class TagsQuery : IModelQuery<TagViewModel>
		{
			ITagManager manager;

			public TagsQuery(ITagManager manager)
			{
				this.manager = manager;

			}

			public async Task<IEnumerable<TagViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var tags = await manager.Fetch(pageSize, offset);
				return tags.Select(t => new TagViewModel(t));
			}

			public Comparison<TagViewModel> Comparer
			{
				get
				{
					return Comparers.TagText;
				}
			}

			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchTagsQuery : IModelQuery<TagViewModel>
		{
			ITagManager manager;
			string searchQuery;

			public SearchTagsQuery(string searchQuery, ITagManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public async Task<IEnumerable<TagViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var tags = await manager.Find(searchQuery, pageSize, offset);
				return tags.Select(t => new TagViewModel(t));
			}

			public Comparison<TagViewModel> Comparer
			{
				get
				{
					return Comparers.TagText;
				}
			}

			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		#endregion
	}
}

