using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.User
{
    public partial class TermsPage : ContentPage
    {
        #region Constructors

        public TermsPage()
        {
            ViewModel = MvxToolbox.LoadViewModel<TermsViewModel>();
            InitializeComponent();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                async () => { await Navigation.PopModalAsync(); }));
        }

        #endregion

        #region Properties

        private TermsViewModel _viewModel;

        public TermsViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                BindingContext = _viewModel;
            }
        }

        #endregion
    }
}