using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public interface ISearchViewModel
	{
		event EventHandler<EventArgs> LoadingNextPageFinished;
		event EventHandler<EventArgs> ReloadFinished;
		string SearchQuery { get; set; }
	}
	
}
