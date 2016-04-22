using Tojeero.Core.ViewModels.Contracts;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
	public partial class FavoriteSocialView : StackLayout
	{
		#region Constructors

		public FavoriteSocialView()
		{			
			InitializeComponent();
		}

		#endregion

		#region Social view model

		public static BindableProperty ViewModelProperty = BindableProperty.Create<FavoriteSocialView, ISocialViewModel>(o => o.ViewModel, null, propertyChanged: OnViewModelChanged);

		public ISocialViewModel ViewModel
		{
			get { return (ISocialViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		private static void OnViewModelChanged(BindableObject bindable, ISocialViewModel oldvalue, ISocialViewModel newvalue)
		{
			var view = (FavoriteSocialView)bindable;
			if(view.rootContent != null)
				view.rootContent.BindingContext = newvalue;
		}

		#endregion
	}
}

