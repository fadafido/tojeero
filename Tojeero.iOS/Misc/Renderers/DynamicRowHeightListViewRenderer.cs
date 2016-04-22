using System;
using System.Reflection;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(ListView), typeof(Tojeero.iOS.Renderers.DynamicRowHeightListViewRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class DynamicRowHeightListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                var table = Control;
                table.Source =
                    new ListViewDataSourceWrapper(this.GetFieldValue<UITableViewSource>(typeof (ListViewRenderer),
                        "dataSource"));
            }
        }
    }

    public class ListViewDataSourceWrapper : UITableViewSource
    {
        private readonly UITableViewSource underlyingTableSource;

        public ListViewDataSourceWrapper(UITableViewSource underlyingTableSource)
        {
            this.underlyingTableSource = underlyingTableSource;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return GetCellInternal(tableView, indexPath);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return underlyingTableSource.RowsInSection(tableview, section);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return underlyingTableSource.GetHeightForHeader(tableView, section);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return underlyingTableSource.GetViewForHeader(tableView, section);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return underlyingTableSource.NumberOfSections(tableView);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            underlyingTableSource.RowSelected(tableView, indexPath);
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return underlyingTableSource.SectionIndexTitles(tableView);
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return underlyingTableSource.TitleForHeader(tableView, section);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var uiCell = GetCellInternal(tableView, indexPath);

            uiCell.SetNeedsLayout();
            uiCell.LayoutIfNeeded();

            var viewCell = uiCell.GetPropertyValue<ViewCell>(uiCell.GetType(), "ViewCell");

            return (nfloat) viewCell.RenderHeight;
        }

        private UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
        {
            return underlyingTableSource.GetCell(tableView, indexPath);
        }
    }

    public static class PrivateExtensions
    {
        public static T GetFieldValue<T>(this object @this, Type type, string name)
        {
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            return (T) field.GetValue(@this);
        }

        public static T GetPropertyValue<T>(this object @this, Type type, string name)
        {
            var property = type.GetProperty(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            return (T) property.GetValue(@this);
        }
    }
}