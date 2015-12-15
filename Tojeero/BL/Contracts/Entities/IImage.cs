using System;

namespace Tojeero.Core
{
	public interface IImage
	{
		byte[] RawImage { get; set; }
		string Name { get; set; }
	}
}

