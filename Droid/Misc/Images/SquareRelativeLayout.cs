﻿using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;

namespace Tojeero.Droid
{
	public class SquareRelativeLayout : RelativeLayout
	{
		public SquareRelativeLayout(IntPtr javeReference, JniHandleOwnership transfer)
			: base(javeReference, transfer)
		{
		}

		public SquareRelativeLayout(Context context)
			: base(context)
		{
		}

		public SquareRelativeLayout(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			
		}

		public SquareRelativeLayout(Context context, IAttributeSet attrs, int defStyleAttr)
			: base(context, attrs, defStyleAttr)
		{			
		}

		public SquareRelativeLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
			: base(context, attrs, defStyleAttr, defStyleRes)
		{			
		}


		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
		}
	}
}

