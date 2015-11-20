using System;
using Android.Graphics;
using XLabs.Platform.Services.Media;
using ExifLib;
using System.IO;
using System.Drawing;
using Android.Views;

namespace Tojeero.Droid
{
	public static class BitmapToolbox
	{
		public static Bitmap RotateToCorrentOrientation(this Bitmap bitmap, ExifOrientation currentOrientation)
		{
			//Calculate rotation
			float degrees = 0;
			switch (currentOrientation)
			{
				case ExifOrientation.TopRight:
					degrees = 90;
					break;
				case ExifOrientation.BottomLeft:
					degrees = -90;
					break;
				case ExifOrientation.BottomRight:
					degrees = 180;
					break;
			}

			//Rotate if needed
			if (degrees != 0)
			{
				using (Matrix mtx = new Matrix())
				{
					mtx.PreRotate(degrees);
					bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, mtx, false);
				}
			}
			return bitmap;
		}

		public static Bitmap RotateToCorrentOrientation(this Bitmap bitmap, SurfaceOrientation currentOrientation)
		{
			//Calculate rotation
			float degrees = 0;
			switch (currentOrientation)
			{
				case SurfaceOrientation.Rotation180:
					degrees = -180;
					break;
				case SurfaceOrientation.Rotation270:
					degrees = 90;
					break;
				case SurfaceOrientation.Rotation90:
					degrees = -90;
					break;
			}

			//Rotate if needed
			if (degrees != 0)
			{
				using (Matrix mtx = new Matrix())
				{
					mtx.PreRotate(degrees);
					bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, mtx, false);
				}
			}
			return bitmap;
		}

		public static byte[] GetRawBytes(this Bitmap bitmap, Bitmap.CompressFormat format)
		{
			//Convert bitmap to byte array
			MemoryStream stream = new MemoryStream();
			bitmap.Compress(format, 100, stream);
			byte[] byteArray = stream.ToArray();
			return byteArray;
		}

		public static Bitmap GetBitmap(this byte[] rawBytes)
		{
			Bitmap bitmap = BitmapFactory.DecodeByteArray (rawBytes, 0, rawBytes.Length);
			return bitmap;
		}

		public static Bitmap ScaleBitmap(this Bitmap bitmap, SizeF size)
		{
			Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, (int)size.Width, (int)size.Height, false);
			return resizedBitmap;
		}

		public static Bitmap GetScaledAndRotatedBitmap(this Bitmap bitmap, ExifOrientation currentOrientation, float maxPixelDimension)
		{
			var newBitmapSize = getScaledSize(bitmap.Width, bitmap.Height, maxPixelDimension);
			if (newBitmapSize != SizeF.Empty)
			{
				var scaled = bitmap.ScaleBitmap(newBitmapSize);
				var rotated = scaled.RotateToCorrentOrientation(currentOrientation);
				if (rotated != scaled)
					scaled.Dispose();
				return rotated;
			}
			else
			{
				return bitmap.RotateToCorrentOrientation(currentOrientation);
			}
		}

		public static Bitmap GetScaledAndRotatedBitmap(this Bitmap bitmap, SurfaceOrientation currentOrientation, float maxPixelDimension)
		{
			var newBitmapSize = getScaledSize(bitmap.Width, bitmap.Height, maxPixelDimension);
			if (newBitmapSize != SizeF.Empty)
			{
				var scaled = bitmap.ScaleBitmap(newBitmapSize);
				var rotated = scaled.RotateToCorrentOrientation(currentOrientation);
				if (rotated != scaled)
					scaled.Dispose();
				return rotated;
			}
			else
			{
				return bitmap.RotateToCorrentOrientation(currentOrientation);
			}
		}

		private static SizeF getScaledSize(float currentWidth, float currentHeight, float maxPixelDimension)
		{
			var size = SizeF.Empty;
			if (currentWidth > 0 && currentHeight > 0 && maxPixelDimension > 0)
			{
				double ratio;
				if (currentWidth > currentHeight)
				{
					ratio = (maxPixelDimension) / ((double)currentWidth);
				}
				else
				{
					ratio = (maxPixelDimension) / ((double)currentHeight);
				}

				var width = (int)Math.Round(ratio * currentWidth);
				var height = (int)Math.Round(ratio * currentHeight);
				size = new SizeF(width, height);
			}

			return size;
		}
	}
}

