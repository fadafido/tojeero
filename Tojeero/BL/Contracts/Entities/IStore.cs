﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IStore : IModelEntity, ISearchableEntity
	{
		string Name { get; set; }

		string Description { get; set; }

		string LowercaseName { get; set; }

		string DeliveryNotes { get; set; }

		string ImageUrl { get; }

		bool IsBlocked { get; }

		string CategoryID { get; set; }

		string OwnerID { get; set; }

		IUser Owner { get; }

		IStoreCategory Category { get; }

		string CityId { get; set; }

		ICity City { get; }

		string CountryId { get; set; }

		ICountry Country { get; }

		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, bool includeInvisible = false);

		Task Save();

		Task SetMainImage(IImage image);

		Task FetchCountry();

		Task LoadRelationships();
	}
}

