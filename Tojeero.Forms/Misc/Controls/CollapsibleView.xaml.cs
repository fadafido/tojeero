using System;
using Tojeero.Core.Toolbox;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public partial class CollapsibleView : StackLayout
    {
        #region Constructors

        public CollapsibleView()
        {
            InitializeComponent();
            setupContentView();
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs> DidCollapse;
        public event EventHandler<EventArgs> DidOpen;

        private bool _isCollapsed = true;

        public bool IsCollapsed
        {
            get { return _isCollapsed; }
            private set
            {
                if (_isCollapsed != value)
                {
                    _isCollapsed = value;
                    setupContentView();
                }
            }
        }

        private View _collapsibleContent;

        public View CollapsibleContent
        {
            get { return _collapsibleContent; }
            set
            {
                _collapsibleContent = value;
                setupContentView();
            }
        }

        #endregion

        #region Bindable properties

        #region Title

        public static BindableProperty TitleProperty = BindableProperty.Create<CollapsibleView, string>(o => o.Title, "",
            propertyChanged: OnTitleChanged);

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void OnTitleChanged(BindableObject bindable, string oldvalue, string newvalue)
        {
            var view = (CollapsibleView) bindable;
            view.titleLabel.Text = newvalue;
        }

        #endregion

        #endregion

        #region Events

        void toggleContentView(object sender, EventArgs args)
        {
            IsCollapsed = !IsCollapsed;
            if (IsCollapsed)
                DidCollapse.Fire(this, new EventArgs());
            else
                DidOpen.Fire(this, new EventArgs());
        }

        #endregion

        #region Utility methods

        private void setupContentView()
        {
            if (contentView != null)
            {
                contentView.Content = CollapsibleContent;
                contentView.IsVisible = !IsCollapsed;
                arrowImage.Source = IsCollapsed ? "icon_arrow_down.png" : "icon_arrow_up.png";
            }
        }

        #endregion
    }
}