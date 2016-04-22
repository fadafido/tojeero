using System;
using Foundation;
using Tojeero.Core.Toolbox;
using UIKit;

namespace Tojeero.iOS.Toolbox
{
	public enum InputAccessoryViewMode
	{
		Done = 1,
		NextPrevious = 2
	}

	[Register("InputToolboxViewOwner")]
	public partial class InputToolboxViewOwner : NSObject
	{

	}
	[Register("InputToolboxView")]
	public partial class InputToolboxView : UIView
	{
		#region Private fields
		private const string nibName = "InputToolboxView";
		private UIView[] _inputs;
		private InputAccessoryViewMode _mode;
		#endregion

		#region Initialization

		public InputToolboxView()
			: base()
		{
		}

		public InputToolboxView(IntPtr handle) : base(handle)
		{
		}

		public static InputToolboxView CreateFromNIB()
		{
			return CreateFromNIB(null);
		}

		public static InputToolboxView CreateFromNIB(UIView[] inputs, InputAccessoryViewMode mode = InputAccessoryViewMode.Done | InputAccessoryViewMode.NextPrevious)
		{
			InputToolboxViewOwner owner = new InputToolboxViewOwner();
			NSBundle.MainBundle.LoadNib(nibName, owner, null);
			owner.inputAccessoryView.makeUICustomizations();
			owner.inputAccessoryView._inputs = inputs;
			owner.inputAccessoryView.Mode = mode;
			return owner.inputAccessoryView;
		}


		#endregion

		#region Public API

		public event EventHandler<EventArgs> DoneButtonTapped;
		public event EventHandler<EventArgs> NextButtonTapped;
		public event EventHandler<EventArgs> PreviousButtonTapped;

		public InputAccessoryViewMode Mode  {
			get
			{
				return _mode;
			}
			set
			{
				if(_mode != value)
				{
					_mode = value;
					toggleMode();
				}
			}
		}
		#endregion

		#region UI events

		partial void doneButtonTapped(NSObject sender)
		{
			UIViewToolbox.ResignFirstResponder();
			DoneButtonTapped.Fire(this, new EventArgs());
		}

		partial void segmentButtonValueChanged(NSObject sender)
		{
			switch(this.segmentControl.SelectedSegment)
			{
				case 0:
					{
						triggerResponder(false);
						PreviousButtonTapped.Fire(this, new EventArgs());
					}
					break;
				case 1:
					{
						triggerResponder(true);
						NextButtonTapped.Fire(this, new EventArgs());
					}
					break;
			}
			this.segmentControl.SelectedSegment = -1;
		}

		#endregion

		#region Utility Methods

		private void makeUICustomizations()
		{
			this.doneButton.Layer.CornerRadius = 2.5f;
			this.segmentControl.SelectedSegment = -1;
		}

		void triggerResponder(bool isNext)
		{
			if (this._inputs != null)
			{
				var firstResponder = UIViewToolbox.FirstResponder;
				if (firstResponder == null)
					return;
				firstResponder.ResignFirstResponder();
				var index = Array.IndexOf<UIView>(this._inputs, firstResponder);
				if (index >= 0)
				{
					index += isNext ? 1 : -1;
					if (index >= 0 && index < this._inputs.Length)
					{
						var view = _inputs[index];
						view.BecomeFirstResponder();
					}
				}
			}
		}

		void toggleMode()
		{
			if (((int)Mode & (int)InputAccessoryViewMode.Done) != 0)
			{
				this.doneButton.Hidden = false;
			}
			else
			{
				this.doneButton.Hidden = true;
			}

			if (((int)Mode & (int)InputAccessoryViewMode.NextPrevious) != 0)
			{
				this.segmentControl.Hidden = false;
			}
			else
			{
				this.segmentControl.Hidden = true;
			}
		}
		#endregion
	}
}

