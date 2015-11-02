using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Toolbox;

namespace Tojeero.Forms
{
	public class FavoritesViewModel : MvxViewModel
	{
		#region Constructors

		public FavoritesViewModel()
		{
		}

		#endregion

		#region Properties

		public Action ShowFavoriteProductsAction;

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showFavoriteProductsCommand;

		public System.Windows.Input.ICommand ShowFavoriteProductsCommand
		{
			get
			{
				_showFavoriteProductsCommand = _showFavoriteProductsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowFavoriteProductsAction.Fire();
				});
				return _showFavoriteProductsCommand;
			}
		}

		#endregion
	}
}

