using System;
using System.ComponentModel;
using System.Reflection;
using Android.Views;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof (ListViewEx), typeof (ListViewExRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class ListViewExRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
        #region Private fields

        private View _footerView;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            updateFooterView();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Forms.Controls.ListViewEx.FooterViewProperty.PropertyName)
            {
                updateFooterView();
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            updateFooterView();
        }

        #endregion

        #region Utility methods

        private void updateFooterView()
        {
            if (Element != null && Control != null)
            {
                //Remove previously added footer view
                if (_footerView != null)
                    Control.RemoveFooterView(_footerView);

                var listView = Element as Forms.Controls.ListViewEx;
                var footerView = listView.FooterView;
                if (footerView != null)
                {
                    _footerView = footerView.GetNativeView();
                    if (_footerView != null)
                        Control.AddFooterView(_footerView);
                }
                else
                {
                    _footerView = null;
                }
            }
        }

        #endregion
    }

    public static class ViewExtensions
    {
        private static readonly Type _platformType = typeof (Platform);
        private static BindableProperty _rendererProperty;

        public static BindableProperty RendererProperty
        {
            get
            {
                var field = _platformType.GetField("RendererProperty",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                _rendererProperty = (BindableProperty) field?.GetValue(null);
                return _rendererProperty;
            }
        }

        public static IVisualElementRenderer GetRenderer(this BindableObject bindableObject)
        {
            return (IVisualElementRenderer) bindableObject.GetValue(RendererProperty);
        }

        public static View GetNativeView(this VisualElement visualElement)
        {
            var renderer = Platform.CreateRenderer(visualElement);
            renderer.ViewGroup.LayoutParameters = new ViewGroup.LayoutParams(300, 80);
            return renderer.ViewGroup?.RootView;
        }
    }
}