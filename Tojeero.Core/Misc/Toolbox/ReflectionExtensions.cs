using System;
using System.Reflection;
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
        public static string GetLocalTableName(this Type type)
        {
            var tableName = type.Name;
            var tableAttribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null && tableAttribute.Name != null)
                tableName = tableAttribute.Name;
            return tableName;
        }

        public static T GetEnum<T>(this string stringValue) where T : struct
        {
            var enumType = typeof (T);
            if (!enumType.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (Enum enumValue in Enum.GetValues(enumType))
            {
                var fi = enumType.GetRuntimeField(enumValue.ToString());
                var attributes = (StringValueAttribute[]) fi.GetCustomAttributes(
                    typeof (StringValueAttribute), false);
                if (attributes.Length > 0)
                {
                    var attr = attributes[0];
                    if (attr.StringValue == stringValue)
                    {
                        return (T) (object) enumValue;
                    }
                }
                else if (enumValue.ToString() == stringValue)
                {
                    return (T) (object) enumValue;
                }
            }
            throw new ArgumentException("The value '" + stringValue + "' is not supported.");
        }

        public static string GetString(this Enum enumValue)
        {
            var fi = enumValue.GetType().GetRuntimeField(enumValue.ToString());
            var attributes = (StringValueAttribute[]) fi.GetCustomAttributes(
                typeof (StringValueAttribute), false);
            if (attributes.Length > 0)
            {
                var attr = attributes[0];
                return attr.StringValue;
            }
            return enumValue.ToString();
        }
    }
}