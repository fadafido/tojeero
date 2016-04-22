using Parse;

namespace Tojeero.Core.Model.Contracts
{
	public interface IParseMultiImageEntity
	{
		ParseRelation<ParseData> Images { get; }
	}
}

