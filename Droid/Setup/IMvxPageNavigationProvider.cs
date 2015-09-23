using System;
using Xamarin.Forms;

namespace Tojeero.Droid
{
	public interface IMvxPageNavigationProvider
	{
		void Push(Page page);
		void Pop();
	}
}

