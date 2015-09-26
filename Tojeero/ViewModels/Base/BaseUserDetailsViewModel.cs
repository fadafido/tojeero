using System;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using Xamarin.Forms;

namespace Tojeero.Core.ViewModels
{
	public class BaseUserDetailsViewModel : BaseUserViewModel
	{
		#region Constructors

		public BaseUserDetailsViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
			PropertyChanged += propertyChanged;
			updateUserData();
		}

		#endregion

		#region Properties

		private string _firstName;
		public string FirstName
		{ 
			get
			{
				return _firstName; 
			}
			set
			{
				_firstName = value; 
				RaisePropertyChanged(() => FirstName); 
				RaisePropertyChanged(() => FullName); 
			}
		}

		private string _lastName;
		public string LastName
		{ 
			get
			{
				return _lastName; 
			}
			set
			{
				_lastName = value; 
				RaisePropertyChanged(() => LastName); 
				RaisePropertyChanged(() => FullName); 
			}
		}
			
		public string FullName
		{ 
			get
			{
				return string.Format("{0} {1}", FirstName, LastName); 
			}
		}

		private string _email;
		public string Email
		{ 
			get
			{
				return _email; 
			}
			set
			{
				_email = value; 
				RaisePropertyChanged(() => Email); 
			}
		}

		private string _profilePicture;
		public string ProfilePicture
		{ 
			get
			{
				return _profilePicture; 
			}
			set
			{
				_profilePicture = value; 
				RaisePropertyChanged(() => ProfilePicture); 
			}
		}

		private string _country;
		public string Country
		{ 
			get
			{
				return _country; 
			}
			set
			{
				_country = value; 
				RaisePropertyChanged(() => Country); 
			}
		}

		private string _city;
		public string City
		{ 
			get
			{
				return _city; 
			}
			set
			{
				_city = value; 
				RaisePropertyChanged(() => City); 
			}
		}

		private string _mobile;
		public string Mobile
		{ 
			get
			{
				return _mobile; 
			}
			set
			{
				_mobile = value; 
				RaisePropertyChanged(() => Mobile); 
			}
		}
			
		public string ProfilePicturePlaceholder
		{ 
			get
			{
				return Images.ProfilePicturePlaceholder;
			}
		}

		#endregion

		#region Utility Methods

		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentUser")
			{
				updateUserData();
			}	
		}

		void updateUserData()
		{
			var user = this.CurrentUser ?? new User();
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.ProfilePicture = user.ProfilePictureUrl;
			this.Country = user.Country;
			this.City = user.City;
			this.Mobile = user.Mobile;
		}

		#endregion
	}
}

