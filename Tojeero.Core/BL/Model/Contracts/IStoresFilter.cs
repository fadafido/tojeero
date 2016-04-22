using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tojeero.Core.Model.Contracts
{
	public interface IStoreFilter : INotifyPropertyChanged
	{
		IStoreCategory Category { get; set; }
		ICountry Country { get; set; }
		ICity City { get; set; }
		ObservableCollection<string> Tags { get; set; }
		IStoreFilter Clone();
        void SetCountryID(string countryId);
        void SetCityID(string cityId);
    }
}

