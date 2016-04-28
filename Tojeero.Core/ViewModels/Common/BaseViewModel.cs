﻿using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public delegate void ShowDialogDelegate(string title, string content,
        string negativeTitle, string positiveTitle = null,
        Action negativeAction = null, Action positiveAction = null);

    public class BaseViewModel : MvxViewModel, IBaseViewModel
    {
        #region Constructors

        public BaseViewModel()
            :base()
        {
            
        }

        #endregion

        #region Properties
        public ShowDialogDelegate ShowDialogAction { get; set; }
        public Action CloseAction { get; set; }

        #endregion

        #region Public API

        public void ShowDialog(string title, string content,
            string negativeTitle, string positiveTitle = null,
            Action negativeAction = null, Action positiveAction = null)
        {
            ShowDialogAction?.Invoke(title, content, negativeTitle, positiveTitle, negativeAction, positiveAction);
        }

        public void Close()
        {
            CloseAction?.Invoke();
        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        public virtual void OnStarting()
        {
        }

        #endregion
    }
}
