﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WindowsInteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{

    public class SlideshowImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Path { get; set; }
        private bool _isSelected;
        private double _opacity;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value is bool sel)
                {
                    if (sel)
                        Opacity = 1d;
                    else
                        Opacity = 0.5d;
                }
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }
        public double Opacity
        {
            get => _opacity; set
            {
                _opacity = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class SlideshowSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        List<string> SelectedPaths { get; set; } = new List<string>();
        public IntRegistrySetting Interval { get; }
        public BoolRegistrySetting Shuffle { get; } 

        private string _folder;
        public string Folder { get => _folder; set
            {
                _folder = value;
                NotifyPropertyChanged();
                FolderImages = GetImagesFromDirectory(Folder);
            }
        }
        public ObservableCollection<SlideshowImage> FolderImages { get; private set; } = null;

        private ObservableCollection<SlideshowImage> GetImagesFromDirectory(string directory)
        {
                if (Directory.Exists(directory))
                {
                    string[] supportedTypes = new string[] { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif", ".tif" };
                    string[] files = Directory.GetFiles(Folder);

                    var imagePaths = from file in files
                                     where supportedTypes.Any(type => file.EndsWith(type))
                                     select file;
                    List<SlideshowImage> images = new List<SlideshowImage>();
                    foreach (string image in imagePaths)
                    {
                        var slideshowImage = new SlideshowImage();
                        slideshowImage.Path = image;
                        slideshowImage.IsSelected = false;
                        images.Add(slideshowImage);
                    }
                    return new ObservableCollection<SlideshowImage>(images);
                }
                return null;
        }
        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SlideshowSettings()
        {

            Shuffle = new BoolRegistrySetting("Shuffle", @"Control Panel\Personalization\Desktop Slideshow", "Shuffle");
            Interval = new IntRegistrySetting("Interval", @"Control Panel\Personalization\Desktop Slideshow", "Interval");
        }
        internal void SetFolder(string path)
        {
            Folder = path;
        }

        /// <summary>
        /// shows file (folder) dialog asking for the folder where are the wallpapers located
        /// </summary>
        public void ShowFolderDialogSlideshow()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            if (!string.IsNullOrWhiteSpace(Folder))
                dialog.InitialDirectory = Folder;
            dialog.Title = "Select a folder for slideshow";
            dialog.Filter = "Directory|*.this.directory"; // prevents displaying files
            dialog.FileName = "select";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                Folder = path;
            }
        }

        public void DeleteIni()
        {
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";
            File.Delete(iniPath);
        }

        string CryptBinaryToString(string text) 
        {
            IntPtr pidl = Win32Methods.ILCreateFromPath(text);

            uint CRYPT_STRING_BASE64 = 0x00000001;

            int k = 0;
            short cb = 0;

            while ((k = Marshal.ReadInt16(pidl + cb)) > 0)
            {
                cb += (short)k;
            }
            int size = 0;
            Win32Methods.CryptBinaryToString(pidl, cb, CRYPT_STRING_BASE64, null, ref size);
            StringBuilder stringBuilder = new StringBuilder(size);
            if (Win32Methods.CryptBinaryToString(pidl, cb, CRYPT_STRING_BASE64, stringBuilder, ref size))
            {
                return stringBuilder?.ToString();
            }
            return null;
        }

        private void CreateNewIni()
        {
            FolderImages = new ObservableCollection<SlideshowImage>() { 
                new SlideshowImage() { Path = @"F:\gamez\ManiaPlanet\UserData\Worktitles\TM2U_Island@adamkooo\Media\Images\Graphics\LoadscreenCurrent.png", IsSelected = true } 
            };

            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";
            StringBuilder iniContentStringBuilder = new StringBuilder();
            
            foreach (var image in FolderImages)
            {
                if (image == null)
                    continue;
                if (!image.IsSelected)
                    continue;
                if (string.IsNullOrWhiteSpace(image.Path))
                    continue;
                iniContentStringBuilder.Append(CryptBinaryToString(image.Path));
            }
        }
    }
}
