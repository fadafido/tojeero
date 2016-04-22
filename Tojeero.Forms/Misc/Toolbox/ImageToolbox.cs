using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Tojeero.Core;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

namespace Tojeero.Forms.Toolbox
{
    public static class ImageToolbox
    {
        #region Private fields

        private static readonly IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
        private static readonly IImageService _imageService = Mvx.Resolve<IImageService>();

        #endregion

        #region Static API

        public static async Task<IImage> PickImage(Page parent)
        {
            IImage image = null;
            string[] titles = null;
            if (_mediaPicker.IsCameraAvailable)
            {
                titles = new[] {AppResources.LabelFromCamera, AppResources.LabelFromLibrary};
            }
            if (titles != null)
            {
                if (parent != null)
                {
                    var action =
                        await
                            parent.DisplayActionSheet(AppResources.TitlePickImage, AppResources.ButtonCancel, null,
                                titles);
                    if (action == AppResources.LabelFromCamera)
                    {
                        image = await takePicture(true);
                    }
                    else if (action == AppResources.LabelFromLibrary)
                    {
                        image = await takePicture(false);
                    }
                }
            }
            else
            {
                image = await takePicture(false);
            }
            return image;
        }

        #endregion

        #region Utility methods

        private static async Task<IImage> takePicture(bool fromCamera)
        {
            try
            {
                MediaFile result = null;
                IImage image;
                if (fromCamera)
                {
                    image = await _imageService.GetImageFromCamera();
                    return image;
                }
                image = await _imageService.GetImageFromLibrary();
                return image;
                return image;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning, true);
            }
            return null;
        }

        #endregion
    }
}