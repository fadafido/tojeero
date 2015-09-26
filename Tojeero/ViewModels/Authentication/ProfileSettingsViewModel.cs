﻿using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core.ViewModels
{
	public class ProfileSettingsViewModel : BaseUserDetailsViewModel
	{
		#region Private Fields and Properties

		CancellationTokenSource _tokenSource;
		CancellationToken _token;
		private List<string> _countryList = new List<string> {"Armenia", "United Arab Emirates"};
		private Dictionary<string, List<string>> _countries;

		#endregion

		#region Constructors

		public ProfileSettingsViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
			PropertyChanged += propertyChanged;
			_countries = new Dictionary<string, List<string>>
			{
				[_countryList[0]] = new List<string> {"Yerevan", "Armavir", "Kapan", "Goris", "Sisian"},
				[_countryList[1]] = new List<string> {"Dubai", "Abu Dhabi", "Sharjah", "Al Ain", "Ajman"}
			};
		}

		public void Init(bool userShouldProvideProfileDetails)
		{
			Hint = userShouldProvideProfileDetails ? "Please provide some details about you to improve your app experience." : null;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> Close;

		public string Title
		{
			get
			{
				return "Profile Settings";
			}
		}

		private string _hint;
		public string Hint
		{ 
			get
			{
				return _hint; 
			}
			set
			{
				_hint = value; 
				RaisePropertyChanged(() => Hint); 
			}
		}

		public List<string> Countries
		{
			get
			{
				return this._countryList;
			}
		}

		private List<string> _cities;
		public List<string> Cities
		{ 
			get
			{
				return _cities; 
			}
			private set
			{
				_cities = value; 
				RaisePropertyChanged(() => Cities); 
			}
		}

//		private int _selectedCountry;
//		public int SelectedCountry
//		{ 
//			get
//			{
//				return _selectedCountry; 
//			}
//			set
//			{
//				_selectedCountry = value; 
//				this.Country = value >= 0 && value < _countryList.Count ? this._countryList[value] : null;
//				RaisePropertyChanged(() => SelectedCountry); 
//			}
//		}
//
//		private int _selectedCity;
//		public int SelectedCity
//		{ 
//			get
//			{
//				return _selectedCity; 
//			}
//			set
//			{
//				_selectedCity = value; 
//				this.City = Cities != null && value >= 0 && value < Cities.Count ? this.Cities[value] : null;
//				RaisePropertyChanged(() => SelectedCity); 
//			}
//		}
		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _submitCommand;
		public System.Windows.Input.ICommand SubmitCommand
		{
			get
			{
				_submitCommand = _submitCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await submit(), () => !IsLoading);
				return _submitCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _cancelCommand;
		public System.Windows.Input.ICommand CancelCommand
		{
			get
			{
				_cancelCommand = _cancelCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => cancel());
				return _cancelCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task submit()
		{
			this.IsLoading = true;
			using (_tokenSource = new CancellationTokenSource())
			{
				_token = _tokenSource.Token;
				try
				{
					var user = getUpdatedUser();
					await _authService.UpdateUserDetails(user, _token);
				}
				catch (Exception ex)
				{
					Mvx.Trace(MvxTraceLevel.Error, "Error occured while saving user details. {0}", ex.ToString());
				}

				this.IsLoading = false;
				this.Close.Fire(this, new EventArgs());
			}
		}

		private void cancel()
		{
			//If we have a token source it mean that submit process has already started
			//so we need to cancel it and the view will be closed inside submit method
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
			}
			//Otherwise just close the view
			else
			{
				this.Close.Fire(this, new EventArgs());
			}
		}

		private User getUpdatedUser()
		{
			var user = new User();
			user.FirstName = this.FirstName;
			user.LastName = this.LastName;
			user.Country = this.Country;
			user.City = this.City;
			user.Mobile = this.Mobile;
			return user;
		}


		void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Country")
			{
				var country = this.Country ?? "";
				if (this._countries.ContainsKey(country))
				{
					this.Cities = _countries[country];
					this.City = this.Cities.Count > 0 ? this.Cities[0] : null;
				}
			}
		}
		#endregion

	}
}

