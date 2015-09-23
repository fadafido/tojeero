using System;

namespace Tojeero.Droid
{
	public interface IMvxPageNavigationHost
	{
		IMvxPageNavigationProvider NavigationProvider { get; set; }
	}
}

