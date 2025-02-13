﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvancedWindowsAppearence.Previews
{
	/// <summary>
	/// Interaction logic for ColorBackgroundSelectionPage.xaml
	/// </summary>
	public partial class ColorBackgroundSelectionPage : Page
	{
		private WallpaperSettings WallpaperSettings;

		public ColorBackgroundSelectionPage(WallpaperSettings wallpaperSettings)
		{
			WallpaperSettings = wallpaperSettings;
			InitializeComponent();
			this.DataContext = WallpaperSettings;
		}

		private void ChangeBgColorButton_Click(object sender, RoutedEventArgs e)
		{
			//open color picker dialog
			WallpaperSettings.BackgroundColor.ItemColor = MainWindow.OpenColorDialog(WallpaperSettings.BackgroundColor.ItemColor);
		}
	}
}
