using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
	public class BorderView : View
	{
		#region Constructors

		public BorderView()
			: base()
		{
		}

		#endregion

		#region Properties

		public static BindableProperty BorderColorProperty = BindableProperty.Create<BorderView, Color>(o => o.BorderColor, Color.Transparent);

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}


		public static BindableProperty BorderWidthProperty = BindableProperty.Create<BorderView, float>(o => o.BorderWidth, 1);

		public float BorderWidth
		{
			get { return (float)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		public static BindableProperty RadiusProperty = BindableProperty.Create<BorderView, float>(o => o.Radius, 0);

		public float Radius
		{
			get { return (float)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}


		#endregion
	}
}

