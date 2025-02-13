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

namespace AdvancedWindowsAppearence.Controls
{
	/// <summary>
	/// Interaction logic for WallpaperControl.xaml
	/// </summary>
	public partial class WallpaperControl : UserControl
	{
		public WallpaperControl()
		{
			InitializeComponent();

			double w = SystemParameters.VirtualScreenWidth;
			double h = SystemParameters.VirtualScreenHeight;
			wallpaperGrid.Width = w / 10;
			wallpaperGrid.Height = h / 10;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is WallpaperSettings)
				return;
			else if (DataContext is GeneralViewModel generalViewModel)
				DataContext = generalViewModel.WallpaperSettings;
			else
				return;
		}
	}
}
