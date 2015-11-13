using System;
using Tojeero.Core.ViewModels;
using XLabs.Platform.Services.Media;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IImageService
	{
		Task<PickedImage> GetImage(MediaFile file);

		Task<IImage> GetImageFromLibrary();
		Task<IImage> GetImageFromCamera();
	}
}

