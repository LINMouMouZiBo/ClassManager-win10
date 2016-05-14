using ClassManager.Model;
using ClassManager.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ClassManager.ViewModels
{
	class OrganizationViewModel
	{
		static private OrganizationViewModel _instance;
		static public OrganizationViewModel Instance {
			get {
				if (_instance == null) {
					_instance = new OrganizationViewModel();
				}
				return _instance;
			}
		}

		private Organization organization;
		public Organization Organization {
			get { return organization; }
		}

		private OrganizationViewModel ()
		{
			organization = new Organization();
			organization.Image = SingleService.Instance.getServerAddress() + "null";
		}

		public async void initialOVM (string account)
		{
			Result res = await SingleService.Instance.searchOrganizationDetail(account);
			if (!res.error) {
                res.organization_data.image = SingleService.Instance.getServerAddress() + (res.organization_data.image == null ? "null" : res.organization_data.image);
				foreach (var item in res.organization_data.members) {
					item.image = SingleService.Instance.getServerAddress() + (item.image == null ? "null" : item.image);
				}
				foreach (var item in res.organization_data.notices) {
					item.image = SingleService.Instance.getServerAddress() + (item.image == null ? "null" : item.image);
					item.unlooksNum = item.unlooks.Count;
				}
				foreach (var item in res.organization_data.Homeworks) {
					item.unlooksNum = item.unlooks.Count;
				}

                Organization.DeepCopy(res.organization_data);
            } else {
				await new Windows.UI.Popups.MessageDialog(res.message).ShowAsync();
			}
		}
        
		public async void setOrganizationData (string account, Dictionary<string, string> settingData)
		{
			Result result = await SingleService.Instance.organizationSetting(account, settingData);

			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("修改成功").ShowAsync();
				initialOVM(account);
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}
        
		public async void vote (string organizationAccount, string voteId, string optionId)
		{
			Result result = await SingleService.Instance.vote(organizationAccount, voteId, optionId);

			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("投票成功").ShowAsync();

				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void createVote (string string_content)
		{
			Result result = await SingleService.Instance.createVote(Organization.Account, string_content);

			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("创建成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		// 私有方法 重新获取organization数据
		private async void regetOrganization ()
		{
			Result result = await SingleService.Instance.searchOrganizationDetail(Organization.Account);

			if (!result.error)
            {
                result.organization_data.image = SingleService.Instance.getServerAddress() + (result.organization_data.image == null ? "null" : result.organization_data.image);
                foreach (var item in result.organization_data.members)
                {
                    item.image = SingleService.Instance.getServerAddress() + (item.image == null ? "null" : item.image);
                }
                foreach (var item in result.organization_data.notices)
                {
                    item.image = SingleService.Instance.getServerAddress() + (item.image == null ? "null" : item.image);
                    item.unlooksNum = item.unlooks.Count;
                }
                foreach (var item in result.organization_data.Homeworks)
                {
                    item.unlooksNum = item.unlooks.Count;
                }

                Organization.DeepCopy(result.organization_data);
            } else {
				await new Windows.UI.Popups.MessageDialog("当前网络状况不佳").ShowAsync();
			}
		}

		public async void createHomeWork (Dictionary<string, string> HomeWorkData)
		{
			Result result = await SingleService.Instance.createHomework(Organization.Account, HomeWorkData);

			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("创建成功").ShowAsync();
				regetOrganization();
				UserViewModel.Instance.updateUser();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void createNotice (Dictionary<string, string> NoticeData)
		{
			Result result = await SingleService.Instance.createNotice(Organization.Account, NoticeData);

			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("创建成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

        public async void createNotice(MultipartFormDataContent multipartContent)
        {
            Result result = await SingleService.Instance.createNotice(Organization.Account, multipartContent);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("创建成功").ShowAsync();
                regetOrganization();
            }
            else {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

        public async void deleteMember (string organizationAccount, string memberAccount)
		{
			Result result = await SingleService.Instance.deleteMember(organizationAccount, memberAccount);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("删除Member成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void deleteHomework (string organizationAccount, string homeworkId)
		{
			Result result = await SingleService.Instance.deleteHomework(organizationAccount, homeworkId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("删除homework成功").ShowAsync();
				regetOrganization();
				UserViewModel.Instance.updateUser();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void deleteNotice (string organizationAccount, string noticeId)
		{
			Result result = await SingleService.Instance.deleteNotice(organizationAccount, noticeId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("删除Notice成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void lookHomework (string organizationAccount, string homeworkId)
		{
			Result result = await SingleService.Instance.lookHomework(organizationAccount, homeworkId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("Homework标记为已查看").ShowAsync();
				regetOrganization();
				UserViewModel.Instance.updateUser();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void lookNotice (string organizationAccount, string noticeId)
		{
			Result result = await SingleService.Instance.lookNotice(organizationAccount, noticeId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("Notice标记为已查看").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void UpMember (string organizationAccount, string memberId)
		{
			Result result = await SingleService.Instance.upMember(organizationAccount, memberId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("提升权限成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

		public async void downMember (string organizationAccount, string memberId)
		{
			Result result = await SingleService.Instance.downMember(organizationAccount, memberId);
			if (!result.error) {
				await new Windows.UI.Popups.MessageDialog("免除权限成功").ShowAsync();
				regetOrganization();
			} else {
				await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
			}
		}

        public async void uploadImage(Stream stream, string filename)
        {
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(stream), "image", filename);

            Result result = await SingleService.Instance.organizationSetting(multipartContent, Organization.Account);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("修改成功").ShowAsync();

                UserViewModel.Instance.updateUser();
                regetOrganization();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }


        public async void setPasswprd(string password)
        {
            Dictionary<string, string> settingData = new Dictionary<string, string>();
            settingData.Add("password", password);

            Result result = await SingleService.Instance.organizationSetting(Organization.Account, settingData);

            if (!result.error)
            {
                await new Windows.UI.Popups.MessageDialog("密码修改成功").ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog(result.message).ShowAsync();
            }
        }

    }
}
