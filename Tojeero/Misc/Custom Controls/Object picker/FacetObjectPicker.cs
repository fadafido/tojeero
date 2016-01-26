using System;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using System.Linq;

namespace Tojeero.Forms
{
	public class FacetObjectPicker<T> : ObjectPicker<FacetViewModel<T>, FacetPickerCell>
		where T : class, IUniqueEntity
	{
		#region Private fields and properties

		private Dictionary<string, int> _facets;

		#endregion

		#region Constructors

		public FacetObjectPicker()
			: base()
		{
			this.Comparer = (x, y) =>
			{
				if (x == null || y == null || x.Data == null || y.Data == null)
					return false;
				return x == y || x.Data == y.Data || x.Data.ID == y.Data.ID;
			};	
			this.ItemCaption = (x) => x == null || x.Data == null ? "" : x.Data.ToString();
			this.ItemsLoader = async () =>
			{
				var facets = this.FacetsLoader == null ? null : await this.FacetsLoader();
				var objects = this.ObjectsLoader == null ? null : await this.ObjectsLoader();
				if (facets == null || objects == null)
					return null;
				var facetedObjects = objects.ApplyFacets(facets, CountVisible).ToList();
				return facetedObjects;
			};
			this.PropertyChanged += propertyChanged;
		}


		#endregion

		#region Properties

		/***************Facets***************/
		public Func<Task<Dictionary<string,int>>> FacetsLoader { get; set; }


        /***************Objects***************/
        public Func<Task<IList<T>>> ObjectsLoader { get; set; }

        /***************Count Visible***************/
        public bool CountVisible { get; set; }

        /***************SelectedObject***************/
        public static BindableProperty SelectedObjectProperty = BindableProperty.Create<FacetObjectPicker<T>, T>(o => o.SelectedObject, null);

		public T SelectedObject
		{
			get { return (T)GetValue(SelectedObjectProperty); }
			set { SetValue(SelectedObjectProperty, value); }
		}


        #endregion

        #region Utility methods

        void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == SelectedItemProperty.PropertyName)
			{
				this.SelectedObject = this.SelectedItem != null ? this.SelectedItem.Data : null;
			}
			if (e.PropertyName == SelectedObjectProperty.PropertyName)
			{
				this.SelectedItem = this.SelectedObject != null ? new FacetViewModel<T>(this.SelectedObject) : null;
			}
		}

		#endregion
	}
}

