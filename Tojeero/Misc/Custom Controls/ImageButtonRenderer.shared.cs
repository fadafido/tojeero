#if __ANDROID__
using Xamarin.Forms.Platform.Android;
#elif __IOS__
using Xamarin.Forms.Platform.iOS;
#endif

namespace Tojeero.Forms.Renderers
{
	using Xamarin.Forms;

	/// <summary>
	/// Draws a button on the Android platform with the image shown in the right 
	/// position with the right size.
	/// </summary>
	public partial class ImageButtonRenderer
	{
		/// <summary>
		/// Returns the proper <see cref="IImageSourceHandler"/> based on the type of <see cref="ImageSource"/> provided.
		/// </summary>
		/// <param name="source">The <see cref="ImageSource"/> to get the handler for.</param>
		/// <returns>The needed handler.</returns>
		private static IImageSourceHandler GetHandler(ImageSource source)
		{
			IImageSourceHandler returnValue = null;
			if (source is UriImageSource)
			{
				returnValue = new ImageLoaderSourceHandler();
			}
			else if (source is FileImageSource)
			{
				returnValue = new FileImageSourceHandler();
			}
			else if (source is StreamImageSource)
			{
				returnValue = new StreamImagesourceHandler();
			}
			return returnValue;
		}
	}
}