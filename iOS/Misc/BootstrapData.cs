using System;
using System.Threading.Tasks;
using System.Linq;
using Parse;
using UIKit;
using System.Collections.Generic;
using Tojeero.Core;

namespace Tojeero.iOS
{
	public static class BootstrapData
	{
		public static async Task GenerateSampleProductsAndStores()
		{
			string stage = "";
			int index = 0;
			try
			{
				int productSampleCount = 18, storeSampleCount = 12;
				int productCount = 200, storeCount = 200;
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
					var product = new ParseProduct()
					{
						Name = string.Format("Product {0}", i + 1),
						Image = productImages[i % productSampleCount],
						Price = (double)priceRandom.Next(10, 200)
					};
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
					var store = new ParseStore()
					{
						Name = string.Format("Store {0}", i + 1),
						Image = storeImages[i % storeSampleCount]
					};
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
					Name_ar = "الامارات",
					Name_en = "UAE",
					Currency_ar = "درهم",
					Currency_en = "AED",
					CountryPhoneCode = "971"
				},
				new ParseCountry()
				{
					Name_ar = "السعودية",
					Name_en = "KSA",
					Currency_ar = "ريال",
					Currency_en = "Ryal",
					CountryPhoneCode = "966"
				},
				new ParseCountry()
				{
					Name_ar = "الكويت",
					Name_en = "Kuwait",
					Currency_ar = "دينار",
					Currency_en = "Dinar",
					CountryPhoneCode = "965"
				},
				new ParseCountry()
				{
					Name_ar = "الاردن",
					Name_en = "Jordan",
					Currency_ar = "دينار",
					Currency_en = "Dinar",
					CountryPhoneCode = "962"
				}
			};

			await ParseObject.SaveAllAsync<ParseCountry>(countries);
		}
	}
}

