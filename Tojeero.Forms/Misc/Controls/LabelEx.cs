using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class LabelEx : Label
    {
        #region Constructors

        #endregion

        #region Properties

        #region Line count

        public static BindableProperty LineCountProperty = BindableProperty.Create<LabelEx, int>(o => o.LineCount, 0);

        public int LineCount
        {
            get { return (int) GetValue(LineCountProperty); }
            set { SetValue(LineCountProperty, value); }
        }

        #endregion

        #endregion
    }
}