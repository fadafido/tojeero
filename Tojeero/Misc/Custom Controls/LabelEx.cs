using System;

namespace Tojeero.Forms
{
	public class LabelEx : Xamarin.Forms.Label
	{
		#region Constructors

		public LabelEx()
			: base()
		{
		}

		#endregion

		#region Properties

		#region Line count

		public static Xamarin.Forms.BindableProperty LineCountProperty = Xamarin.Forms.BindableProperty.Create<LabelEx, int>(o => o.LineCount, 0);

		public int LineCount
		{
			get { return (int)GetValue(LineCountProperty); }
			set { SetValue(LineCountProperty, value); }
		}

		#endregion

		#endregion
	}
}

