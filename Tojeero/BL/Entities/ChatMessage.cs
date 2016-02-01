using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
    public class ChatMessage : MvxViewModel, IChatMessage
    {
        private string _text;
        public string Text
        { 
            get  
            {
                return _text; 
            }
            set 
            {
                _text = value; 
                RaisePropertyChanged(() => Text); 
            }
        }  
        public void Clear()
        {
            this.Text = null;
        }
    }
}
