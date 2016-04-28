using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Contracts;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public abstract class BasePage<T> : ContentPage
        where T : IBaseViewModel
    {
        #region Constructors

        protected BasePage()
            :base()
        {
            UpdateDialogAction();
        }

        #endregion

        #region Properties
        public virtual T ViewModel  
        {
            get
            {
                return (T) BindingContext;
            }
            protected set
            {
                BindingContext = value;
                UpdateDialogAction();
                SetupViewModel();
            }
        }

        #endregion

        #region Protected API

        protected virtual void SetupViewModel()
        {

        }

        #endregion

        #region Parent override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            ViewModel?.OnStarting();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel?.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel?.OnDisappearing();
            base.OnDisappearing();
        }

        #endregion

        #region Utility methods

        private void UpdateDialogAction()
        {
            if(ViewModel == null)
                return;
            ViewModel.ShowDialogAction = async (title, content, negativeTitle, positiveTitle, negativeAction, positiveAction) =>
            {
                if (negativeTitle == null)
                    return;
                if (positiveTitle == null)
                {
                    await DisplayAlert(title, content, negativeTitle);
                    negativeAction?.Invoke();
                }
                else
                {
                    var result = await DisplayAlert(title, content, positiveTitle, negativeTitle);
                    if (result)
                        positiveAction?.Invoke();
                    else
                        negativeAction?.Invoke();
                }
            };
        }

        #endregion
    }
}
