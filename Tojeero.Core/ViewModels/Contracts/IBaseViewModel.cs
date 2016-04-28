using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Contracts
{
    public interface IBaseViewModel
    {
        ShowDialogDelegate ShowDialogAction { get; set; }
        void ShowDialog(string title, string content,
            string negativeTitle, string positiveTitle = null,
            Action negativeAction = null, Action positiveAction = null);
        void OnAppearing();
        void OnDisappearing();
        void OnStarting();
    }
}
