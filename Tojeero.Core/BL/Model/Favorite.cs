using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
	public class Favorite : MvxViewModel, IFavorite
	{
		#region Constructors

		public Favorite()
		{
		}

		public Favorite(string objectID, bool isFavorite = false)
		{
			this.ObjectID = objectID;
			this.IsFavorite = isFavorite;
		}

		#endregion

		#region Properties

		private string _objectID;

		public string ObjectID
		{ 
			get
			{
				return _objectID; 
			}
			set
			{
				_objectID = value; 
				RaisePropertyChanged(() => ObjectID); 
			}
		}
			
		private bool _isFavorite;

		public bool IsFavorite
		{ 
			get
			{
				return _isFavorite; 
			}
			set
			{
				_isFavorite = value; 
				RaisePropertyChanged(() => IsFavorite); 
			}
		}

		#endregion
	}
}

