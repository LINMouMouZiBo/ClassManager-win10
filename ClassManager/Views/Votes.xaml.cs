using ClassManager.Controls;
using ClassManager.Model;
using ClassManager.Service;
using ClassManager.ViewModels;
using Newtonsoft.Json.Linq;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace ClassManager.Views
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class Votes : Page
    {
        private OrganizationViewModel OVM;
        private UserViewModel UVM;

        bool isPowerful = false;

        public Votes ()
		{
			this.InitializeComponent();
            OVM = OrganizationViewModel.Instance;
            UVM = UserViewModel.Instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            OVM.initialOVM((string)e.Parameter);
            isPowerful = false;
            foreach (var item in UVM.User.Relationships)
            {
                if (item.account == OVM.Organization.Account && (item.position == "founder" || item.position == "manager"))
                {
                    isPowerful = true;
                }
            }
        }

        private async void option_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dialog = new ContentDialog()
            {
                Title = "确定为其投票？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (_s, _e) => {
                string voteId = ((Vote)((Grid)((ListView)sender).Parent).DataContext)._id;
                string optionId = ((Option)e.ClickedItem)._id;
                OVM.vote(OVM.Organization.Account, voteId, optionId);
            };
            await dialog.ShowAsync();
        }
        
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog()
            {
                Title = "新建投票",
                Content = new CreateVoteContent(),
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (_s, _e) =>
            {
                    JObject jo = new JObject();
                    jo.Add("name", ((CreateVoteContent)_s.Content).getName());
                    jo.Add("content", ((CreateVoteContent)_s.Content).getContent());
                    jo.Add("deadline", ((CreateVoteContent)_s.Content).getDeadline());
                    jo.Add("options", ((CreateVoteContent)_s.Content).getOptions().ToString());

                    OVM.createVote(jo.ToString());
            };
            await dialog.ShowAsync();
        }
    }

    public class SupporterNumCVT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "已有" + ((List<User>)value).Count + "人支持";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class IsVotedCVT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVoted = true;

            List<User> unvotes = ((List<User>)value);
            foreach (User item in unvotes)
            {
                if (item.Account == UserViewModel.Instance.User.Account)
                {
                    isVoted = false;
                }
            }

            return !isVoted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsVotedTextCVT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVoted = true;

            List<User> unvotes = ((List<User>)value);
            foreach (User item in unvotes)
            {
                if (item.Account == UserViewModel.Instance.User.Account)
                {
                    isVoted = false;
                }
            }

            return isVoted ? "已投票" : "未投票";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedCVT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int index = -1;

            List<Option> options = ((List<Option>)value);
            for (int i = 0; i < options.Count; ++i)
            {
                List<User> supporters = options[i].supporters;
                foreach (User supporter in supporters)
                {
                    if (supporter.Account == UserViewModel.Instance.User.Account)
                    {
                        index = i;
                        break;
                    }
                    if (index != -1) break;
                }
            }

            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class UnvotesNumCVT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "还有" + ((List<User>)value).Count + "人未投票";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
