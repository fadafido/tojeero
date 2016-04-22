using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public class FacetViewModel<T> : MvxViewModel where T : IUniqueEntity
    {
        #region Constructors

        public FacetViewModel(T data = default(T), int count = 0, bool countVisible = true)
        {
            Data = data;
            Count = count;
            CountVisible = countVisible;
        }

        #endregion

        #region Properties

        private T _data;

        public T Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }

        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged(() => Count);
            }
        }

        private bool _countVisible;

        public bool CountVisible
        {
            get { return _countVisible; }
            set
            {
                _countVisible = value;
                RaisePropertyChanged(() => CountVisible);
            }
        }

        #endregion
    }
}