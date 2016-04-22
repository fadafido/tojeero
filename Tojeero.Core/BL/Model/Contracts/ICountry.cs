using System.Threading.Tasks;

namespace Tojeero.Core.Model.Contracts
{
	public interface ICountry : IUniqueEntity
	{
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
		string Currency { get; }
		string Currency_en { get; }
		string Currency_ar { get; }
		string CountryPhoneCode { get; }

		ICity[] Cities { get; }
		Task LoadCities();
	}
}

