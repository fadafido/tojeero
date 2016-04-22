using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseFacetedCollectionViewModel<T> : LoadableNetworkViewModel where T : IUniqueEntity
    {
        #region Private fields and properties

        readonly IFacetQuery<T> _facetQuery;

        #endregion

        #region Constructors

        public BaseFacetedCollectionViewModel(IFacetQuery<T> facetQuery)
        {
            _facetQuery = facetQuery;
        }

        #endregion

        #region Properties

        private List<FacetViewModel<T>> _facets;

        public List<FacetViewModel<T>> Facets
        {
            get { return _facets; }
            private set
            {
                _facets = value;
                RaisePropertyChanged(() => Facets);
            }
        }

        #endregion

        #region Commands

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ?? new MvxCommand(async () => { await reload(); });
                return _reloadCommand;
            }
        }

        #endregion

        #region Utility methods

        public async Task reload()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            try
            {
                var items = await _facetQuery.FetchObjects();
                var facets = await _facetQuery.FetchFacets();

                if (items == null || facets == null)
                {
                    Facets = null;
                }
                else
                {
                    Facets =
                        items.Join(facets, i => i.ID, f => f.Key, (i, f) => new FacetViewModel<T>(i, f.Value)).ToList();
                }
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading facets for type '{0}'", LoggingLevel.Error, true,
                    typeof (T));
                failureMessage = AppResources.MessageLoadingFailed;
            }
            StopLoading(failureMessage);
        }

        #endregion
    }
}