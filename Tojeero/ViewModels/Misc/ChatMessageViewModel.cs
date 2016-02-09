using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Xamarin.Forms;

namespace Tojeero.Forms.ViewModels.Misc
{
    public class ChatMessageViewModel : MvxViewModel
    {
        #region Private fields and properties

        private readonly IProductManager _productManager;

        #endregion

        #region Constructors

        public ChatMessageViewModel(
            IProductManager productManager,
            IChatMessage message = null, 
            string profilePictureUrl = null, 
            bool isSentByCurrentUser = true)
        {
            _productManager = productManager;
            _message = message;
            _isSentByCurrentUser = isSentByCurrentUser;
            ProfilePictureUrl = profilePictureUrl;
            Product = new ProductViewModel();
            updateViewModel();
        }

        #endregion

        #region Properties

        private IChatMessage _message;
        public IChatMessage Message
        {
            get
            {
                return _message;
            }
            private set
            {
                if (_message != value)
                {
                    _message = value;
                    updateViewModel();
                }
            }
        }

        private bool _isSentByCurrentUser;
        public bool IsSentByCurrentUser
        {
            get
            {
                return _isSentByCurrentUser;
            }
            private set
            {
                if(_isSentByCurrentUser != value)
                {
                    _isSentByCurrentUser = value;
                    updateViewModel();
                }
            }
        }

        public string ProfilePictureUrl { get; private set; }
        public string FormattedDeliveryDate { get; private set; }

        private ProductViewModel _product;
        public ProductViewModel Product
        { 
            get  
            {
                return _product; 
            }
            private set 
            {
                _product = value; 
                RaisePropertyChanged(() => Product); 
            }
        }  

        #endregion

        #region Utility methods

        private async void updateViewModel()
        {
            FormattedDeliveryDate = $"{Message?.DeliveryDate:ddd, d MMM HH:mm}";
            RaiseAllPropertiesChanged();
            IProduct product = null;
            if (Message?.ProductID != null)
            {
                product = await _productManager.FetchProduct(Message.ProductID);
            }
            Product.Product = product;
        }

        #endregion
    }
}
