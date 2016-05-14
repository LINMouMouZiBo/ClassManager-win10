using ClassManager.Model;
using ClassManager.Service;
using ClassManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace ClassManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class JoinOrganizationPage : Page
    {
        public JoinOrganizationPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Result result = await UserViewModel.Instance.searchOrganization(account.Text);
            
            if (!result.error)
            {
                image.Visibility = Visibility.Visible;
                name.Visibility = Visibility.Visible;
                need_password.Visibility = Visibility.Visible;
                confirmBtn.Visibility = Visibility.Visible;

                if (result.organization_data.need_password)
                {
                    need_password.Text = "加入该班级需要密码";
                    password.Visibility = Visibility.Visible;
                }
                else
                {
                    need_password.Text = "加入该班级不需要密码";
                    password.Visibility = Visibility.Collapsed;
                }

                //image.Source = SingleService.Instance.getServerAddress() + (result.organization_data.Image == null ? "null" : result.organization_data.Image);
                name.Text = result.organization_data.name == null ? "" : result.organization_data.name;
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
                image.Visibility = Visibility.Collapsed;
                name.Visibility = Visibility.Collapsed;
                need_password.Visibility = Visibility.Collapsed;
                password.Visibility = Visibility.Collapsed;
                confirmBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (password.Visibility == Visibility.Visible)
            {
                UserViewModel.Instance.joinOrganizationWithPassword(account.Text, password.Password);
            }
            else
            {
                UserViewModel.Instance.joinOrganizationWithoutPassword(account.Text);
            }
			Frame.Navigate(typeof(HomePage));
		}
    }
}
