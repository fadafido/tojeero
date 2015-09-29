using System;
using Parse;

namespace Tojeero.Core
{
	public static class ParseInitialize
	{
		public static void Initialize()
		{
			//Register custom subclasses
			ParseObject.RegisterSubclass<Product>();
			ParseObject.RegisterSubclass<Store>();

			//Initialize parse
			ParseClient.Initialize(Constants.ParseApplicationId, Constants.ParseDotNetKey);
			ParseFacebookUtils.Initialize(Constants.FacebookAppId);
		}
	}
}

