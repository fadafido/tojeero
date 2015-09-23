using System;
using System.Threading.Tasks;
using System.IO;
using PCLStorage;
using Beezy.MvvmCross.Plugins.SecureStorage;
using Cirrious.CrossCore;
using Nito.AsyncEx;

namespace Tojeero.Core
{
    /// <summary>
    /// Access token.
    /// </summary>
    /// <remarks>Access token is retrieved from the API.</remarks>
    public class AccessToken
    {
		#region Private Fields and Properties

		private static string USER_TOKEN_KEY = "com.tojeero.tojeero:UserSessionToken";
		private static AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

        #region Properties

		/// <summary>
		/// Gets or sets the access_token.
		/// </summary>
		/// <value>The string representation of access_token.</value>
		public string Token { get; set; }

		/// <summary>
		/// Gets or sets the userName.
		/// </summary>
		/// <value>The user name.</value>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the date when the token will expire
		/// </summary>
		/// <value>The date when the token will expire.</value>
		public DateTimeOffset? ExpirationDate { get; set; }

		#endregion

        #region Public API
        /// <summary>
        /// Determines whether the token is expired or not.
        /// </summary>
        /// <returns><c>true</c> If the token is not expired; otherwise, <c>false</c>.</returns>
        public bool IsExpired()
		{
			if (ExpirationDate == null)
				return false;
			return DateTimeOffset.Now >= ExpirationDate.Value;
		}

		/// <summary>
		/// Save the access token.
		/// </summary>
		public async Task Save()
		{
			try
			{
				using(var writerLock = await _locker.WriterLockAsync())
				{
					await Task.Factory.StartNew(() =>
						{
							var secureStorage = Mvx.Resolve<IMvxProtectedData>();
							string jsonToken = Newtonsoft.Json.JsonConvert.SerializeObject(this);
							secureStorage.Protect(USER_TOKEN_KEY, jsonToken);
						});
				}
			}
			catch (Exception ex)
			{
				Mvx.Trace("Error occurred while saving access token to secure storage. {0}", ex.ToString());
			}
		}

		/// <summary>
		/// Loads the stored access token.
		/// </summary>
		/// <returns>The access token that has been previously stored if it exists.
		/// Otherwise returns <c>null</c></returns>
		public static async Task<AccessToken> LoadStoredToken()
		{
			try
			{
				using(var readerLock = await _locker.ReaderLockAsync())
				{
					var accessToken = await Task<AccessToken>.Factory.StartNew(() =>
						{
							var secureStorage = Mvx.Resolve<IMvxProtectedData>();
							string jsonToken = secureStorage.Unprotect(USER_TOKEN_KEY);
							if (string.IsNullOrEmpty(jsonToken))
								return null;
							var token = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(jsonToken);
							return token;
						});
					return accessToken;
				}
			}
			catch (Exception ex)
			{
				Mvx.Trace("Error occurred while loading stored access token from secure storage. {0}", ex.ToString());
				return null;
			}
		}

		public static async Task DeleteStoredToken()
		{
			try
			{
				using(var writerLock = await _locker.WriterLockAsync())
				{
					await Task.Factory.StartNew(() =>
						{
							var secureStorage = Mvx.Resolve<IMvxProtectedData>();
							secureStorage.Remove(USER_TOKEN_KEY);
						});
				}
			}
			catch (Exception ex)
			{
				Mvx.Trace("Error occurred while deleting stored access token from secure storage. {0}", ex.ToString());
			}
		}

		#endregion
    }
}

