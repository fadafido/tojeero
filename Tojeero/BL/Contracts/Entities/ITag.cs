using System;

namespace Tojeero.Core
{
	public interface ITag : ISelectableEntity, IModelEntity
	{
		string Text { get; set; }
	}
}

