using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Parse;
using Tojeero.Core.Messages;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Core.Model
{
    public abstract class BaseLocalizableModelEntity<T> : BaseModelEntity<T>
        where T : ParseObject
    {
        #region Private fields

        private ILocalizationService _localization;
        private IMvxMessenger _messenger;
        private MvxSubscriptionToken _token;

        #endregion

        #region Constructors

        public BaseLocalizableModelEntity()
        {
            initialize();
        }

        public BaseLocalizableModelEntity(T parseObject = null)
            : base(parseObject)
        {
            initialize();
        }

        private void initialize()
        {
            _localization = Mvx.Resolve<ILocalizationService>();
            _messenger = Mvx.Resolve<IMvxMessenger>();
            _token = _messenger.Subscribe<LanguageChangedMessage>(message => { raiseCulturalPropertyChange(); });
        }

        #endregion

        #region Properties

        public LanguageCode Language
        {
            get { return _localization.Language; }
        }

        #endregion

        #region Protected api

        protected abstract void raiseCulturalPropertyChange();

        #endregion
    }
}