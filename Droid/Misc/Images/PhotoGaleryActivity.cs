
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Content.PM;
using Nito.AsyncEx;
using System.Threading;
using Android.Support.V7.Widget;
using Android.Graphics.Drawables;

namespace Tojeero.Droid
{
	[Activity(Label = "PhotoGaleryActivity", Icon = "@drawable/icon", Theme = "@style/Theme.Tojeero", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class PhotoGaleryActivity : Activity
	{
		#region Private fields and properties

		RecyclerView _recyclerView;
		GridLayoutManager _layoutManager;
		PhotoGaleryAdapter _adapter;
		PhotoGalery _galery;
		#endregion

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Create your application here
			this.SetContentView(Resource.Layout.activity_photo_galery);

			_layoutManager = new GridAutofitLayoutManager(this, 150);
			_recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view_photo_galery);
			_recyclerView.SetLayoutManager(_layoutManager);

			_galery = new PhotoGalery(this);
			await _galery.LoadImages();
			_adapter = new PhotoGaleryAdapter(_galery);
			_recyclerView.SetAdapter(_adapter);
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
			
		public class PhotoGaleryAdapter : RecyclerView.Adapter
		{
			
			private PhotoGalery _galery;

			public PhotoGaleryAdapter(PhotoGalery galery)
			{
				this._galery = galery;
			}

			#region implemented abstract members of Adapter

			static int create = 0;
			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				Console.WriteLine("CREATE VIEW HOLDER - {0}", ++create);

				var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.grid_cell_photo_galery_item, null);
				var holder = new ImageViewHolder(view);
				return holder;
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				var cell = (ImageViewHolder)holder;
				cell.ImageView.Id = position;
				cell.FetchImage = (token) => _galery.GetImageAtIndex(position, token);
				try
				{					
					cell.LoadBitmap();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			public override int ItemCount
			{
				get
				{
					return this._galery.Count;
				}
			}

			#endregion

			class ImageViewHolder : RecyclerView.ViewHolder
			{
				#region Private fields and properties

				static int count = 0;
				private CancellationTokenSource _cancellationToken;
				private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

				#endregion

				#region Constructors

				public ImageViewHolder(View parent)
					:base(parent)
				{
					this.ImageView = parent.FindViewById<ImageView>(Resource.Id.thumbImage);
				}

				#endregion

				#region Properties

				public ImageView ImageView { get; set; }

				public Func<CancellationToken, Task<Bitmap>> FetchImage { get; set; }

				#endregion

				public async void LoadBitmap()
				{
					//If we had already running task on this view, cancel it.
					if (_cancellationToken != null)
					{
						_cancellationToken.Cancel();
					}

					//Create new cancellation token
					_cancellationToken = new CancellationTokenSource();

					//Set the image to null
					setImage(null, _cancellationToken.Token);

					Bitmap image = null;
					try
					{
						image = await FetchImage(_cancellationToken.Token);
						//If the task is cancelled dispose the image and return
						if (_cancellationToken.Token.IsCancellationRequested && image != null)
						{
							image.Recycle();
							return;
						}
						else
						{
							//Set new image
							setImage(image, _cancellationToken.Token);
						}

					}
					catch (Exception ex)
					{
					}
				}

				private void setImage(Bitmap bitmap, CancellationToken token)
				{
					using (var writerLock = _locker.WriterLock(token))
					{
						if (ImageView == null)
							return;
						if (ImageView.Drawable != null)
						{
							var old = ((BitmapDrawable)ImageView.Drawable).Bitmap;
							if (old != null && !old.IsRecycled)
								old.Recycle();
						}
						ImageView.SetImageBitmap(bitmap);
					}
				}
			}
		}
	}
}

