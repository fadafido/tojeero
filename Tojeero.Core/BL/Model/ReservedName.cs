using Parse;

namespace Tojeero.Core.Model
{	
	public enum ReservedNameType
	{
		Unknown,	
		Store
	}

	[ParseClassName("ReservedName")]
	public class ReservedName : ParseObject
	{
		#region Constructors

		public ReservedName()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("type")]
		public int Type
		{
			get
			{
				return GetProperty<int>();
			}
			set
			{
				SetProperty<int>(value);
			}
		}

		#endregion
	}
}

