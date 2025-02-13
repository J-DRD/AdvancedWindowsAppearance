﻿using AdvancedWindowsAppearence.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdvancedWindowsAppearence
{
	public class GeneralViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged; // to do: implement this to refresh the preview

		internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ThemeSettings ThemeSettings { get; set; }
		public ApplicationSettings ApplicationSettings { get; private set; }
		public AeroColorRegistrySetting ThemeColor { get; private set; } = new AeroColorRegistrySetting("Theme Color", "ColorizationColor");
		public ColorAppearanceSetting[] ColorSettings { get; private set; }
		public FontAppearanceSetting[] FontSettings { get; private set; }
		public WallpaperSettings WallpaperSettings { get; private set; }
		public RegistrySettingsViewModel RegistrySettingsViewModel { get; private set; }
		public AeroColorsViewModel AeroColorsViewModel { get; private set; }
		public bool UseThemes { get; set; } = true; //when false: it means to not apply theme, only to change registry settings
		public bool IsWindows10 { get; set; }

		private bool _isSavingInProgress = false;

		public bool IsSavingInProgress
		{
			get => _isSavingInProgress;
			set
			{
				_isSavingInProgress = value;
				NotifyPropertyChanged();
			}
		}

		#region Initialization

		public GeneralViewModel()
		{
			IsWindows10 = (bool)(Environment.OSVersion.Version >= new Version(10, 0));

			Task.Run(() =>
			{
				ApplicationSettings = new ApplicationSettings();
				ThemeSettings = new ThemeSettings(this);
				RegistrySettingsViewModel = new RegistrySettingsViewModel();
				InitAeroColors();
				InitRegistrySettings();
				InitDPIScale();
				InitColors();
				InitFonts();
				InitWallpaper();
			}
			);
		}

		private void InitAeroColors()
		{
			AeroColorsViewModel = new AeroColorsViewModel();
			AeroColorsViewModel.Add("Active Window Color", "AccentColor", new Version(10, 0));
			AeroColorsViewModel.Add("Inactive Window Color", "AccentColorInactive", new Version(10, 0));
		}

		private void InitDPIScale()
		{
			FontManager.GetDPI();
		}

		private void InitRegistrySettings()
		{
			RegistrySettingsViewModel.Add("Enable colored titlebars", "ColorPrevalence", new Version(10, 0));
			//RegistrySettingsViewModel.Add("Enable opaque taskbar (win 8 only)", "ColorizationOpaqueBlend", new Version(6, 2));  not working?
			RegistrySettingsViewModel.Add("Enable peek at desktop", "EnableAeroPeek", new Version(6, 1));
			RegistrySettingsViewModel.Add("Hibernate thumbnails in taskbar", "AlwaysHibernateThumbnails", new Version(6, 1));
			RegistrySettingsViewModel.AddWithPath("Enable transparency effects", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", new Version(10, 0));
			RegistrySettingsViewModel.AddWithPath("Show accent color on the start and actioncenter", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", new Version(10, 0));
			RegistrySettingsViewModel.AddWithPath("Use light theme for Apps", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", new Version(10, 0));
			RegistrySettingsViewModel.AddWithPath("Use light theme for System", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", new Version(10, 0));
			RegistrySettingsViewModel.AddWithPath("Always hide scrollbars in modern apps", @"Control Panel\Accessibility", "DynamicScrollbars", new Version(10, 0));
			RegistrySettingsViewModel.AddWithPath("Show Windows version on desktop", @"Control Panel\Desktop", "PaintDesktopVersion", new Version(6, 1));
		}

		private void InitColors()
		{
			ColorSettings = new ColorAppearanceSetting[] { //I - implemented, NI - not implemented, NU - not used (in Windows)
                new ColorAppearanceSetting("Active Title Color 1", string.Empty, "ActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Color 2", string.Empty, "GradientActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Text", string.Empty, "TitleText"), //I
                new ColorAppearanceSetting("Active Window Border", string.Empty, "ActiveBorder"), //I
                new ColorAppearanceSetting("Application Background (Deprecated)", string.Empty, "AppWorkspace"), //NU
                new ColorAppearanceSetting("Button Alternate Face (Deprecated)", string.Empty, "ButtonAlternateFace"), // NU
                new ColorAppearanceSetting("Button Dark Shadow (Right & Bottom Border)", string.Empty, "ButtonDkShadow"),//I
                new ColorAppearanceSetting("Button Face / 3D Objects", string.Empty, "ButtonFace"),//I
                new ColorAppearanceSetting("Button Hilight (Edge of Top & Left Border)", string.Empty, "ButtonHiLight"), //I
                new ColorAppearanceSetting("Button Light (Top & Left Border)", string.Empty, "ButtonLight"), //I
                new ColorAppearanceSetting("Button Shadow (Edge of Right & Bottom Border)", string.Empty, "ButtonShadow"), //I
                new ColorAppearanceSetting("Button Text Color", string.Empty, "ButtonText"), //I
                new ColorAppearanceSetting("Caption Buttons Width", "CaptionWidth", string.Empty), //I
                new ColorAppearanceSetting("Desktop", string.Empty, "Background"),//I
                new ColorAppearanceSetting("Gray Text", string.Empty, "GrayText"), //I
                new ColorAppearanceSetting("Hilight (Selection Border)", string.Empty, "Hilight"), //I
                new ColorAppearanceSetting("Hilighted Text", string.Empty, "HilightText"), //I
                new ColorAppearanceSetting("Hypertext Link / Hilight Selection (Fill)", string.Empty, "HotTrackingColor"), //I
                new ColorAppearanceSetting("Icon Size", "Shell Icon Size", string.Empty), //I
                new ColorAppearanceSetting("Icon Horizontal Spacing", "IconSpacing", string.Empty), //I
                new ColorAppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", string.Empty), //I
                new ColorAppearanceSetting("Inactive Title Color 1", string.Empty, "InactiveTitle"), //I
                new ColorAppearanceSetting("Inactive Title Color 2", string.Empty, "GradientInactiveTitle"), //I
                new ColorAppearanceSetting("Inactive Title Text", string.Empty, "InactiveTitleText"), //I
                new ColorAppearanceSetting("Inactive Window Border", string.Empty, "InactiveBorder"),
				new ColorAppearanceSetting("Menu", "MenuHeight", "Menu"),//I
                new ColorAppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar"), //I, NU
                new ColorAppearanceSetting("Selected Items", string.Empty, "MenuHilight"), //NI
                new ColorAppearanceSetting("Tool Tip", string.Empty, "InfoWindow"), //I
                new ColorAppearanceSetting("Window", string.Empty, "Window"),//I
                new ColorAppearanceSetting("Window Border Width", "BorderWidth", string.Empty), //I
                new ColorAppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame"), //NU, I
                new ColorAppearanceSetting("Window Title Height / Caption Buttons Height", "CaptionHeight", string.Empty) //., I
            };
		}

		private void InitFonts()
		{
			//TO DO: add font weight and font family bindings
			FontSettings = new FontAppearanceSetting[] {
				new FontAppearanceSetting("Active / Inactive Title Font", "CaptionFont", string.Empty), //., I
                new FontAppearanceSetting("Icon Font", "IconFont", string.Empty),
				new FontAppearanceSetting("Menu Font", "MenuFont", "MenuText"), //I, I
                new FontAppearanceSetting("Palette Title Font", "SmCaptionFont", string.Empty),
				new FontAppearanceSetting("Status Font", "StatusFont", "InfoText"),//I, I
                new FontAppearanceSetting("Window Text Font", "MessageFont", "WindowText") //I, I
            };
		}

		private void InitWallpaper()
		{
			var bgColor = ColorSettings[13];
			if (bgColor == null)
				throw new Exception("Background color is not loaded");
			if (!bgColor.HasColor)
				bgColor.ItemColor = Color.Black;
			WallpaperSettings = new WallpaperSettings(bgColor);
		}

		#endregion Initialization

		#region Processes

		public static void RestartDWM()
		{
			var processes = Process.GetProcesses();
			foreach (Process p in processes)
			{
				if (p.ProcessName == "dwm")
				{
					try
					{
						p.Kill();
					}
					catch (Exception ex)
					{
						Console.WriteLine("cannot kill dwm.exe, error: " + ex.Message);
					}
				}
			}
		}

		public static void RestartExplorer()
		{
			var processes = Process.GetProcesses();
			foreach (Process p in processes)
			{
				if (p.ProcessName == "explorer")
				{
					try
					{
						p.Kill();
					}
					catch (Exception ex)
					{
						Console.WriteLine("cannot kill explorer.exe, error: " + ex.Message);
					}
				}
			}

			System.Threading.Thread.Sleep(1000);
			if (Process.GetProcessesByName("explorer").Length == 0)
				Process.Start("explorer.exe");
		}

		public void ShowThemesControlPanel()
		{
			Process.Start("explorer", "shell:::{ED834ED6-4B5A-4bfe-8F11-A626DCB6A921}");
		}

		public void OpenThemesFolder()
		{
			Process.Start("explorer", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Windows\\Themes");
		}

		#endregion Processes

		#region Save (Separate)

		private void ShowSavedToRegistryDialog()
		{
			MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
			IsSavingInProgress = false;
		}

		private void ShowSavedAsThemeDialog()
		{
			MessageBox.Show("You need to restart to apply some of these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
			IsSavingInProgress = false;
		}

		private void ShowSavedSuccessfullyDialog()
		{
			MessageBox.Show("Theme applied successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
			IsSavingInProgress = false;
		}

		internal async Task SaveThemeModesToRegistry()
		{
			await RegistrySettingsViewModel.SaveAll();
		}

		internal async Task SaveThemeAsTheme()
		{
			IsSavingInProgress = true;
			await ThemeSettings.SaveTheme();
			await AeroColorsViewModel.SaveAll();
			ShowSavedSuccessfullyDialog();
		}

		internal async Task SaveTitleColorsAsTheme()
		{
			IsSavingInProgress = true;
			await ThemeSettings.SaveTitleColors();
			await Task.Delay(2000);
			App.Current.Resources["ThemeColor"] = Converters.BrushToColorConverter.MediaColorToBrush(ThemeColor.ItemColor);
			// Save "Enable colored titlebars"
			if (IsWindows10 && RegistrySettingsViewModel.RegistrySettings.Count > 0)
			{
				RegistrySettingsViewModel.RegistrySettings[0].SaveToRegistry();
			}
			await AeroColorsViewModel.SaveAll();
			ShowSavedSuccessfullyDialog();
		}

		internal async Task SaveTitleColorsToRegistry()
		{
			IsSavingInProgress = true;
			await AeroColorsViewModel.SaveAll();
			await Task.Run(ThemeColor.SaveToRegistry);
			ShowSavedToRegistryDialog();
		}

		internal async Task SaveColorsMetricsAsTheme()
		{
			IsSavingInProgress = true;
			await ThemeSettings.SaveColorsAndMetrics();
			ShowSavedToRegistryDialog();
			await AeroColorsViewModel.SaveAll();
		}

		internal async Task SaveColorsMetricsToRegistry()
		{
			IsSavingInProgress = true;
			List<Task> appliedSettingsTasks = new List<Task>();
			foreach (var setting in ColorSettings)
			{
				if (setting == null) continue;
				if (!setting.IsEdited) continue;
				appliedSettingsTasks.Add(Task.Run(setting.SaveToRegistry));
				setting.IsEdited = false;
			}
			await Task.WhenAll(appliedSettingsTasks);
			ShowSavedToRegistryDialog();
		}

		internal async Task SaveFontsAsTheme()
		{
			IsSavingInProgress = true;
			await ThemeSettings.SaveFonts();
			ShowSavedAsThemeDialog();
			await AeroColorsViewModel.SaveAll();
		}

		internal async Task SaveFontsToRegistry()
		{
			IsSavingInProgress = true;
			List<Task> appliedSettingsTasks = new List<Task>();
			foreach (var font in FontSettings)
			{
				if (font == null) continue;
				if (!font.IsEdited) continue;
				appliedSettingsTasks.Add(Task.Run(font.SaveToRegistry));
				font.IsEdited = false;
			}
			await Task.WhenAll(appliedSettingsTasks);
			ShowSavedToRegistryDialog();
		}

		internal async Task SaveWallpaperAsTheme()
		{
			IsSavingInProgress = true;
			await ThemeSettings.SaveWallpaper();
			ShowSavedSuccessfullyDialog();
			await AeroColorsViewModel.SaveAll();
		}

		internal async Task SaveWallpaperToRegistry()
		{
			IsSavingInProgress = true;
			await Task.Run(WallpaperSettings.SaveToRegistry);
			ShowSavedToRegistryDialog();
		}

		internal async Task SaveRegistrySettingsToRegistry()
		{
			IsSavingInProgress = true;
			await RegistrySettingsViewModel.SaveAll();
			ShowSavedToRegistryDialog();
		}

		#endregion Save (Separate)

		#region Restore

		public void RunRegFile(string path)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "reg.exe";
			startInfo.Arguments = "IMPORT \"" + path + "\"";
			Process.Start(startInfo);
		}

		internal async Task ResetFonts()
		{
			RunRegFile("Window Metrics fix.reg");
			await Task.Delay(100);
		}

		internal async Task ResetColors()
		{
			RunRegFile("Colors fix.reg");
			await Task.Delay(100);
		}

		internal async Task ResetDWM()
		{
			RunRegFile("DWM fix.reg");
			await Task.Delay(100);
		}

		internal async Task ResetToDefaults()
		{
			var taskList = new List<Task>();
			taskList.Add(ResetFonts());
			taskList.Add(ResetColors());
			taskList.Add(ResetDWM());
			await Task.WhenAll(taskList);
			await ResetTheme();
		}

		internal async Task ResetTheme()
		{
			await ThemeSettings.ApplyTheme(@"C:\Windows\Resources\Themes\aero.theme");
			await Task.Delay(1000);
		}

		#endregion Restore

		#region Export

		private string GetColorsInReg()
		{
			string output = @"[HKEY_CURRENT_USER\Control Panel\Colors]";

			foreach (var c in ColorSettings)
			{
				if (!c.HasColor)
					continue;
				output += "\n\"" + c.ColorRegistryPath + "\"=\"" + c.ItemColorValue + "\"";
			}

			output += Environment.NewLine + Environment.NewLine;
			output += @"[HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics]";

			foreach (var c in ColorSettings)
			{
				if (!c.HasSize)
					continue;
				if (c.SizeRegistryKey == "Shell Icon Size")
					output += "\n\"" + c.SizeRegistryKey + "\"=\"" + (c.Size) + "\"";
				else
					output += "\n\"" + c.SizeRegistryKey + "\"=\"" + (-15 * c.Size) + "\"";
			}

			return output;
		}

		private string GetFontsInReg()
		{
			string output = string.Empty;

			foreach (var f in FontSettings)
			{
				if (!f.HasSize)
					continue;
				string valueText = "hex:";
				foreach (byte b in f.GetFontInRegistryFormat())
				{
					valueText += b.ToString("X") + ",";
				}
				valueText = valueText.Remove(valueText.Length - 1, 1);
				output += "\n\"" + f.FontRegistryPath + "\"=" + valueText;
			}

			output += Environment.NewLine + Environment.NewLine;
			output += @"[HKEY_CURRENT_USER\Control Panel\Colors]";
			foreach (var f in FontSettings)
			{
				if (!f.HasColor)
					continue;
				output += "\n\"" + f.ColorRegistryPath + "\"=\"" + f.ItemColorValue + "\"";
			}

			output += Environment.NewLine + Environment.NewLine;
			return output;
		}

		public async Task ExportToReg()
		{
			string textToSave = "Windows Registry Editor Version 5.00\n\n";
			textToSave += GetColorsInReg();
			textToSave += GetFontsInReg();
			textToSave += RegistrySettingsViewModel.GetSettingsInReg();
			textToSave += AeroColorsViewModel.GetSettingsInReg();
			Console.WriteLine(textToSave); //debugging solution

			string path = Environment.CurrentDirectory + "\\Exported Settings";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string fileName = "Export" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".reg";

			var sw = new StreamWriter(path + "\\" + fileName);
			await sw.WriteAsync(textToSave);
			sw.Close();
		}

		#endregion Export
	}
}
