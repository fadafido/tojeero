using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Services.Contracts
{
    public interface IImageService
    {
        Task<IImage> GetImageFromLibrary();
        Task<IImage> GetImageFromCamera();
    }
}