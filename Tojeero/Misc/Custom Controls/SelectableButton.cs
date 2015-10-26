using System;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class SelectableButton : Button
	{
		#region Constructors

		public SelectableButton()
		{
		}

		#endregion

		#region Properties

		#region Selected image

		public static BindableProperty SelectedImageProperty = BindableProperty.Create<SelectableButton, FileImageSource>(o => o.SelectedImage, null);

		public FileImageSource SelectedImage
		{
			get { return (FileImageSource)GetValue(SelectedImageProperty); }
			set { SetValue(SelectedImageProperty, value); }
		}
			
		#endregion

		#region Deselected image

		public static BindableProperty DeselectedImageProperty = BindableProperty.Create<SelectableButton, FileImageSource>(o => o.DeselectedImage, null);

		public FileImageSource DeselectedImage
		{
			get { return (FileImageSource)GetValue(DeselectedImageProperty); }
			set { SetValue(DeselectedImageProperty, value); }
		}

		#endregion

		#region Is selected

		public static BindableProperty IsSelectedProperty = BindableProperty.Create<SelectableButton, bool>(o => o.IsSelected, false);

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		#endregion

		#endregion
	}
}

