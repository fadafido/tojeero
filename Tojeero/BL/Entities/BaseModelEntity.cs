using System;

namespace Tojeero.Core
{
	public class BaseModelEntity : IModelEntity
	{
		#region Constructors

		public BaseModelEntity()
		{
		}

		#endregion

		#region Properties

		#region IModelEntity implementation

		private int _sortOrder;

		public int SortOrder
		{
			get
			{
				return _sortOrder;
			}
			set
			{
				if (_sortOrder != value)
				{
					_sortOrder = value;
				}
			}
		}

		#endregion

		#region IUniqueEntity implementation

		private string _id;

		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				if (_id != value)
				{
					_id = value;
				}
			}
		}

		#endregion



		#endregion
	}
}

