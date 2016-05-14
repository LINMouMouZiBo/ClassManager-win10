using ClassManager.Model;
using ClassManager.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace ClassManager.ViewModels
{
    class UserViewModel
    {
        static private UserViewModel _instance;
        static public UserViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserViewModel();
                }
                return _instance;
            }
        }

        private User user;
        public User User
        {
            get { return user; }
        }

        private UserViewModel()
        {
            user = new User();
        }

        public void initialUVM()
        {
            User u = SingleService.Instance.getUser();

            User.DeepCopy(u);
			creat_tile();
		}

		//使用最新创建的Homework来更新磁贴，在每次initialUVM和regetUser调用
		private void creat_tile ()
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(File.ReadAllText("Controls/tiles.xml"));
			var elements = xml.GetElementsByTagName("text");
			List<UserHomework> homs = new List<UserHomework>();
			homs = User.Homeworks.ToList();
			if (homs.Count < 1) {
				return;
			}
			homs.Sort((a, b) => {
				DateTime tem1 = Convert.ToDateTime(a.join_on);
				DateTime tem2 = Convert.ToDateTime(a.join_on);
				return tem1.CompareTo(tem2);
			});
			for (int i = 0; i < 12; i = i + 3) {
				elements[i].InnerText = homs.First().name;
				elements[i + 1].InnerText = "ddl:" + Convert.ToDateTime(homs.First().deadline).ToString("yyyy/MM/dd");
				elements[i + 2].InnerText = homs.First().content;
			}

			var updator = TileUpdateManager.CreateTileUpdaterForApplication();
			var notification = new TileNotification(xml);
			updator.Update(notification);
		}

		public async void setPasswprd(string password)
        {
            Dictionary<string, string> settingData = new Dictionary<string, string>();
            settingData.Add("password", password);

            Result result = await SingleService.Instance.userSetting(settingData);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("密码修改成功").ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void setUserData(Dictionary<string, string> settingData)
        {
            Result result = await SingleService.Instance.userSetting(settingData);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("修改成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void createOrganization(string account, string password)
        {
            Result result = await SingleService.Instance.registerOrganization(account, password);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("创建班级成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async Task<Result> searchOrganization(string account)
        {
            return await SingleService.Instance.searchOrganization(account);
        }

        public async void joinOrganizationWithPassword(string account, string password)
        {
            Result result = await SingleService.Instance.joinWithPassword(account, password);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("加入班级成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void deleteUserHomework(string account, string _id)
        {
            Result result = await SingleService.Instance.deleteHomework(account, _id);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("删除成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void joinOrganizationWithoutPassword(string account)
        {
            Result result = await SingleService.Instance.joinWithoutPassword(account);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("加入班级成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void deleteOrganization(string account)
        {
            Result result = await SingleService.Instance.deleteOrganization(account);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("解散班级成功").ShowAsync();

                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public void logout()
        {
            User.DeepCopy(new User());
        }

        public async void complishHomework(string homeworkId, bool uncomplete)
        {
            Result result = await SingleService.Instance.complishHomework(homeworkId, uncomplete);
            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("homework标记为" + (uncomplete ? "未完成" : "完成")).ShowAsync();
                regetUser();
            }
            else {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public void updateUser()
        {
            regetUser();
        }

        public async void uploadImage(Stream stream, string filename)
        {
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            StreamContent sc = new StreamContent(stream);
            sc.Headers.Add("Content-Type", "image/*");

            multipartContent.Add(sc, "image", filename);

            Result result = await SingleService.Instance.userSetting(multipartContent);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("修改成功").ShowAsync();
                regetUser();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        // 私有方法 重新获取user数据
        private async void regetUser()
        {
            Result result = await SingleService.Instance.updateUser();

            if (!result.error)
            {
                User u = SingleService.Instance.getUser();
                u.Name = u.Name == null ? "（还没有设置）" : u.Name;

                foreach (var item in u.Relationships)
                {
                    item.name = item.name == null ? "（还没有设置）" : item.name;
                }

                User.DeepCopy(u);
				creat_tile();
			}
            else
            {
                await new Windows.UI.Popups.MessageDialog("当前网络状况不佳").ShowAsync();
            }
        }
    }
}