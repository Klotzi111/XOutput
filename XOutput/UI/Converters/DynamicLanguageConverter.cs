using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
	/// <summary>
	/// Translates a text.
	/// Cannot be used backwards.
	/// </summary>
	public class DynamicLanguageConverter : IMultiValueConverter
	{
		/// <summary>
		/// Translates a text.
		/// </summary>
		/// <param name="values">translation values and text to translate</param>
		/// <param name="targetType">Ignored</param>
		/// <param name="parameter">Ignored</param>
		/// <param name="culture">Ignored</param>
		/// <returns></returns>
		public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var translations = values[0] as Dictionary<string, string>;
			string key;
			if (values[1] is Enum)
			{
				key = values[1].GetType().Name + "." + values[1].ToString();
				return GetTranslation(translations, key) ?? values[1].ToString();
			}
			else if (values[1] is string val1Str)
			{
				key = val1Str;
			}
			else if (values[1] is bool)
			{
				key = (bool)values[1] ? "True" : "False";
			}
			else if (values[1] is null)
			{
				return null;
			}
			else
			{
				return values[1].ToString();
			}
			return GetTranslation(translations, key) ?? key;
		}

		/// <summary>
		/// Intentionally not implemented.
		/// </summary>
		/// <param name="value">Ignored</param>
		/// <param name="targetTypes">Ignored</param>
		/// <param name="parameter">Ignored</param>
		/// <param name="culture">Ignored</param>
		/// <returns></returns>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		protected string? GetTranslation(Dictionary<string, string>? translations, string key)
		{
			if (translations == null || key == null || !translations.ContainsKey(key))
			{
				return null;
			}
			return translations[key];
		}
	}
}
