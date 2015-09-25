using System;
using Cirrious.CrossCore;

namespace Tojeero.Core.Toolbox
{
	/// <summary>
	/// Usefull event handler extensions
	/// </summary>
	public static class EventHandlerToolbox
	{

		public static void Fire<T>( this EventHandler<T> handler, object sender, T args ) 
			where T : EventArgs
		{
			if( handler != null )
			{
				try
				{
					handler( sender, args );
				}
				catch( Exception ex )
				{
					Mvx.Error( ex.ToString() );
					throw;
				}
			}
		}

		public static void Fire( this EventHandler handler, object sender, EventArgs args ) 
		{
			if( handler != null )
			{
				try
				{
					handler( sender, args );
				}
				catch( Exception ex )
				{
					Mvx.Error( ex.ToString() );
					throw;
				}
			}
		}
		
		public static void Fire( this EventHandler handler, object sender ) 
		{
			Fire( handler, sender, EventArgs.Empty );
		}
	}
}

