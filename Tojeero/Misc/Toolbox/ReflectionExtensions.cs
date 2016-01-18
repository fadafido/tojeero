using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core.Toolbox
{
	[AttributeUsage(AttributeTargets.Field)]
	public class StringValueAttribute : Attribute
	{
		public string StringValue { get; set; }

		public StringValueAttribute()
		{
			StringValue = null;
		}

		public StringValueAttribute(string stringValue)
		{
			StringValue = stringValue;
		}
	}

	public static class ReflectionExtensions
	{
		public static string GetLocalTableName(this Type type) {
			var tableName = type.Name;
			var tableAttribute = IntrospectionExtensions.GetTypeInfo(type).GetCustomAttribute<TableAttribute>();
			if (tableAttribute != null && tableAttribute.Name != null)
				tableName = tableAttribute.Name;
			return tableName;
		}
			
		public static T GetEnum<T>(this string stringValue) where T : struct
		{
			Type enumType = typeof(T);
			if (!enumType.GetTypeInfo().IsEnum) 
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			foreach (Enum enumValue in Enum.GetValues(enumType))
			{
				FieldInfo fi = enumType.GetRuntimeField(enumValue.ToString());
				StringValueAttribute[] attributes = (StringValueAttribute[])fi.GetCustomAttributes(
					typeof(StringValueAttribute), false);
				if (attributes.Length > 0)
				{
					StringValueAttribute attr = attributes[0];
					if (attr.StringValue == stringValue)
					{
						return (T)(object)enumValue;
					}
				}
				else if (enumValue.ToString() == stringValue)
				{
					return (T)(object)enumValue;
				}
			}
			throw new ArgumentException("The value '" + stringValue + "' is not supported.");
		}

		public static string GetString(this Enum enumValue)
		{
			FieldInfo fi = enumValue.GetType().GetRuntimeField(enumValue.ToString());
			StringValueAttribute[] attributes = (StringValueAttribute[])fi.GetCustomAttributes(
				typeof(StringValueAttribute), false);
			if (attributes.Length > 0)
			{
				StringValueAttribute attr = attributes[0];
				return attr.StringValue;
			}
			else
			{
				return enumValue.ToString();
			}
		}
	}

}
