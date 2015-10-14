﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tojeero.Forms.Resources;
using System.Linq;

namespace Tojeero.Core.ViewModels
{
	public class FilterProductsViewModel : LoadableNetworkViewModel
	{
		#region Private fields

		private readonly IProductCategoryManager _categoryManager;
		private readonly IProductSubcategoryManager _subcategoryManager;
		private readonly ICountryManager _countryManager;
		private readonly ICityManager _cityManager;
		private readonly ITagManager _tagManager;

		#endregion

		#region Constructors

		public FilterProductsViewModel(IProductCategoryManager categoryManager, IProductSubcategoryManager subcategoryManager, 
			ICountryManager countryManager, ICityManager cityManager, ITagManager tagManager)
			: base()
		{
			this._tagManager = tagManager;
			this._cityManager = cityManager;
			this._countryManager = countryManager;
			this._subcategoryManager = subcategoryManager;
			this._categoryManager = categoryManager;
		}

		#endregion

		#region Properties
		public IProductFilter ProductFilter
		{
			get
			{
				return RuntimeSettings.ProductFilter;
			}
		}

		private IProductCategory[] _categories;

		public IProductCategory[] Categories
		{ 
			get
			{
				return _categories; 
			}
			set
			{
				_categories = value; 
				RaisePropertyChanged(() => Categories); 
			}
		}

		private IProductSubcategory[] _subcategories;

		public IProductSubcategory[] Subcategories
		{ 
			get
			{
				return _subcategories; 
			}
			set
			{
				_subcategories = value; 
				RaisePropertyChanged(() => Subcategories); 
			}
		}

		private ICountry[] _countries;

		public ICountry[] Countries
		{ 
			get
			{
				return _countries; 
			}
			set
			{
				_countries = value; 
				RaisePropertyChanged(() => Countries); 
			}
		}

		private ICity[] _cities;

		public ICity[] Cities
		{ 
			get
			{
				return _cities; 
			}
			set
			{
				_cities = value; 
				RaisePropertyChanged(() => Cities); 
			}
		}

		private ObservableCollection<ITag> _tags;

		public ObservableCollection<ITag> Tags
		{ 
			get
			{
				return _tags; 
			}
			set
			{
				_tags = value; 
				RaisePropertyChanged(() => Tags); 
			}
		}
		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						
					}, () => !IsLoading);
				return _reloadCommand;
			}
		}

		#endregion

		#region Protected API

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			if (e.IsConnected)
			{
				this.ReloadCommand.Execute(null);
			}
		}

		#endregion

		#region Utility methods

		private async Task reload()
		{
			StartLoading("Loading data...");
			string failureMessage = null;
			try
			{

				await loadCountries();
				await loadCities();
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading(AppResources.MessageLoadingTimeOut);
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while loading data in Product filter screen.", LoggingLevel.Error, true);
				StopLoading(AppResources.MessageLoadingFailed);
			}
			StopLoading(failureMessage);
		}

		private async Task loadCategories()
		{
			this.Categories = (await _categoryManager.Fetch()).ToArray();
		}

		private async Task loadSubcategories()
		{
			this.Subcategories = this.ProductFilter.Category != null ? (await _subcategoryManager.Fetch(this.ProductFilter.Category.ID)).ToArray() : null;
		}

		private async Task loadCountries()
		{
			this.Countries = (await _countryManager.Fetch()).ToArray();
		}

		private async Task loadCities()
		{
			this.Cities = this.ProductFilter.Country != null ? (await _cityManager.Fetch(this.ProductFilter.Country.CountryId)).ToArray() : null;
		}

		#endregion
	}
}

