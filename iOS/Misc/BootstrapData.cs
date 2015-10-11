using System;
using System.Threading.Tasks;
using System.Linq;
using Parse;
using UIKit;
using System.Collections.Generic;
using Tojeero.Core;
using PCLStorage;
using Newtonsoft.Json;
using Tojeero.Core.Toolbox;

namespace Tojeero.iOS
{
	class JsonProduct
	{
		public string Name { get; set; }

		public string Tags { get; set; }

		public string Description { get; set; }

		public int Price { get; set; }
	}

	class JsonStore
	{
		public string Name { get; set; }

		public string Description { get; set; }
	}

	public static class BootstrapData
	{
		public static async Task GenerateSampleProductsAndStores()
		{
			string stage = "";
			int index = 0;
			try
			{
				var jsonProducts = await LoadFromJson<JsonProduct>("products.json");
				var jsonStores = await LoadFromJson<JsonStore>("stores.json");
				int productSampleCount = 18, storeSampleCount = 12;
				int productCount = jsonProducts.Count, storeCount = jsonStores.Count;
				string productsDir = "Samples/Products/";
				string storesDir = "Samples/Stores/";
				Random priceRandom = new Random();

				/////////
				stage = "Loading product images";
				index = 0;
				var productImages = Enumerable.Range(1, productSampleCount).Select(i =>
					{
						index++;
						var name = string.Format("Product {0}.png", i);
						return new ParseFile(name, UIImage.FromFile(string.Format("{0}{1}", productsDir, name)).GetRawBytes(ImageType.Png));
					}).ToArray();

				////////
				stage = "Loading store images";
				index = 0;
				var storeImages = Enumerable.Range(1, storeSampleCount).Select(i =>
					{
						index++;
						var name = string.Format("Store {0}.png", i);
						return new ParseFile(name, UIImage.FromFile(string.Format("{0}{1}", storesDir, name)).GetRawBytes(ImageType.Png));
					}).ToArray();

				//////////
				stage = "Saving product images";
				int count = 1;
				index = 0;
				foreach (var file in productImages)
				{
					index++;
					await file.SaveAsync();
					Console.WriteLine("Saved {0} of {1} product images", count++, productSampleCount);
				}

				//////////
				stage = "Saving store images";
				count = 0;
				index = 0;
				foreach (var file in storeImages)
				{
					index++;
					await file.SaveAsync();
					Console.WriteLine("Saved {0} of {1} store images", count++, storeSampleCount);
				}

				//////////
				stage = "Saving products";
				List<ParseProduct> products = new List<ParseProduct>();
				for (int i = 0; i < productCount; i++)
				{
					index = i;
					var prod = jsonProducts[i];
					var product = new ParseProduct()
					{
						Name = prod.Name,
						Description = prod.Description,
						Tags = prod.Tags.Tokenize(),
						Image = productImages[i % productSampleCount],
						Price = prod.Price
					};
					product.SearchTokens = new string[] { product.Name, product.Description, prod.Tags }.Tokenize();
					products.Add(product);
				}
				await ParseObject.SaveAllAsync<ParseProduct>(products);
				Console.WriteLine("Saved {0} products ", productCount);

				//////////
				stage = "Saving stores";
				List<ParseStore> stores = new List<ParseStore>();
				for (int i = 0; i < storeCount; i++)
				{
					index = i;
					var s = jsonStores[i];
					var store = new ParseStore()
					{
						Name = s.Name,
						Image = storeImages[i % storeSampleCount],
						Description = s.Description						
					};
					store.SearchTokens = new string[] { s.Name, s.Description }.Tokenize();
					stores.Add(store);
				}
				await ParseObject.SaveAllAsync<ParseStore>(stores);
				Console.WriteLine("Saved {0} stores ", storeCount);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured while {0}, index = {1}. {2}.", stage, index, ex.ToString());
			}
		}


		public static async Task CreateData()
		{
			var countries = new ParseCountry[]
			{
				new ParseCountry()
				{
					CountryId = 1,
					Name_ar = "الامارات",
					Name_en = "UAE",
					Currency_ar = "درهم",
					Currency_en = "AED",
					CountryPhoneCode = "971"
				},
				new ParseCountry()
				{
					CountryId = 2,
					Name_ar = "السعودية",
					Name_en = "KSA",
					Currency_ar = "ريال",
					Currency_en = "Ryal",
					CountryPhoneCode = "966"
				},
				new ParseCountry()
				{
					CountryId = 3,
					Name_ar = "الكويت",
					Name_en = "Kuwait",
					Currency_ar = "دينار",
					Currency_en = "Dinar",
					CountryPhoneCode = "965"
				},
				new ParseCountry()
				{
					CountryId = 4,
					Name_ar = "الاردن",
					Name_en = "Jordan",
					Currency_ar = "دينار",
					Currency_en = "Dinar",
					CountryPhoneCode = "962"
				}
			};

			await ParseObject.SaveAllAsync<ParseCountry>(countries);
		}

		private static async Task<List<T>> LoadFromJson<T>(string file)
		{
			var dataFolder = await FileSystem.Current.GetFolderFromPathAsync("Samples");
			var store = await dataFolder.GetFileAsync(file);
			var json = await store.ReadAllTextAsync();
			var items = JsonConvert.DeserializeObject<List<T>>(json);
			return items;
		}
	}
}

