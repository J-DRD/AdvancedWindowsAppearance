﻿using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
	public class ColorAppearanceSetting : AppearanceSetting
	{
		public readonly string SizeRegistryKey;

		public ColorAppearanceSetting(string _name, string _regeditPath, string _colorRegistryPath)
		{
			Name = _name;
			SizeRegistryKey = _regeditPath;
			ColorRegistryPath = _colorRegistryPath;
			LoadColorValues();
		}

		private void LoadColorValues()
		{
			Size = GetSizeFromRegistry(SizeRegistryKey);
			ItemColor = GetColorFromRegistry(ColorRegistryPath);
			IsEdited = false;
		}

		private int? GetSizeFromRegistry(string registrypath)
		{
			if (registrypath == null || registrypath == string.Empty) return null;
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
			if (registryKey == null) return null;

			var sizeReg = registryKey.GetValue(registrypath);
			registryKey.Close();
			if (sizeReg == null) return null;

			int intsize = int.Parse(sizeReg.ToString());
			if (registrypath != "Shell Icon Size")
				intsize = intsize / (-15);

			return intsize;
		}

		private void SaveSizeToRegistry()
		{
			if (!Size.HasValue) return;

			var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
			float sizeTemp = -15;

			if (SizeRegistryKey == "Shell Icon Size")
				sizeTemp = Size.Value;
			else
				sizeTemp = -15 * Size.Value;

			key.SetValue(SizeRegistryKey, sizeTemp, RegistryValueKind.String);
			key.Close();
		}

		public void SaveToRegistry()
		{
			base.SaveColorToRegistry();
			SaveSizeToRegistry();
			IsEdited = false;
		}
	}
}
