using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System.ComponentModel;
using Android.Graphics;


[assembly:ExportRenderer(typeof(BorderView), typeof(Tojeero.Droid.Renderers.BorderViewRenderer))]

namespace Tojeero.Droid.Renderers
{
	public class BorderViewRenderer : VisualElementRenderer<BorderView>
	{
		//
		// Methods
		//
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.Background.Dispose();
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BorderView> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null && e.OldElement == null)
			{
				this.UpdateBackground();
			}
		}

		private void UpdateBackground()
		{			
			this.SetBackgroundDrawable(new BorderViewRenderer.BorderViewDrawable(base.Element));
		}

		//
		// Nested Types
		//
		private class BorderViewDrawable : Drawable
		{
			BorderView borderView;
			Bitmap normalBitmap;
			bool isDisposed;

			public override bool IsStateful
			{
				get
				{
					return false;
				}
			}

			public override int Opacity
			{
				get
				{
					return 0;
				}
			}

			public BorderViewDrawable(BorderView borderView)
			{
				this.borderView = borderView;
				borderView.PropertyChanged += new PropertyChangedEventHandler(this.FrameOnPropertyChanged);
			}

			private void FrameOnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == Frame.OutlineColorProperty.PropertyName)
				{
					this.InvalidateSelf();
				}
			}

			protected override bool OnStateChange(int[] state)
			{
				return false;
			}

			public override void Draw(Canvas canvas)
			{
				int num = base.Bounds.Width();
				int num2 = base.Bounds.Height();
				if (num <= 0 || num2 <= 0)
				{
					if (this.normalBitmap != null)
					{
						this.normalBitmap.Dispose();
						this.normalBitmap = null;
					}
					return;
				}
				if (this.normalBitmap == null || this.normalBitmap.Height != num2 || this.normalBitmap.Width != num)
				{
					if (this.normalBitmap != null)
					{
						this.normalBitmap.Dispose();
						this.normalBitmap = null;
					}
					this.normalBitmap = this.CreateBitmap(false, num, num2);
				}
				Bitmap bitmap = this.normalBitmap;
				using (Paint paint = new Paint())
				{
					canvas.DrawBitmap(bitmap, 0, 0, paint);
				}
			}

			private Bitmap CreateBitmap(bool pressed, int width, int height)
			{
				Bitmap bitmap;
				using (Bitmap.Config argb = Bitmap.Config.Argb8888)
				{
					bitmap = Bitmap.CreateBitmap(width, height, argb);
				}
				using (Canvas canvas = new Canvas(bitmap))
				{
					this.DrawBackground(canvas, width, height, pressed);
					this.DrawOutline(canvas, width, height);
				}
				return bitmap;
			}

			private void DrawOutline(Canvas canvas, int width, int height)
			{
				using (Paint paint = new Paint {
					AntiAlias = true
				})
				{
					using (Path path = new Path())
					{
						using (Path.Direction cw = Path.Direction.Cw)
						{
							using (Paint.Style stroke = Paint.Style.Stroke)
							{
								using (RectF rectF = new RectF(0, 0, (float)width, (float)height))
								{
									path.AddRect(rectF, cw);
									paint.StrokeWidth = Xamarin.Forms.Forms.Context.ToPixels(this.borderView.BorderWidth);
									paint.SetStyle(stroke);
									paint.Color = this.borderView.BorderColor.ToAndroid();
									canvas.DrawPath(path, paint);
								}
							}
						}
					}
				}
			}

			private void DrawBackground(Canvas canvas, int width, int height, bool pressed)
			{
				using (Paint paint = new Paint {
					AntiAlias = true
				})
				{
					using (Path path = new Path())
					{
						using (Path.Direction cw = Path.Direction.Cw)
						{
							using (Paint.Style fill = Paint.Style.Fill)
							{
								using (RectF rectF = new RectF(0, 0, (float)width, (float)height))
								{
									path.AddRect(rectF, cw);
									var color = this.borderView.BackgroundColor.ToAndroid();
									paint.SetStyle(fill);
									paint.Color = color;
									canvas.DrawPath(path, paint);
								}
							}
						}
					}
				}
			}

			public override void SetAlpha(int alpha)
			{
			}

			public override void SetColorFilter(ColorFilter cf)
			{
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !this.isDisposed)
				{
					if (this.normalBitmap != null)
					{
						this.normalBitmap.Dispose();
						this.normalBitmap = null;
					}
					this.isDisposed = true;
				}
				base.Dispose(disposing);
			}
		}
	}
}

