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
				List<Product> products = new List<Product>();
				for (int i = 0; i < productCount; i++)
				{
					index=i;
					var product = new Product()
						{
							Name = string.Format("Product {0}", i + 1),
							Image = productImages[i % productSampleCount],
							Price = (double)priceRandom.Next(10, 200)
						};
					products.Add(product);
				}
				await ParseObject.SaveAllAsync<Product>(products);
				Console.WriteLine("Saved {0} products ", productCount);

				//////////
				stage = "Saving stores";
				List<Store> stores = new List<Store>();
				for (int i = 0; i < storeCount; i++)
				{
					index=i;
					var store = new Store(){
						Name = string.Format("Store {0}", i + 1),
						Image = storeImages[i % storeSampleCount]
					};
					stores.Add(store);
				}
				await ParseObject.SaveAllAsync<Store>(stores);
				Console.WriteLine("Saved {0} stores ", storeCount);
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error occured while {0}, index = {1}. {2}.", stage, index, ex.ToString());
			}
		}

	}
}

