﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
	public class RegistrySettingsViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<BoolRegistrySetting> RegistrySettings { get; set; } = new ObservableCollection<BoolRegistrySetting>();

		private bool _isEdited;

		public event PropertyChangedEventHandler PropertyChanged;

		internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool IsEdited
		{
			get => _isEdited; set
			{
				_isEdited = value;
				NotifyPropertyChanged();
			}
		}

		public void Add(string name, string registrykey, Version winVer)
		{
			var registryPath = @"Software\Microsoft\Windows\DWM";
			AddWithPath(name, registryPath, registrykey, winVer);
		}

		public void AddWithPath(string name, string registrypath, string registrykey, Version minimalWinVer)
		{
			if (minimalWinVer.CompareTo(Environment.OSVersion.Version) >= 0)
			{
				RegistrySettings.Add(new BoolRegistrySetting());
				return;
			}
			BoolRegistrySetting registrySetting = new BoolRegistrySetting(name, registrypath, registrykey);
			registrySetting.PropertyChanged += RegistrySetting_PropertyChanged;
			RegistrySettings.Add(registrySetting);
		}

		private void RegistrySetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			IsEdited = true;
		}

		public async Task SaveAll()
		{
			List<Task> tasks = new List<Task>();
			foreach (BoolRegistrySetting registrySetting in RegistrySettings)
			{
				if (registrySetting == null) continue;
				if (!registrySetting.Checked.HasValue) continue;

				tasks.Add(Task.Run(() => registrySetting.SaveToRegistry()));
			}
			await Task.WhenAll(tasks);
		}

		public string GetSettingsInReg()
		{
			string output = string.Empty;
			string prevPath = string.Empty;
			foreach (var rs in RegistrySettings)
			{
				if (!rs.IsEnabled)
					continue;
				string curPath = rs.RegistryPath.Replace(rs.RegistryKey + "\\", string.Empty);
				if (prevPath != curPath)
				{
					output += "\n[HKEY_CURRENT_USER\\" + curPath + "]";
					prevPath = curPath;
				}
				if (rs.Checked.HasValue)
					output += "\n\"" + rs.RegistryKey + "\"=dword:0000000" + Convert.ToInt32(rs.Checked.Value);
			}

			output += Environment.NewLine + Environment.NewLine;
			return output;
		}
	}
}
