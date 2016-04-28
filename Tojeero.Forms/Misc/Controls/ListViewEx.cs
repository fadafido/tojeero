using System.Windows.Input;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class ListViewEx : Xamarin.Forms.ListView
    {
        #region Constructors

        public ListViewEx()
        {
            ItemSelected += OnItemSelected;
        }

        #endregion

        #region Properties

        public static readonly BindableProperty ItemClickedCommandProperty =
            BindableProperty.Create<ListViewEx, ICommand>(p => p.ItemClickedCommand, null);

        public ICommand ItemClickedCommand
        {
            get { return (ICommand) GetValue(ItemClickedCommandProperty); }
            set { SetValue(ItemClickedCommandProperty, value); }
        }

        public static BindableProperty FooterViewProperty = BindableProperty.Create<ListViewEx, View>(o => o.FooterView,
            null);

        public View FooterView
        {
            get { return (View) GetValue(FooterViewProperty); }
            set { SetValue(FooterViewProperty, value); }
        }

        #endregion


        #region Utility methods

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SelectedItem = null;
                if (ItemClickedCommand != null && ItemClickedCommand.CanExecute(SelectedItem))
                    ItemClickedCommand.Execute(SelectedItem);
            }
            SelectedItem = null;
        }

        #endregion
    }
}