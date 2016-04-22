using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class ProductSubcategory : BaseLocalizableModelEntity<ParseProductSubcategory>, IProductSubcategory
    {
        #region Constructors

        public ProductSubcategory()
        {
        }

        public ProductSubcategory(ParseProductSubcategory subcategory = null)
            : base(subcategory)
        {
        }

        #endregion

        #region Properties

        private string _categoryID;

        public string CategoryID
        {
            get
            {
                if (_categoryID == null && _parseObject != null && _parseObject.Category != null)
                    _categoryID = _parseObject.Category.ObjectId;
                return _categoryID;
            }
            set { _categoryID = value; }
        }

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

    [ParseClassName("ProductSubcategory")]
    public class ParseProductSubcategory : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("category")]
        public ParseProductCategory Category
        {
            get { return GetProperty<ParseProductCategory>(); }
            set { SetProperty(value); }
        }

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