using Cirrious.MvvmCross.Plugins.Location;

namespace Tojeero.Core.Services.Contracts
{
	public interface ILocationService
	{
		MvxGeoLocation LastKnownLocation { get; }
	}
}

