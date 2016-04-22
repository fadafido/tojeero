using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Cirrious.CrossCore;
using Tojeero.Core.Services.Contracts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tojeero.Forms.Markup_Extensions
{
	// You exclude the 'Extension' suffix when using in Xaml markup
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		readonly CultureInfo ci;
		const string ResourceId = "Tojeero.Core.Resources.AppResources";

		public TranslateExtension()
		{
			ci = Mvx.Resolve<ILocalizationService>().Culture;
		}

		public string Text { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Text == null)
				return "";

			ResourceManager resmgr = new ResourceManager(ResourceId, typeof(ILocalizationService).GetTypeInfo().Assembly);

			var translation = resmgr.GetString(Text, ci);

			if (translation == null)
			{
				var ex = new ArgumentException(String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, ci.Name), "Text");
				#if DEBUG
				throw ex;
				#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
				Tools.Logger.Log(ex, LoggingLevel.Error, true);
				#endif
			}
			return translation;
		}
	}
}