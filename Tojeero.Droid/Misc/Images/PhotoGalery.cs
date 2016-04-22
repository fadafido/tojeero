using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Nito.AsyncEx;

namespace Tojeero.Droid.Images
{
	public class PhotoGalery
	{
		#region Private fields and properties

		private string[] _imagePaths;
		private int[] _imageIDs;
		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
		private Context _context;

		#endregion

		#region Constructors

		public PhotoGalery(Context context)
		{
			_context = context;
		}

		#endregion

		#region Public API

		public async Task LoadImages()
		{
			using (var writerLock = await _locker.WriterLockAsync())
			{
				if (_imageIDs != null)
					return;
				await Task.Factory.StartNew(() =>
					{
						string[] columns = { MediaStore.Images.Media.InterfaceConsts.Data, MediaStore.Images.Media.InterfaceConsts.Id };
						string orderBy = MediaStore.Images.Media.InterfaceConsts.DateModified + " DESC";

						var imageCursor = _context.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, columns, null, null, orderBy);

						int image_column_index = imageCursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id);
						var count = imageCursor.Count;
						this._imagePaths = new string[count];
						_imageIDs = new int[count];

						for (int i = 0; i < count; i++)
						{
							imageCursor.MoveToPosition(i);
							_imageIDs[i] = imageCursor.GetInt(image_column_index);
							int dataColumnIndex = imageCursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data);
							_imagePaths[i] = imageCursor.GetString(dataColumnIndex);
						}
						imageCursor.Close();
						this.Count = _imageIDs.Length;
					});
			}
		}

		public async Task<Bitmap> GetThumbnailAtIndex(int index, CancellationToken token)
		{
			if (!(index >= 0 && index < this.Count))
				return null;
			//Load the new image
			var image = await Task<Bitmap>.Factory.StartNew(() =>
				{
					var bitmap = MediaStore.Images.Thumbnails.GetThumbnail(
						             _context.ContentResolver,
						             _imageIDs[index], 
						             ThumbnailKind.MiniKind, null);
					return bitmap;
				}, token);
			return image;
		}

		public string GetImagePathAtIndex(int index)
		{
			if (!(index >= 0 && index < this.Count))
				return null;
			//Load the new image path
			var imagePath = _imagePaths[index];
			
			return imagePath;
		}

		private int _count;

		public int Count
		{
			get
			{
				return _count;
			}
			private set
			{
				_count = value;
			}
		}

		#endregion

	}
}

