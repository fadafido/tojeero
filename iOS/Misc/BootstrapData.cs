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

	class JsonSubcategory
	{
		public string name_en { get; set; }

		public string name_ar { get; set; }

		public string category { get; set; }
	}

	public static class BootstrapData
	{
		public static async Task GenerateSampleProductsAndStores()
		{
			var stores = await GenerateSampleStores();
			await GenerateSampleProducts(stores);
		}
		
		private static async Task GenerateSampleProducts(List<ParseStore> stores)
		{
			string stage = "";
			int index = 0;
			try
			{
				var catMap = new Dictionary<ParseProductCategory,ParseProductSubcategory[]>();
				var categories = (await new ParseQuery<ParseProductCategory>().FindAsync()).ToArray();
				foreach (var cat in categories)
				{
					catMap[cat] = (await new ParseQuery<ParseProductSubcategory>().Where(s => s.Category == cat).FindAsync()).ToArray();
				}

				var countryMap = new Dictionary<int, ParseCity[]>();
				var countries = (await new ParseQuery<ParseCountry>().FindAsync()).ToArray();
				foreach (var c in countries)
				{
					countryMap[c.CountryId] = (await new ParseQuery<ParseCity>().Where(cty => cty.CountryId == c.CountryId).FindAsync()).ToArray();
				}

				var jsonProducts = await LoadFromJson<JsonProduct>("products.json");
				int productSampleCount = 18;
				int productCount = jsonProducts.Count;
				string productsDir = "Samples/Products/";
				Random catRandom = new Random();
				Random subcatRandom = new Random();
				Random countryRand = new Random();
				Random cityRand = new Random();
				Random storeRand = new Random();

				/////////
				stage = "Loading product images";
				index = 0;
				var productImages = Enumerable.Range(1, productSampleCount).Select(i =>
					{
						index++;
						var name = string.Format("Product {0}.png", i);
						return new ParseFile(name, UIImage.FromFile(string.Format("{0}{1}", productsDir, name)).GetRawBytes(ImageType.Png));
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
				stage = "Saving products";
				List<ParseProduct> products = new List<ParseProduct>();
				for (int i = 0; i < productCount; i++)
				{
					index = i;
					var prod = jsonProducts[i];
					var product = new ParseProduct()
					{
						Name = prod.Name,
						LowercaseName = prod.Name.ToLower(),
						Description = prod.Description,
						Tags = prod.Tags.Tokenize(),
						Image = productImages[i % productSampleCount],
						Price = prod.Price,
						Store = stores[storeRand.Next(0, stores.Count)]
					};
					product.Category = categories[catRandom.Next(0, categories.Length)];
					var subs = catMap[product.Category];
					product.Subcategory = subs[subcatRandom.Next(0, subs.Length)];

					product.CountryId = countries[countryRand.Next(0, countries.Length)].CountryId;
					var cities = countryMap[product.CountryId.Value];
					product.CityId = cities[cityRand.Next(0, cities.Length)].CityId;

					product.SearchTokens = new string[] { product.Name, product.Description, prod.Tags }.Tokenize();
					var tags = prod.Tags.Tokenize().Select(t => new ParseTag() { Text = t });
					try
					{
						await tags.SaveAllAsync();
					}
					catch (Exception ex)
					{
						
					}
					products.Add(product);
				}
				await ParseObject.SaveAllAsync<ParseProduct>(products);
				Console.WriteLine("Saved {0} products ", productCount);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured while {0}, index = {1}. {2}.", stage, index, ex.ToString());
			}
		}

		private static async Task<List<ParseStore>> GenerateSampleStores()
		{
			string stage = "";
			int index = 0;
			try
			{
				var categories = (await new ParseQuery<ParseStoreCategory>().FindAsync()).ToArray();
				var countryMap = new Dictionary<int, ParseCity[]>();
				var countries = (await new ParseQuery<ParseCountry>().FindAsync()).ToArray();
				foreach (var c in countries)
				{
					countryMap[c.CountryId] = (await new ParseQuery<ParseCity>().Where(cty => cty.CountryId == c.CountryId).FindAsync()).ToArray();
				}

				var jsonStores = await LoadFromJson<JsonStore>("stores.json");
				int storeSampleCount = 12, count;
				int storeCount = jsonStores.Count;
				string storesDir = "Samples/Stores/";
				Random catRandom = new Random();
				Random countryRand = new Random();
				Random cityRand = new Random();

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
				stage = "Saving stores";
				List<ParseStore> stores = new List<ParseStore>();
				for (int i = 0; i < storeCount; i++)
				{
					index = i;
					var s = jsonStores[i];
					var store = new ParseStore()
					{
						Name = s.Name,
						LowercaseName = s.Name.ToLower(),
						Image = storeImages[i % storeSampleCount],
						Description = s.Description						
					};
					store.Category = categories[catRandom.Next(0, categories.Length)];

					store.CountryId = countries[countryRand.Next(0, countries.Length)].CountryId;
					var cities = countryMap[store.CountryId.Value];
					store.CityId = cities[cityRand.Next(0, cities.Length)].CityId;

					store.SearchTokens = new string[] { s.Name, s.Description }.Tokenize();
					stores.Add(store);
				}
				await ParseObject.SaveAllAsync<ParseStore>(stores);
				Console.WriteLine("Saved {0} stores ", storeCount);
				return stores;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured while {0}, index = {1}. {2}.", stage, index, ex.ToString());
				return null;
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

		public static async Task CreateSubcategories()
		{
			var subcategories = (await LoadFromJson<JsonSubcategory>("subcategories.json")).Select(s => new ParseProductSubcategory()
				{ 
					Name_en = s.name_en,
					Name_ar = s.name_ar,
					Category = ParseObject.CreateWithoutData<ParseProductCategory>(s.category)
				});
			await subcategories.SaveAllAsync();
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

