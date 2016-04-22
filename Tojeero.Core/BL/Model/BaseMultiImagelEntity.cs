using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public abstract class BaseMultiImagelEntity<T> : BaseModelEntity<T>, IMultiImageEntity
        where T : ParseObject, IParseMultiImageEntity
    {
        #region Constructors

        public BaseMultiImagelEntity(T parseObject = null)
            : base(parseObject)
        {
        }

        #endregion

        #region IMultiImageEntity implementation

        public async Task<IEnumerable<IData>> GetImages()
        {
            if (ParseObject == null)
                return null;
            var result = await ParseObject.Images.Query.FindAsync();
            var images = result.Select(d => new Data(d));
            return images;
        }

        public async Task AddImage(IImage image, bool shouldSave = true)
        {
            if (ParseObject == null)
                return;
            var imageFile = new ParseFile(image.Name, image.RawImage);
            await imageFile.SaveAsync();
            var data = new ParseData
            {
                File = imageFile
            };
            await data.SaveAsync();
            ParseObject.Images.Add(data);
            if (shouldSave)
                await ParseObject.SaveAsync();
        }

        public async Task AddImages(IEnumerable<IImage> images, bool shouldSave = true)
        {
            if (ParseObject == null)
                return;
            foreach (var image in images)
            {
                var imageFile = new ParseFile(image.Name, image.RawImage);
                await imageFile.SaveAsync();
                var data = new ParseData
                {
                    File = imageFile
                };
                await data.SaveAsync();
                ParseObject.Images.Add(data);
            }
            if (shouldSave)
                await ParseObject.SaveAsync();
        }

        public async Task RemoveImage(string imageID, bool shouldSave = true)
        {
            if (ParseObject == null)
                return;
            ParseObject.Images.Remove(Parse.ParseObject.CreateWithoutData<ParseData>(imageID));
            if (shouldSave)
                await ParseObject.SaveAsync();
        }

        public async Task RemoveImages(IEnumerable<string> imageIDs, bool shouldSave = true)
        {
            if (ParseObject == null)
                return;

            foreach (var imageID in imageIDs)
            {
                ParseObject.Images.Remove(Parse.ParseObject.CreateWithoutData<ParseData>(imageID));
            }
            if (shouldSave)
                await ParseObject.SaveAsync();
        }

        #endregion
    }
}