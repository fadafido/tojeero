using Tojeero.Core;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{		
	public class TabButton : Button
	{
		public TabButton()
		{
			
		}

		#region Properties

		public static BindableProperty IsSelectedProperty = BindableProperty.Create<TabButton, bool>(o => o.IsSelected, false);

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set {
				Tools.Logger.Log("SETTING {0} IsSelected: {1}", this.Text, value);
				SetValue(IsSelectedProperty, value); 
				Tools.Logger.Log("IS SET {0} IsSelected: {1}", this.Text, this.IsSelected);
			}
		}

		#endregion
	}
}

