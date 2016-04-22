using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.Managers
{
    public class TagManager : ITagManager
    {
        #region Private fields and properties

        private readonly IModelEntityManager _manager;

        #endregion

        #region Constructors

        public TagManager(IModelEntityManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region ITagManager implementation

        public Task<IEnumerable<ITag>> Fetch(int pageSize, int offset)
        {
            return _manager.Fetch<ITag, Tag>(new FetchTagsQuery(pageSize, offset, _manager),
                Constants.TagsCacheTimespan.TotalMilliseconds);
        }

        public Task<IEnumerable<ITag>> Find(string query, int pageSize, int offset)
        {
            return _manager.Fetch<ITag, Tag>(new FindTagsQuery(query, pageSize, offset, _manager),
                Constants.TagsCacheTimespan.TotalMilliseconds);
        }

        public Task ClearCache()
        {
            return _manager.Cache.Clear<Tag>();
        }

        #endregion
    }

    #region Queries

    public class FetchTagsQuery : IQueryLoader<ITag>
    {
        readonly int pageSize;
        readonly int offset;
        readonly IModelEntityManager manager;

        public FetchTagsQuery(int pageSize, int offset, IModelEntityManager manager)
        {
            this.manager = manager;
            this.offset = offset;
            this.pageSize = pageSize;
        }

        public string ID
        {
            get
            {
                var cachedQueryId = string.Format("tags-p{0}o{1}", pageSize, offset);
                return cachedQueryId;
            }
        }

        public async Task<IEnumerable<ITag>> LocalQuery()
        {
            return await manager.Cache.FetchTags(pageSize, offset);
        }

        public async Task<IEnumerable<ITag>> RemoteQuery()
        {
            return await manager.Rest.FetchTags(pageSize, offset);
        }

        public async Task PostProcess(IEnumerable<ITag> items)
        {
        }
    }

    public class FindTagsQuery : IQueryLoader<ITag>
    {
        readonly int pageSize;
        readonly int offset;
        readonly IModelEntityManager manager;
        readonly string searchQuery;

        public FindTagsQuery(string searchQuery, int pageSize, int offset, IModelEntityManager manager)
        {
            this.searchQuery = searchQuery;
            this.manager = manager;
            this.offset = offset;
            this.pageSize = pageSize;
        }

        public string ID
        {
            get
            {
                var cachedQueryId = string.Format("tags-p{0}o{1}-{2}", pageSize, offset,
                    string.Join(",", searchQuery.Tokenize()));
                return cachedQueryId;
            }
        }

        public async Task<IEnumerable<ITag>> LocalQuery()
        {
            var result = await manager.Cache.FindTags(searchQuery, pageSize, offset);
            return result;
        }

        public async Task<IEnumerable<ITag>> RemoteQuery()
        {
            var result = await manager.Rest.FindTags(searchQuery, pageSize, offset);
            return result;
        }

        public async Task PostProcess(IEnumerable<ITag> items)
        {
        }
    }

    #endregion
}