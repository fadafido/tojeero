using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class PagerIndicatorDots : StackLayout
    {
        #region Private fields and properties

        private int dotCount = 1;
        private int _selectedIndex;
        private Button _selectedButton;

        #endregion

        #region Constructors

        public PagerIndicatorDots()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.Center;
            Orientation = StackOrientation.Horizontal;
            DotColor = Color.Black;
        }

        #endregion

        #region Properties

        public Color DotColor { get; set; }

        public double DotSize { get; set; }

        #region ItemsSource

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create<PagerIndicatorDots, IList>(
                pi => pi.ItemsSource,
                null,
                BindingMode.OneWay,
                propertyChanging:
                    (bindable, oldValue, newValue) => { ((PagerIndicatorDots) bindable).ItemsSourceChanging(); },
                propertyChanged:
                    (bindable, oldValue, newValue) => { ((PagerIndicatorDots) bindable).ItemsSourceChanged(); }
                );

        public IList ItemsSource
        {
            get { return (IList) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #region SelectedItem

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create<PagerIndicatorDots, object>(
                pi => pi.SelectedItem,
                null,
                propertyChanged:
                    (bindable, oldValue, newValue) => { ((PagerIndicatorDots) bindable).SelectedItemChanged(); });

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion

        #endregion

        #region Utility methods

        void CreateDot()
        {
            //Make one button and add it to the dotLayout
            var dot = new Button
            {
                BorderRadius = Convert.ToInt32(DotSize/2),
                HeightRequest = DotSize,
                WidthRequest = DotSize,
                BackgroundColor = DotColor
            };
            Children.Add(dot);
        }

        void ItemsSourceChanging()
        {
            if (ItemsSource != null)
                _selectedIndex = ItemsSource.IndexOf(SelectedItem);
        }

        void ItemsSourceChanged()
        {
            if (ItemsSource == null)
                return;

            // Dots *************************************
            var countDelta = ItemsSource.Count - Children.Count;

            if (countDelta > 0)
            {
                for (var i = 0; i < countDelta; i++)
                {
                    CreateDot();
                }
            }
            else if (countDelta < 0)
            {
                for (var i = 0; i < -countDelta; i++)
                {
                    Children.RemoveAt(0);
                }
            }
            //*******************************************
            SelectedItemChanged();
        }

        void SelectedItemChanged()
        {
            var selectedIndex = ItemsSource.IndexOf(SelectedItem);
            var pagerIndicators = Children.Cast<Button>().ToList();

            foreach (var pi in pagerIndicators)
            {
                UnselectDot(pi);
            }

            if (selectedIndex > -1)
            {
                SelectDot(pagerIndicators[selectedIndex]);
            }
        }

        static void UnselectDot(Button dot)
        {
            dot.Opacity = 0.5;
        }

        static void SelectDot(Button dot)
        {
            dot.Opacity = 1.0;
        }

        #endregion
    }
}