using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Contracts;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IQueryLoader<T>
    {
        /// <summary>
        /// Gets the ID of the query which will be used to uniquely indentify this query in cache. 
        /// Return null if if you want to disable caching for this query.
        /// </summary>
        /// <value>The ID of the query.</value>
        string ID { get; }

        Task<IEnumerable<T>> LocalQuery();

        Task<IEnumerable<T>> RemoteQuery();

        /// <summary>
        /// This method will be called before the result of the remote query will be saved to local cache.
        /// You can use this method to do additional processing of the result.
        /// </summary>
        /// <param name="items">The resulting items of the query returned from RemoteQuery method.</param>
        Task PostProcess(IEnumerable<T> items);
    }

    public interface IModelEntityManager
    {
        IRestRepository Rest { get; }
        ICacheRepository Cache { get; }
        Task<IEnumerable<T>> Fetch<T, Entity>(IQueryLoader<T> loader, double? expiresIn = null);
    }
}