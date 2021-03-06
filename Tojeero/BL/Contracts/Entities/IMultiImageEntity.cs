﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IMultiImageEntity
	{
		Task<IEnumerable<IData>> GetImages();

		Task AddImage(IImage image, bool shouldSave = true);

		Task AddImages(IEnumerable<IImage> images, bool shouldSave = true);

		Task RemoveImage(string imageID, bool shouldSave = true);

		Task RemoveImages(IEnumerable<string> imageIDs, bool shouldSave = true);
	}
}

