using Tojeero.Core.ViewModels.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    [ContentProperty("RootContent")]
    public class BaseLoadablePage<T> : BasePage<T>
        where T : BaseLoadableViewModel
    {
        #region Private fields

        private readonly ContentView _rootContent;
        private readonly LoadingOverlay _loadingOverlay;

        #endregion

        #region Constructors

        protected BaseLoadablePage()
            :base()
        {
            Grid grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            _rootContent = new ContentView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            grid.Children.Add(_rootContent);
            _loadingOverlay = new LoadingOverlay()
            {
                IsVisible = false
            };
            grid.Children.Add(_loadingOverlay);
            Content = grid;
            this.SetBinding(LoadingTitleProperty, "LoadingText");
            this.SetBinding(IsLoadingProperty, "IsLoading");
        }

        #endregion

        #region Properties

        public View RootContent
        {
            get { return _rootContent.Content; }
            set { _rootContent.Content = value; }
        }

        #region IsLoading

        public static BindableProperty IsLoadingProperty = BindableProperty.Create<BaseLoadablePage<T>, bool>(o => o.IsLoading,
            false,
            propertyChanged: OnIsLoadingChanged);

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        private static void OnIsLoadingChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var page = bindable as BaseLoadablePage<T>;
            page._loadingOverlay.IsVisible = newvalue;
        }

        #endregion

        #region LoadingTitle

        public static BindableProperty LoadingTitleProperty = BindableProperty.Create<BaseLoadablePage<T>, string>(o => o.LoadingTitle, "",
            propertyChanged: OnLoadingTitleChanged);

        public string LoadingTitle
        {
            get { return (string)GetValue(LoadingTitleProperty); }
            set { SetValue(LoadingTitleProperty, value); }
        }

        private static void OnLoadingTitleChanged(BindableObject bindable, string oldvalue, string newvalue)
        {
            var page = bindable as BaseLoadablePage<T>;
            page._loadingOverlay.Title = newvalue;
        }

        #endregion

        #endregion
    }
}
