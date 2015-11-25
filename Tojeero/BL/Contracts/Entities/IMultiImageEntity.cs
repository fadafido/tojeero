using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IMultiImageEntity
	{
		Task<IEnumerable<IData>> GetImages();

		Task AddImage(IImage image);

		Task AddImages(IEnumerable<IImage> images);

		Task RemoveImage(string imageID);

		Task RemoveImages(IEnumerable<string> imageIDs);
	}
}

