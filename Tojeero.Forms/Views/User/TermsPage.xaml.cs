using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.User
{
    public partial class TermsPage
    {
        #region Constructors

        public TermsPage()
        {
            InitializeComponent();

            ViewModel = MvxToolbox.LoadViewModel<TermsViewModel>();
            
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                async () => { await Navigation.PopModalAsync(); }));
        }

        #endregion
    }
}