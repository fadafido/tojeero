using System;
using Parse;

namespace Tojeero.Core
{
	public interface IParseMultiImageEntity
	{
		ParseRelation<ParseData> Images { get; }
	}
}

