﻿using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace AdvancedWindowsAppearence.Converters
{
	internal class FontToFontFamilyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Font f = (Font)value;
			return f.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
