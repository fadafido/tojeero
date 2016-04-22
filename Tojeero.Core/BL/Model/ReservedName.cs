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

        #endregion

        #region Properties

        [ParseFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("type")]
        public int Type
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        #endregion
    }
}