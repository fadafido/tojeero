﻿using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Tojeero.Core.ViewModels
{
	public class BaseCollectionViewModel<T> : LoadableNetworkViewModel 
		where T : IModelEntity
	{
		#region Private fields and properties

		#endregion


		#region Constructors

		public BaseCollectionViewModel(QueryDelegate<T> query)
			: base()
		{
			_collection = new ModelEntityCollection<T>(query, 10);
		}

		#endregion

		#region Properties

		private IModelEntityCollection<T> _collection;
		public IModelEntityCollection<T> Collection
		{ 
			get
			{
				return _collection; 
			}
			set
			{
				_collection = value; 
				RaisePropertyChanged(() => Collection); 
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadNextPageCommand;
		public System.Windows.Input.ICommand LoadNextPageCommand
		{
			get
			{
				_loadNextPageCommand = _loadNextPageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await loadNextPage();
					}, () => !this.IsLoading);
				return _loadNextPageCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task loadNextPage()
		{
			this.StartLoading("Loading...");

			string failureMessage = "";
			try
			{
				await _collection.FetchNextPageAsync();
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Error);
				failureMessage = "Failed loading data.";
			}

			this.StopLoading(failureMessage);
		}

		#endregion
	}
}

