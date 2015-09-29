using System;
using Parse;

namespace Tojeero.Core
{
	[ParseClassName("Product")]
	public class Product : BaseModelEntity, IProduct
	{
		public Product()
		{
		}
	}
}

