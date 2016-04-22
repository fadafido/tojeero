﻿using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class Data : BaseModelEntity<ParseData>, IData
    {
        #region Constructors

        public Data()
        {
        }

        public Data(ParseData parseData = null)
            : base(parseData)
        {
        }

        #endregion

        #region Properties

        [Ignore]
        public override ParseData ParseObject
        {
            get { return base.ParseObject; }
            set
            {
                _url = null;
                base.ParseObject = value;
            }
        }

        private string _url;

        public string Url
        {
            get
            {
                if (_url == null && _parseObject != null && _parseObject.File != null && _parseObject.File.Url != null)
                    _url = _parseObject.File.Url.ToString();
                return _url;
            }
            set
            {
                _url = value;
                RaisePropertyChanged(() => Url);
            }
        }

        #endregion
    }

    [ParseClassName("Data")]
    public class ParseData : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("file")]
        public ParseFile File
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty(value); }
        }

        #endregion
    }
}