using System.Windows.Input;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
	public class SelectableButton : Image
	{
		#region Constructors

		public SelectableButton()
		{
			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) => {
				this.Command.Execute(null);
			};
			this.GestureRecognizers.Add(tapGestureRecognizer);
			this.PropertyChanged += propertyChanged;
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

		#region Command

		public static BindableProperty CommandProperty = BindableProperty.Create<SelectableButton, ICommand>(o => o.Command, null);

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
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

		#region Utility methods


		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsSelectedProperty.PropertyName ||
			    e.PropertyName == SelectedImageProperty.PropertyName ||
			    e.PropertyName == DeselectedImageProperty.PropertyName)
			{
				this.Source = this.IsSelected ? this.SelectedImage : this.DeselectedImage;
			}
			else if (e.PropertyName == IsEnabledProperty.PropertyName)
			{
				this.Opacity = this.IsEnabled ? 1 : 0.5;
			}
		}

		#endregion
	}
}

