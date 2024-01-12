using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
	/// <summary>
	/// Converts a boolean value to brush.
	/// Cannot be used backwards.
	/// </summary>
	public class EnumerableNotEmptyToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a list to a bool value, based on if it has any elements.
		/// </summary>
		/// <param name="value">List to check</param>
		/// <param name="targetType">Ignored</param>
		/// <param name="parameter">Invert</param>
		/// <param name="culture">Ignored</param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not IEnumerable enumerableValue)
			{
				throw new ArgumentException("Value must be a IEnumerable");
			}

			var testValue = enumerableValue.Cast<object>().Any();
			if (parameter is string && (string)parameter == "invert")
			{
				testValue = !testValue;
			}
			return testValue ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Intentionally not implemented.
		/// </summary>
		/// <param name="value">Ignored</param>
		/// <param name="targetType">Ignored</param>
		/// <param name="parameter">Ignored</param>
		/// <param name="culture">Ignored</param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
