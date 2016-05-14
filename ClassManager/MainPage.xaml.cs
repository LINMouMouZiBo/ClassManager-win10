using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassManager.Service;
using ClassManager.Model;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ClassManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

            //input_account.Text = "14330015";
            //input_password.Password = "123456";
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

        }

        private async void onClickLogin(object sender, RoutedEventArgs e)
        {
            Result result = await SingleService.Instance.login(input_account.Text, input_password.Password);
            if (result.error)
            {
                var i = new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
                return;
            }
            Debug.WriteLine(JsonConvert.SerializeObject(result));
            if (SingleService.Instance.hasLogin())
            {
                Frame.Navigate(typeof(HomePage));
            }
        }

        private async void onClickRegister(object sender, RoutedEventArgs e)
        {
            Result result = await SingleService.Instance.registerUser(input_account.Text, input_password.Password);
            Debug.WriteLine(JsonConvert.SerializeObject(result));
            var i = new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
        }
    }
}
