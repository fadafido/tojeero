using System.Windows.Input;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public partial class RemovableContainerView : Grid
    {
        #region Constructors

        public RemovableContainerView()
        {
            InitializeComponent();
            setupRootContent();
            removeButton.IsVisible = false;
        }

        #endregion

        #region Properties

        private View _rootContent;

        public View RootContent
        {
            get { return _rootContent; }
            set
            {
                if (_rootContent != value)
                {
                    _rootContent = value;
                    setupRootContent();
                }
            }
        }

        public static BindableProperty RemoveEnabledProperty =
            BindableProperty.Create<RemovableContainerView, bool>(o => o.RemoveEnabled, false,
                propertyChanged: OnRemoveEnabledChanged);

        public bool RemoveEnabled
        {
            get { return (bool) GetValue(RemoveEnabledProperty); }
            set { SetValue(RemoveEnabledProperty, value); }
        }

        private static void OnRemoveEnabledChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var control = (RemovableContainerView) bindable;
            control.removeButton.IsVisible = newvalue;
        }

        public static BindableProperty RemoveCommandProperty =
            BindableProperty.Create<RemovableContainerView, ICommand>(o => o.RemoveCommand, null,
                propertyChanged: OnRemoveCommandChanged);

        public ICommand RemoveCommand
        {
            get { return (ICommand) GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        private static void OnRemoveCommandChanged(BindableObject bindable, ICommand oldvalue, ICommand newvalue)
        {
            var control = (RemovableContainerView) bindable;
            control.removeButton.Command = newvalue;
        }

        #endregion

        #region Utility methods

        void setupRootContent()
        {
            if (rootContainer != null)
            {
                rootContainer.Children.Clear();
                if (RootContent != null)
                    rootContainer.Children.Add(RootContent);
            }
        }

        #endregion
    }
}