
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using System.Threading.Tasks;
using Android.Graphics;

namespace Tojeero.Droid
{
	[Activity(Label = "PhotoGaleryActivity")]			
	public class PhotoGaleryActivity : Activity
	{
		#region Private fields and properties

		private ImageAdapter imageAdapter;

		private string[] arrPath;
		private int[] _imageIDs;
		private int count;

		#endregion

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Create your application here
			this.SetContentView(Resource.Layout.photo_galery);

			string[] columns = { MediaStore.Images.Media.InterfaceConsts.Data, MediaStore.Images.Media.InterfaceConsts.Id };
			string orderBy = MediaStore.Images.Media.InterfaceConsts.Id;

			var imagecursor =  ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, columns, null, null, orderBy);

			int image_column_index = imagecursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id);
			this.count = imagecursor.Count;
			this.arrPath = new string[this.count];
			_imageIDs = new int[count];
			for (int i = 0; i < this.count; i++) {
				imagecursor.MoveToPosition(i);
				_imageIDs[i] = imagecursor.GetInt(image_column_index);
				int dataColumnIndex = imagecursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data);
				arrPath[i] = imagecursor.GetString(dataColumnIndex);
			}

			GridView imagegrid = (GridView) FindViewById(Resource.Id.PhoneImageGrid);
			imageAdapter = new ImageAdapter(_imageIDs);
			imagegrid.Adapter = imageAdapter;
			imagecursor.Close();

//			Button selectBtn = (Button) FindViewById(Resource.Id.selectBtn);
//			selectBtn.setOnClickListener(new OnClickListener() {
//
//				public void onClick(View v) {
//					final int len = thumbnailsselection.length;
//					int cnt = 0;
//					string selectImages = "";
//					for (int i = 0; i < len; i++) {
//						if (thumbnailsselection[i]) {
//							cnt++;
//							selectImages = selectImages + arrPath[i] + "|";
//						}
//					}
//					if (cnt == 0) {
//						Toast.makeText(getApplicationContext(), "Please select at least one image", Toast.LENGTH_LONG).show();
//					} else {
//
//						Log.d("SelectedImages", selectImages);
//						Intent i = new Intent();
//						i.putExtra("data", selectImages);
//						setResult(Activity.RESULT_OK, i);
//						finish();
//					}
//				}
//			});
		}


//
//
//
//		public void onBackPressed() {
//			setResult(Activity.RESULT_CANCELED);
//			super.onBackPressed();
//
//		}

		public class ImageAdapter : BaseAdapter {
			
			private LayoutInflater _inflater;
			private int[] _imageIDs;

			public ImageAdapter(int[] imageIDs) 
			{
				this._imageIDs = imageIDs;
				_inflater = (LayoutInflater)Application.Context.GetSystemService(Context.LayoutInflaterService);
			}

			#region implemented abstract members of BaseAdapter

			public override Java.Lang.Object GetItem(int position)
			{
				return position;
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				ViewHolder holder;
				if (convertView == null) {
					holder = new ViewHolder();
					convertView = _inflater.Inflate(Resource.Layout.photo_galery_item, null);
					holder.imageview = (ImageView) convertView.FindViewById(Resource.Id.thumbImage);

					convertView.Tag = holder;
				} else {
					holder = (ViewHolder) convertView.Tag;
				}
				holder.imageview.Id = position;

				try 
				{
					setBitmap (holder.imageview, _imageIDs[position]);
				} 
				catch (Exception e) 
				{
					Console.WriteLine(e);
				}

				holder.id = position;
				return convertView;
			}

			public override int Count
			{
				get
				{
					return _imageIDs.Length;
				}
			}

			#endregion

			private async void setBitmap(ImageView imageView, int imageId)
			{
				var image = await Task<Bitmap>.Factory.StartNew(() => MediaStore.Images.Thumbnails.GetThumbnail(Application.Context.ContentResolver, imageId, ThumbnailKind.MicroKind, null));
				imageView.SetImageBitmap(image);
			}

			class ViewHolder : Java.Lang.Object
			{
				public ImageView imageview { get; set; }
				public int id { get; set; }
			}
		}
	}
}

