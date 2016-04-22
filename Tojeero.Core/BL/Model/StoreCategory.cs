using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class StoreCategory : BaseLocalizableModelEntity<ParseStoreCategory>, IStoreCategory
    {
        #region Constructors

        public StoreCategory()
        {
        }

        public StoreCategory(ParseStoreCategory category = null)
            : base(category)
        {
        }

        #endregion

        #region Properties

        [Ignore]
        public string Name
        {
            get
            {
                if (Language == LanguageCode.Arabic && !string.IsNullOrEmpty(Name_ar))
                    return Name_ar;
                return Name_en;
            }
        }

        public string Name_en
        {
            get { return ParseObject.Name_en; }
            set { ParseObject.Name_en = value; }
        }


        public string Name_ar
        {
            get { return ParseObject.Name_ar; }
            set { ParseObject.Name_ar = value; }
        }

        #endregion

        #region Parent 

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region implemented abstract members of BaseLocalizableModelEntity

        protected override void raiseCulturalPropertyChange()
        {
            RaisePropertyChanged(() => Name);
        }

        #endregion
    }

    [ParseClassName("StoreCategory")]
    public class ParseStoreCategory : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("name_en")]
        public string Name_en
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("name_ar")]
        public string Name_ar
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        #endregion
    }
}