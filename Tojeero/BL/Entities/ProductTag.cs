using System;

namespace Tojeero.Core
{
	public class ProductTag : IUniqueEntity
	{
		public string ID { get; set; }
		public string ProductID { get; set; }
		public string Tag { get; set; }
	}
}

