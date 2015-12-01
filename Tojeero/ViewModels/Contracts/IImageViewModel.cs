using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Tojeero.Core.ViewModels
{
	public interface IImageViewModel : INotifyPropertyChanged
	{
		Action<IImageViewModel> DidPickImageAction { get; set; }

		Func<Task<IImage>> PickImageFunction { get; set; }

		string ImageID { get; set; }

		ImageSource Image { get; }

		string ImageUrl { get; set; }

		IImage NewImage { get; set; }

		ICommand PickImageCommand { get; }

		bool IsLoading { get; set; }

		ICommand RemoveImageCommand { get; }

		bool CanExecuteRemoveImageCommand { get; }

		/// <summary>
		/// Gets or sets the function which will be called before removing the image.
		/// The function should return true if you want to allow removal
		/// This will allow you to preprocess the image before deleting.
		/// </summary>
		/// <value>The remove item action.</value>
		Func<Task<bool>> RemoveImageAction { get; set; }
	}
}

