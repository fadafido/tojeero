using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Image
{
	public class PickedImage : IImage
	{
		public byte[] RawImage { get; set; }
		public string Name { get; set; }
	}
}

