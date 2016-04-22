namespace Tojeero.Core.Model.Contracts
{
	public interface IImage
	{
		byte[] RawImage { get; set; }
		string Name { get; set; }
	}
}

