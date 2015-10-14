using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class TagsViewModel : BaseSearchViewModel<ITag>
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

		#region implemented abstract members of BaseSearchViewModel

		protected override BaseCollectionViewModel<ITag> GetBrowsingViewModel()
		{
			return new BaseCollectionViewModel<ITag>(new TagsQuery(_manager), Constants.TagsPageSize);
		}

		protected override BaseCollectionViewModel<ITag> GetSearchViewModel(string searchQuery)
		{
			return new BaseCollectionViewModel<ITag>(new SearchTagsQuery(searchQuery, _manager), Constants.TagsPageSize);
		}

		#endregion

		#region Queries

		private class TagsQuery : IModelQuery<ITag>
		{
			ITagManager manager;

			public TagsQuery(ITagManager manager)
			{
				this.manager = manager;

			}

			public Task<IEnumerable<ITag>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Fetch(pageSize, offset);
			}

			private Comparison<ITag> _comparer;

			public Comparison<ITag> Comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparison<ITag>((x, y) =>
							{
								if (x.ID == y.ID)
									return 0;
								return x.Text.CompareTo(y.Text);
							});
					}
					return _comparer;
				}
			}

			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchTagsQuery : IModelQuery<ITag>
		{
			ITagManager manager;
			string searchQuery;

			public SearchTagsQuery(string searchQuery, ITagManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public Task<IEnumerable<ITag>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Find(searchQuery, pageSize, offset);
			}

			private Comparison<ITag> _comparer;

			public Comparison<ITag> Comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparison<ITag>((x, y) =>
							{
								if (x.ID == y.ID)
									return 0;
								return x.Text.CompareTo(y.Text);
							});
					}
					return _comparer;
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

