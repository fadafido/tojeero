using System;
using Cirrious.MvvmCross.ViewModels;
using Parse;
using Cirrious.MvvmCross;
using System.ComponentModel;
using System.Linq.Expressions;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core
{
	public abstract class BaseMultiImagelEntity<T> : BaseModelEntity<T>, IMultiImageEntity where T : ParseObject, IParseMultiImageEntity
	{
		#region Constructors

		public BaseMultiImagelEntity(T parseObject = null)
			:base(parseObject)
		{

		}

		#endregion

		#region IMultiImageEntity implementation

		public async Task<IEnumerable<IData>> GetImages()
		{
			if (this.ParseObject == null)
				return null;
			var result = await this.ParseObject.Images.Query.FindAsync();
			var images = result.Select(d => new Data(d));
			return images;
		}

		public async Task AddImage(IImage image)
		{
			if (this.ParseObject == null)
				return;
			var imageFile = new ParseFile(image.Name, image.RawImage);
			await imageFile.SaveAsync();
			var data = new ParseData()
				{ 
					File = imageFile
				};
			await data.SaveAsync();
			this.ParseObject.Images.Add(data);
			this.ParseObject.SaveAsync();
		}

		public async Task AddImages(IEnumerable<IImage> images)
		{
			if (this.ParseObject == null)
				return;
			foreach (var image in images)
			{
				var imageFile = new ParseFile(image.Name, image.RawImage);
				await imageFile.SaveAsync();
				var data = new ParseData()
					{ 
						File = imageFile
					};
				await data.SaveAsync();
				this.ParseObject.Images.Add(data);
			}
			this.ParseObject.SaveAsync();
		}

		public async Task RemoveImage(string imageID)
		{
			if (this.ParseObject == null)
				return;
			this.ParseObject.Images.Remove(Parse.ParseObject.CreateWithoutData<ParseData>(imageID));
			await this.ParseObject.SaveAsync();
		}

		public async Task RemoveImages(IEnumerable<string> imageIDs)
		{
			if (this.ParseObject == null)
				return;
			
			foreach (var imageID in imageIDs)
			{
				this.ParseObject.Images.Remove(Parse.ParseObject.CreateWithoutData<ParseData>(imageID));
			}
			await this.ParseObject.SaveAsync();
		}
			
		#endregion
	}
		
}

