using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class LoadingOverlay : Grid
    {
        #region Constructors

        public LoadingOverlay()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private string _title;
        public string Title  
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                TitleLabel.Text = value;
            }
        }

        #endregion

    }
}
