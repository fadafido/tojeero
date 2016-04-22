using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
	public class Tag : BaseModelEntity<ParseTag>, ITag
	{
		#region Constructors

		public Tag()
			:base()
		{

		}

		public Tag(ParseTag tag = null)
			: base(tag)
		{

		}


		#endregion

		#region Properties

		[Ignore]
		public override ParseTag ParseObject
		{
			get
			{
				return base.ParseObject;
			}
			set
			{

				base.ParseObject = value;
			}
		}

		public string Text
		{
			get
			{
				return this.ParseObject.Text;
			}
			set
			{
				this.ParseObject.Text = value;
				this.RaisePropertyChanged(() => Text);
			}
		}

		#endregion

		#region Parent 

		public override string ToString()
		{
			return Text;	
		}

		#endregion
	}

	[ParseClassName("Tag")]
	public class ParseTag : ParseObject
	{
		#region Constructors

		public ParseTag()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("text")]
		public string Text
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
			
		#endregion
	}
}

