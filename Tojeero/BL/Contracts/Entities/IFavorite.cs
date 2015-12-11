using System;
using System.ComponentModel;

namespace Tojeero.Core
{
	public interface IFavorite : INotifyPropertyChanged
	{
		string ObjectID { get; set; }
		bool IsFavorite { get; set; }
	}
}

