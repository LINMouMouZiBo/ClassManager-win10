﻿using ClassManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace ClassManager.Views
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class Setting : Page
	{
		private OrganizationViewModel OVM;

        private StorageFile file = null;

        public Setting ()
		{
			this.InitializeComponent();
			OVM = OrganizationViewModel.Instance;
		}

		protected override void OnNavigatedTo (NavigationEventArgs e)
		{
			OVM.initialOVM((string)e.Parameter);
			Frame rootFrame = Window.Current.Content as Frame;
			if (rootFrame.CanGoBack) {
				// Show UI in title bar if opted-in and in-app backstack is not empty.
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
					AppViewBackButtonVisibility.Visible;
			} else {
				// Remove the UI from the title bar if in-app back stack is empty.
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
					AppViewBackButtonVisibility.Collapsed;
			}
		}

		private async void SelectPictureButton_Click (object sender, RoutedEventArgs e)
		{

			// 设置 file picker.
			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			openPicker.ViewMode = PickerViewMode.Thumbnail;

			openPicker.FileTypeFilter.Clear();
			openPicker.FileTypeFilter.Add(".png");
			openPicker.FileTypeFilter.Add(".jpeg");
			openPicker.FileTypeFilter.Add(".jpg");

			// 打开 file picker.
			file = await openPicker.PickSingleFileAsync();

			// 'file' is null if user cancels the file picker.
			if (file != null) {
				// Open a stream for the selected file.
				// The 'using' block ensures the stream is disposed
				// after the image is loaded.
				using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
					// Set the image source to the selected bitmap.
					BitmapImage bitmapImage = new BitmapImage();

					bitmapImage.SetSource(fileStream);
					image.Source = bitmapImage;
				}
			}
		}

		private void settingBtn_Click (object sender, RoutedEventArgs e)
		{
			Dictionary<string, string> settingData = new Dictionary<string, string>();
			settingData.Add("name", name.Text);

			OVM.setOrganizationData(OVM.Organization.Account, settingData);
		}

        private async void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (file == null)
            {
                await new Windows.UI.Popups.MessageDialog("请选择一张图片").ShowAsync();
                return;
            }

            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
            Stream stream = fileStream.AsStream();

            OVM.uploadImage(stream, file.Name);
        }

        private async void setPasswordBtn_Click (object sender, RoutedEventArgs e)
		{
			if (password.Password != password_confirm.Password) {
				await new Windows.UI.Popups.MessageDialog("两次输入法的密码不一致").ShowAsync();
				return;
            }
            if (password.Password.Length < 6 && password.Password.Length != 0)
            {
                await new Windows.UI.Popups.MessageDialog("密码至少需要6位").ShowAsync();
                return;
            }
            if (password.Password.Length == 0)
            {
                var dialog = new ContentDialog()
                {
                    Content = "不提供密码将清除本班密码，是否继续？",
                    PrimaryButtonText = "是",
                    SecondaryButtonText = "否",
                    FullSizeDesired = false,
                };

                dialog.PrimaryButtonClick += (_s, _e) =>
                {
                    OVM.setPasswprd(password.Password);
                };
                await dialog.ShowAsync();
            }
            else
            {
                OVM.setPasswprd(password.Password);
            }
		}
	}

	class MaleCVT : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, string language)
		{
			if (value == null) return false;
			return (string)value == "male" ? true : false;
		}

		public object ConvertBack (object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
	class FemaleCVT : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, string language)
		{
			if (value == null) return false;
			return (string)value == "female" ? true : false;
		}

		public object ConvertBack (object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
