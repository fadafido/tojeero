using System;
using Xamarin.Forms;
using System.ComponentModel;

namespace Tojeero.Core.ViewModels
{
	public interface IImageViewModel : INotifyPropertyChanged
	{
		ImageSource Image { get; }
		string ImageUrl { get; set; }
		IImage NewImage { get; set; }
	}
}

