using System;
using Cirrious.MvvmCross.Plugins.Location;

namespace Tojeero.Core.Services
{
	public interface ILocationService
	{
		MvxGeoLocation LastKnownLocation { get; }
	}
}

