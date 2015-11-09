using System;

namespace Tojeero.Core.ViewModels
{
	public class PickedImage : IImage
	{
		public byte[] RawImage { get; set; }
		public string Name { get; set; }
	}
}

