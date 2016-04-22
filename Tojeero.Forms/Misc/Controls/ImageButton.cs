using System.Windows.Input;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class ImageButton : View
    {
        #region Constructors

        #endregion

        #region Properties

        public static BindableProperty CommandProperty = BindableProperty.Create<ImageButton, ICommand>(o => o.Command,
            null);

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static BindableProperty ImageProperty = BindableProperty.Create<ImageButton, ImageSource>(o => o.Image,
            null);

        public ImageSource Image
        {
            get { return (ImageSource) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static BindableProperty BackgroundImageProperty =
            BindableProperty.Create<ImageButton, ImageSource>(o => o.BackgroundImage, null);

        public ImageSource BackgroundImage
        {
            get { return (ImageSource) GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public static BindableProperty SelectedImageProperty =
            BindableProperty.Create<ImageButton, ImageSource>(o => o.SelectedImage, null);

        public ImageSource SelectedImage
        {
            get { return (ImageSource) GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        public static BindableProperty IsSelectedProperty = BindableProperty.Create<ImageButton, bool>(
            o => o.IsSelected, false, BindingMode.OneWay);

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion
    }
}