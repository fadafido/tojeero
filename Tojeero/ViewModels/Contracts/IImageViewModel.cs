using System;

namespace Tojeero.Core.ViewModels
{
	public interface IImageViewModel
	{
		string ImageUrl { get; set; }
		IImage NewImage { get; set; }
	}
}

