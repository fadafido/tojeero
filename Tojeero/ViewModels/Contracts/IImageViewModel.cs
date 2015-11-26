using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Tojeero.Core.ViewModels
{
	public interface IImageViewModel : INotifyPropertyChanged
	{
		Action<IImageViewModel> RemoveImageAction { get; set; }

		Action<IImageViewModel> DidPickImageAction { get; set; }

		Func<Task<IImage>> PickImageFunction { get; set; }

		ImageSource Image { get; }

		string ImageUrl { get; set; }

		IImage NewImage { get; set; }

		ICommand PickImageCommand { get; }

		bool IsPickingImage { get; set; }

		ICommand RemoveImageCommand { get; }

		bool CanExecuteRemoveImageCommand { get; }
	}
}

