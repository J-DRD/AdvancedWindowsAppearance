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
using System.Windows.Shapes;
using System.Windows.Shell;

namespace AdvancedWindowsAppearence
{
	/// <summary>
	/// Interaction logic for ModernWindow.xaml
	/// </summary>
	public partial class ModernWindow : Window
	{
		private bool IsLightMode = true;
		private int MaximizeOffset = 5;

		public ModernWindow(bool? isLightMode)
		{
			if (isLightMode != null)
				IsLightMode = isLightMode.Value;
			UpdateTheme(IsLightMode);
			InitializeComponent();
		}

		/// <summary>
		/// Update Ui by the selected theme
		/// </summary>
		public void UpdateTheme(bool isLightMode)
		{
			if (!isLightMode) //force dark mode
			{
				App.Current.Resources["ButtonFaceColor"] = new BrushConverter().ConvertFromString("#FF404040");
				App.Current.Resources["BackgroundColor"] = Brushes.Black;
				App.Current.Resources["BackgroundColorTabItems"] = new BrushConverter().ConvertFromString("#9F252525");
				App.Current.Resources["ForegroundColor"] = Brushes.White;
			}
			else
			{
				App.Current.Resources["ButtonFaceColor"] = SystemColors.ControlBrush;
				App.Current.Resources["BackgroundColor"] = SystemColors.WindowBrush;
				App.Current.Resources["BackgroundColorTabItems"] = new BrushConverter().ConvertFromString("#BFFFFFFF");
				App.Current.Resources["ForegroundColor"] = SystemColors.WindowTextBrush;
			}
			IsLightMode = isLightMode;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WindowRounding.RoundWindow(this);
			WindowShadow.DropShadowToWindow(this);
			WindowBlur.SetIsEnabled(this, true);
			AdjustBorder();
		}

		/// <summary>
		/// if the window rounding is enabled, adjust the border to it
		/// </summary>
		private void AdjustBorder()
		{
			if (!WindowRounding.IsRoundingEnabled.HasValue)
				return;

			if (!WindowRounding.IsRoundingEnabled.Value) //if is rounding disabled
				return;
			windowBorder.CornerRadius = new CornerRadius(8.0);
			windowBorder.BorderThickness = new Thickness(2);
			MaximizeOffset = 4;
		}

		private void window_Activated(object sender, EventArgs e)
		{
			this.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#33000000");
			titlebarGrid.Opacity = 1;
			windowBorder.BorderBrush = (Brush)App.Current.Resources["ThemeColor"];
			windowBorder.Opacity = 1;
		}

		private void window_Deactivated(object sender, EventArgs e)
		{
			if (IsLightMode)
				this.Background = System.Windows.Media.Brushes.Gray;
			else
				this.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF101010");
			titlebarGrid.Opacity = 0.4;
			windowBorder.BorderBrush = Brushes.Gray;
			windowBorder.Opacity = 0.4;
		}

		#region Caption Buttons and actions

		private void Minimize()
		{
			SystemCommands.MinimizeWindow(this);
		}

		private void Maximize()
		{
			masterGrid.Margin = new Thickness(MaximizeOffset);
			SystemCommands.MaximizeWindow(this);
			maximizeButton.Content = "";
		}

		private void Restore()
		{
			masterGrid.Margin = new Thickness(0);
			SystemCommands.RestoreWindow(this);
			maximizeButton.Content = "";
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
				return;
			if (this.WindowState != WindowState.Normal)
			{
				Restore();
				return;
			}
			this.DragMove();
		}

		private void minimizeButton_Click(object sender, RoutedEventArgs e)
		{
			Minimize();
		}

		private void maximizeButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.WindowState != WindowState.Maximized)
			{
				this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
				Maximize();
			}
			else
			{
				Restore();
			}
		}

		private void closeButton_Click_1(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		#endregion Caption Buttons and actions
	}
}
