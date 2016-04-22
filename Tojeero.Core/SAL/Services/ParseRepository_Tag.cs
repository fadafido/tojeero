using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parse;
using Tojeero.Core;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Services
{
	public partial class ParseRepository
	{
		#region IRepository implementation

		public async Task<IEnumerable<ITag>> FetchTags(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchTagsTimeout))
			{
				var query = new ParseQuery<ParseTag>().OrderBy(p => p.Text);
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Tag(p) as ITag);
			}
		}

		public async Task<IEnumerable<ITag>> FindTags(string searchQuery, int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindTagsTimeout))
			{
				var parseQuery = new ParseQuery<ParseTag>().Where(t => t.Text.StartsWith(searchQuery.Trim())).OrderBy(t => t.Text);
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Tag(s) as ITag);
			}
		}
		#endregion
	}
}

