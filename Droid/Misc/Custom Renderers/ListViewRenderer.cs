using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Tojeero.Forms.ListView), typeof(Tojeero.Droid.Renderers.ListViewRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class ListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
        #region Private fields

        private Android.Views.View _footerView = null;

        #endregion

        #region Parent override
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            updateFooterView();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Forms.ListView.FooterViewProperty.PropertyName)
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

                var listView = Element as Forms.ListView;
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
        private static readonly Type _platformType = typeof (Xamarin.Forms.Platform.Android.Platform);
        private static BindableProperty _rendererProperty;

        public static BindableProperty RendererProperty
        {
            get
            {
                var field = _platformType.GetField("RendererProperty",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                _rendererProperty = (BindableProperty)field?.GetValue(null);
                return _rendererProperty;
            }
        }

        public static IVisualElementRenderer GetRenderer(this BindableObject bindableObject)
        {
            return (IVisualElementRenderer)bindableObject.GetValue(RendererProperty);
        }

        public static Android.Views.View GetNativeView(this VisualElement visualElement)
        {
            var renderer = Platform.CreateRenderer(visualElement);
            renderer.ViewGroup.LayoutParameters = new ViewGroup.LayoutParams(300, 80);
            return renderer.ViewGroup?.RootView;
        }
    }
}