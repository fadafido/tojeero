using System;

namespace Tojeero.Core.ViewModels.Contracts
{
	public interface ISearchViewModel
	{
		event EventHandler<EventArgs> LoadingNextPageFinished;
		event EventHandler<EventArgs> ReloadFinished;
		string SearchQuery { get; set; }
	}
	
}
