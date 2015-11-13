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

namespace Tojeero.Droid
{
	[Activity(Label = "CameraActivity",Icon = "@drawable/icon", Theme="@style/Theme.Tojeero",  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class CameraActivity : Activity, TextureView.ISurfaceTextureListener
	{
		global::Android.Hardware.Camera camera;
		global::Android.Widget.Button takePhotoButton;
		global::Android.Widget.Button toggleFlashButton;
		global::Android.Widget.Button switchCameraButton;

		CameraFacing cameraType;
		TextureView textureView;
		SurfaceTexture surfaceTexture;

		bool flashOn;

		byte[] imageBytes;

		public CameraActivity ()
		{

		}
			
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			this.SetContentView(Resource.Layout.activity_camera);

			try {
				
				cameraType = CameraFacing.Back;

				textureView = this.FindViewById<TextureView> (Resource.Id.textureView);
				textureView.SurfaceTextureListener = this;

				takePhotoButton = this.FindViewById<global::Android.Widget.Button> (Resource.Id.takePhotoButton);
				takePhotoButton.Click += TakePhotoButtonTapped;

				switchCameraButton = this.FindViewById<global::Android.Widget.Button> (Resource.Id.switchCameraButton);
				switchCameraButton.Click += SwitchCameraButtonTapped;

				toggleFlashButton = this.FindViewById<global::Android.Widget.Button> (Resource.Id.toggleFlashButton);
				toggleFlashButton.Click += ToggleFlashButtonTapped;

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public void OnSurfaceTextureAvailable (SurfaceTexture surface, int width, int height)
		{
			camera = global::Android.Hardware.Camera.Open ((int) cameraType);
			textureView.LayoutParameters = new FrameLayout.LayoutParams (width, height);
			surfaceTexture = surface;

			camera.SetPreviewTexture (surface);
			PrepareAndStartCamera ();
		}

		public bool OnSurfaceTextureDestroyed (SurfaceTexture surface)
		{
			camera.StopPreview ();
			camera.Release ();

			return true;
		}

		public void OnSurfaceTextureSizeChanged (SurfaceTexture surface, int width, int height)
		{
			PrepareAndStartCamera ();
		}

		public void OnSurfaceTextureUpdated (SurfaceTexture surface)
		{

		}

		private void PrepareAndStartCamera ()
		{
			camera.StopPreview ();

			var display = this.WindowManager.DefaultDisplay;
			if (display.Rotation == SurfaceOrientation.Rotation0) {
				camera.SetDisplayOrientation (90);
			}

			if (display.Rotation == SurfaceOrientation.Rotation270) {
				camera.SetDisplayOrientation (180);
			}

			camera.StartPreview ();
		}

		private void SwitchCameraButtonTapped (object sender, EventArgs e)
		{
			if (cameraType == CameraFacing.Front) {
				cameraType = CameraFacing.Back;

				camera.StopPreview ();
				camera.Release ();
				camera = global::Android.Hardware.Camera.Open ((int) cameraType);
				camera.SetPreviewTexture (surfaceTexture);
				PrepareAndStartCamera ();
			} else {
				cameraType = CameraFacing.Front;

				camera.StopPreview ();
				camera.Release ();
				camera = global::Android.Hardware.Camera.Open ((int) cameraType);
				camera.SetPreviewTexture (surfaceTexture);
				PrepareAndStartCamera ();
			}
		}

		private void ToggleFlashButtonTapped (object sender, EventArgs e)
		{
			flashOn = !flashOn;
			if (flashOn) {
				if (cameraType == CameraFacing.Back) {
					toggleFlashButton.SetBackgroundResource (Resource.Drawable.FlashButton);
					cameraType = CameraFacing.Back;

					camera.StopPreview ();
					camera.Release ();
					camera = global::Android.Hardware.Camera.Open ((int) cameraType);
					var parameters = camera.GetParameters ();
					parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeTorch;
					camera.SetParameters (parameters);
					camera.SetPreviewTexture (surfaceTexture);
					PrepareAndStartCamera ();
				}
			} else {
				toggleFlashButton.SetBackgroundResource (Resource.Drawable.NoFlashButton);
				camera.StopPreview ();
				camera.Release ();

				camera = global::Android.Hardware.Camera.Open ((int) cameraType);
				var parameters = camera.GetParameters ();
				parameters.FlashMode = global::Android.Hardware.Camera.Parameters.FlashModeOff;
				camera.SetParameters (parameters);
				camera.SetPreviewTexture (surfaceTexture);
				PrepareAndStartCamera ();
			}
		}

		private async void TakePhotoButtonTapped (object sender, EventArgs e)
		{
			camera.StopPreview ();

			var image = textureView.Bitmap;
			using (var imageStream = new MemoryStream ()) {
				await image.CompressAsync (Bitmap.CompressFormat.Jpeg, 50, imageStream);
				image.Recycle ();
				imageBytes = imageStream.ToArray ();
			}


			camera.StartPreview ();
		}
	}
}