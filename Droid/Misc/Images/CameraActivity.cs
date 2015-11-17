using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content.PM;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Droid
{
	[Activity(Label = "CameraActivity",
		Icon = "@drawable/icon", 
		Theme="@style/Theme.Tojeero",  
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation=ScreenOrientation.Portrait)]
	public class CameraActivity : Activity, TextureView.ISurfaceTextureListener
	{
		#region Private fields and properties

		private global::Android.Hardware.Camera camera;
		private global::Android.Widget.Button takePhotoButton;
		private global::Android.Widget.Button toggleFlashButton;
		private global::Android.Widget.Button switchCameraButton;

		private CameraFacing cameraType;
		private TextureView textureView;
		private SurfaceTexture surfaceTexture;

		private bool _flashOn;

		private static string KEY_RECEIVER_ID = "RECEIVER_ID";
		private Guid _receiverID;
		private Bitmap _selectedImage;

		#endregion

		#region Constructors

		public CameraActivity()
		{

		}

		#endregion
			
		#region View lifecycle management

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			if (this.Intent != null)
			{
				var receiverID = this.Intent.GetStringExtra(KEY_RECEIVER_ID);
				if(receiverID != null)
					_receiverID = new Guid(receiverID);
			}

			this.SetContentView(Resource.Layout.activity_camera);

			try
			{
				
				cameraType = CameraFacing.Back;

				textureView = this.FindViewById<TextureView>(Resource.Id.textureView);
				textureView.SurfaceTextureListener = this;

				takePhotoButton = this.FindViewById<global::Android.Widget.Button>(Resource.Id.takePhotoButton);
				takePhotoButton.Click += TakePhotoButtonTapped;

				switchCameraButton = this.FindViewById<global::Android.Widget.Button>(Resource.Id.switchCameraButton);
				switchCameraButton.Click += SwitchCameraButtonTapped;

				toggleFlashButton = this.FindViewById<global::Android.Widget.Button>(Resource.Id.toggleFlashButton);
				toggleFlashButton.Click += ToggleFlashButtonTapped;

			}
			catch (Exception ex)
			{
				Xamarin.Insights.Report(ex);
			}
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			publishSelectedImage();
		}

		#endregion

		#region ISurface

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			camera = global::Android.Hardware.Camera.Open((int)cameraType);
			//textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
			surfaceTexture = surface;

			camera.SetPreviewTexture(surface);
			PrepareAndStartCamera();
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			camera.StopPreview();
			camera.Release();

			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			PrepareAndStartCamera();
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{

		}

		#endregion

		#region Public API

		public static Intent GetIntentForReceiver(Context context, Guid receiver)
		{
			var intent = new Intent(context, typeof(CameraActivity));
			intent.PutExtra(KEY_RECEIVER_ID, receiver.ToString());
			return intent;
		}

		#endregion

		#region Utility methods

		private void PrepareAndStartCamera()
		{
			camera.StopPreview();

			var display = this.WindowManager.DefaultDisplay;
			if (display.Rotation == SurfaceOrientation.Rotation0)
			{
				camera.SetDisplayOrientation(90);
			}

			if (display.Rotation == SurfaceOrientation.Rotation270)
			{
				camera.SetDisplayOrientation(180);
			}

			camera.StartPreview();
		}

		private void SwitchCameraButtonTapped(object sender, EventArgs e)
		{
			if (cameraType == CameraFacing.Front)
			{
				cameraType = CameraFacing.Back;

				camera.StopPreview();
				camera.Release();
				camera = global::Android.Hardware.Camera.Open((int)cameraType);
				camera.SetPreviewTexture(surfaceTexture);
				PrepareAndStartCamera();
			}
			else
			{
				cameraType = CameraFacing.Front;

				camera.StopPreview();
				camera.Release();
				camera = global::Android.Hardware.Camera.Open((int)cameraType);
				camera.SetPreviewTexture(surfaceTexture);
				PrepareAndStartCamera();
			}
		}

		private void ToggleFlashButtonTapped(object sender, EventArgs e)
		{
			_flashOn = !_flashOn;
			if (_flashOn)
			{
				if (cameraType == CameraFacing.Back)
				{
					toggleFlashButton.SetBackgroundResource(Resource.Drawable.FlashButton);
					cameraType = CameraFacing.Back;

					camera.StopPreview();
					camera.Release();
					camera = global::Android.Hardware.Camera.Open((int)cameraType);
					var parameters = camera.GetParameters();
					parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeTorch;
					camera.SetParameters(parameters);
					camera.SetPreviewTexture(surfaceTexture);
					PrepareAndStartCamera();
				}
			}
			else
			{
				toggleFlashButton.SetBackgroundResource(Resource.Drawable.NoFlashButton);
				camera.StopPreview();
				camera.Release();

				camera = global::Android.Hardware.Camera.Open((int)cameraType);
				var parameters = camera.GetParameters();
				parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeOff;
				camera.SetParameters(parameters);
				camera.SetPreviewTexture(surfaceTexture);
				PrepareAndStartCamera();
			}
		}

		private async void TakePhotoButtonTapped(object sender, EventArgs e)
		{
			camera.StopPreview();

			var image = textureView.Bitmap;
			_selectedImage = image.GetScaledAndRotatedBitmap(SurfaceOrientation.Rotation0, 1000);
			Finish();
			publishSelectedImage();
		}

		public void publishSelectedImage()
		{
			var messenger = Mvx.Resolve<IMvxMessenger>();
			messenger.Publish<CameraImageSelectedMessage>(new CameraImageSelectedMessage(this, _receiverID, _selectedImage));
		}

		#endregion
	}
}