using System;

namespace Tojeero.Forms
{
	public class Label : Xamarin.Forms.Label
	{
		#region Constructors

		public Label()
			: base()
		{
		}

		#endregion

		#region Properties

		#region Line count

		public static Xamarin.Forms.BindableProperty LineCountProperty = Xamarin.Forms.BindableProperty.Create<Label, int>(o => o.LineCount, 0);

		public int LineCount
		{
			get { return (int)GetValue(LineCountProperty); }
			set { SetValue(LineCountProperty, value); }
		}

		#endregion

		#endregion
	}
}

