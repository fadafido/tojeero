using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class FacetObjectPicker<T> : ObjectPicker<FacetViewModel<T>, FacetPickerCell>
        where T : class, IUniqueEntity
    {
        #region Private fields and properties

        private Dictionary<string, int> _facets;

        #endregion

        #region Constructors

        public FacetObjectPicker()
        {
            Comparer = (x, y) =>
            {
                if (x == null || y == null || x.Data == null || y.Data == null)
                    return false;
                return x == y || x.Data == y.Data || x.Data.ID == y.Data.ID;
            };
            ItemCaption = x => x == null || x.Data == null ? "" : x.Data.ToString();
            ItemsLoader = async () =>
            {
                var facets = FacetsLoader == null ? null : await FacetsLoader();
                var objects = ObjectsLoader == null ? null : await ObjectsLoader();
                if (facets == null || objects == null)
                    return null;
                var facetedObjects = objects.ApplyFacets(facets, CountVisible).ToList();
                return facetedObjects;
            };
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        /***************Facets***************/
        public Func<Task<Dictionary<string, int>>> FacetsLoader { get; set; }


        /***************Objects***************/
        public Func<Task<IList<T>>> ObjectsLoader { get; set; }

        /***************Count Visible***************/
        public bool CountVisible { get; set; } = true;

        /***************SelectedObject***************/

        public static BindableProperty SelectedObjectProperty =
            BindableProperty.Create<FacetObjectPicker<T>, T>(o => o.SelectedObject, null);

        public T SelectedObject
        {
            get { return (T) GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        #endregion

        #region Utility methods

        void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SelectedItemProperty.PropertyName)
            {
                SelectedObject = SelectedItem != null ? SelectedItem.Data : null;
            }
            if (e.PropertyName == SelectedObjectProperty.PropertyName)
            {
                SelectedItem = SelectedObject != null ? new FacetViewModel<T>(SelectedObject) : null;
            }
        }

        #endregion
    }
}