using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Foundation;

namespace ObjectX.Touch.Toolbox
{
	public static class UIViewToolbox
	{	
		/// <summary>
		/// Finds the first responder in the view by recursivly searching out the view hierarchy begining with <paramref name="view"/>
		/// </summary>
		/// <returns>The first responder.</returns>
		/// <param name="view">View.</param>
		public static UIView FindFirstResponder( this UIView view )
		{
			if( view.IsFirstResponder )
				return view;

			foreach( var v in view.Subviews )
			{
				var first = v.FindFirstResponder();
				if( first != null )
					return first;
			}

			return null;
		}

		/// <summary>
		/// Gets the first responder of key window.
		/// </summary>
		/// <value>The first responder of key window.</value>
		public static UIView FirstResponder { get { return UIApplication.SharedApplication.KeyWindow.FindFirstResponder(); } }

		public static void ResignFirstResponder()
		{
			var fr = FirstResponder;
			if( fr != null )
				fr.ResignFirstResponder();
		}

		/// <summary>
		/// Finds the first occurence of child with specified type in the view by recursivly searching out the view hierarchy begining with <paramref name="root"/>
		/// </summary>
		/// <returns>The first occurence of child with specified type, or <c>null</c> if not found.</returns>
		/// <param name="root">Root.</param>
		/// <typeparam name="T">The type of the child view that the user wants to get.</typeparam>
		public static T FindViewOfType<T>( this UIView root ) where T : UIView
		{
			return FindViewOfType( root, typeof( T ) ) as T;
		}

		/// <summary>
		/// Finds the first occurence of child with specified type in the view by recursivly searching out the view hierarchy begining with <paramref name="root"/>
		/// </summary>
		/// <returns>The first occurence of child with specified type.</returns>
		/// <param name="root">Root.</param>
		/// <param name="type">Type.</param>
		public static UIView FindViewOfType( this UIView root, Type type )
		{
			if( type.IsInstanceOfType( root ) )
				return root;

			foreach( var v in root.Subviews )
			{
				var child = v.FindViewOfType( type );
				if( child != null )
					return child;
			}

			return null;
		}

		/// <summary>
		/// Finds the first occurence of child view which satisfies the <paramref name="matching"/> predicate 
		/// by recursivly searching out the view hierarchy begining with <paramref name="root"/>
		/// </summary>
		/// <returns>The first occurence of child view which satisfies the <paramref name="matching"/> predicate.</returns>
		/// <param name="root">The root view from where to begin the search.</param>
		/// <param name="matching">Matching.</param>
		public static UIView FindView( this UIView root, Func<UIView,bool> matching )
		{
			if( root == null )
				return null;
			
			if( matching( root ) )
				return root;
			
			foreach( var v in root.Subviews )
			{
				var child = v.FindView( matching );
				if( child != null )
					return child;
			}
			
			return null;
		}

		/// <summary>
		/// Finds the first occurence of child with specified has specified class name
		/// in the view by recursivly searching out the view hierarchy begining with <paramref name="root"/>.
		/// </summary>
		/// <returns>The first occurence of child with specified has specified class name.</returns>
		/// <param name="root">The root view from where to begin the search.</param>
		/// <param name="className">Class name.</param>
		public static UIView FindViewOfClass( this UIView root, string className )
		{
			if( root == null )
				return null;

			var cls = new ObjCRuntime.Class( root.ClassHandle );
			if( cls.Name == className )
				return root;

			foreach( var v in root.Subviews )
			{
				var child = v.FindViewOfClass( className );
				if( child != null )
					return child;
			}

			return null;
		}

		/// <summary>
		/// Finds the first occurence of the superview of this view which has the type specified in <paramref name="type"/> parameter
		/// </summary>
		/// <returns>The first occurence of the superview of given type, or <c>null</c> if not found.</returns>
		/// <param name="view">The view the superview of which needs to be returned</param>
		/// <param name="type">The type of the superview that the user wants to get.</param>
		public static UIView FindSuperviewOfType(this UIView view, Type type)
		{
			UIView superview = view.Superview;
			while (superview != null && superview.GetType () != type)
				superview = superview.Superview;
			return superview;
		}

		/// <summary>
		/// Finds the first occurence of the superview of this view which has the specified type. 
		/// </summary>
		/// <returns>The first occurence of the superview of given type, or <c>null</c> if not found.</returns>
		/// <param name="view">The view the superview of which needs to be returned</param>
		/// <param name="type">The type of the superview that the user wants to get.</param>
		public static T FindSuperviewOfType<T>(this UIView view) where T : UIView
		{
			return FindSuperviewOfType( view, typeof( T ) ) as T;
		}

		/// <summary>
		/// Finds the keyboard view.
		/// </summary>
		/// <returns>The keyboard view.</returns>
		public static UIView FindKeyboardView()
		{
			var windows = UIApplication.SharedApplication.Windows;
			Array.Reverse( windows );
			foreach( var window in windows )
			{
				var kb = FindViewOfClass( window, "UIKeyboardImpl" );
				if( kb == null )
					kb = FindViewOfClass( window, "UIKeyboard" );
				if( kb != null )
					return kb;
			}

			return null;
		}
	}
}

