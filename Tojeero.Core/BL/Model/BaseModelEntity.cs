using Cirrious.MvvmCross;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using Parse;

namespace Tojeero.Core.Model
{
	public abstract class BaseModelEntity<T> : MvxNotifyPropertyChanged where T : ParseObject
	{
		#region Constructors

		public BaseModelEntity(T parseObject = null)
		{
			_parseObject = parseObject ?? Parse.ParseObject.Create<T>(); ;
			var alwaysOnUIThread = (MvxSingletonCache.Instance == null)
				? true
				: MvxSingletonCache.Instance.Settings.AlwaysRaiseInpcOnUserInterfaceThread;
			ShouldAlwaysRaiseInpcOnUserInterfaceThread(alwaysOnUIThread);
		}

		#endregion

		#region Properties

		protected T _parseObject;
		[Ignore]
		public virtual T ParseObject
		{ 
			get
			{
				return _parseObject; 
			}
			set
			{
				_parseObject = value ?? Parse.ParseObject.Create<T>(); 
				RaiseAllPropertiesChanged();
			}
		}

		[PrimaryKey]
		[JsonProperty("objectID")]
		public string ID
		{ 
			get
			{
				return _parseObject.ObjectId;
			}
			set
			{
				_parseObject.ObjectId = value;
				RaisePropertyChanged(() => ID); 
			}
		}

		#endregion

	}
}

