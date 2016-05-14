using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassManager.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.UI.Xaml.Media;
using System.ServiceModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ClassManager.Service
{
    public class SingleService
    {
        private static SingleService _instatnce;
        public static SingleService Instance
        {
            get
            {
                if (_instatnce == null)
                {
                    _instatnce = new SingleService();
                }
                return _instatnce;
            }
        }

        private string _serverAddress;
        private User _user;

        private System.Net.Http.HttpClient httpClient;

        private SingleService()
        {
            _serverAddress = "http://120.27.45.70:3000/";
            _user = null;

            // 自动处理cookie
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            httpClient = new System.Net.Http.HttpClient(handler);
        }


        private async Task<JObject> _post(string sub_url, Dictionary<string, string> dict)
        {
            var content = new FormUrlEncodedContent(dict);
            try
            {
                var response = await httpClient.PostAsync(_serverAddress + sub_url, content);
                var responseByte = await response.Content.ReadAsByteArrayAsync();
                string responseString = System.Text.Encoding.UTF8.GetString(responseByte);
                JObject resultJson = JObject.Parse(responseString);
                return resultJson;
            }
            catch
            {
                return null;
            }
        }
        // 重载 _post
        private async Task<JObject> _post(string sub_url, string string_content)
        {
            // E.g. a JSON string.
            StringContent stringContent = new StringContent(
                string_content,
                Encoding.UTF8,
                "application/json");
            try
            {
                var response = await httpClient.PostAsync(_serverAddress + sub_url, stringContent);
                var responseByte = await response.Content.ReadAsByteArrayAsync();
                string responseString = System.Text.Encoding.UTF8.GetString(responseByte);
                JObject resultJson = JObject.Parse(responseString);
                return resultJson;
            }
            catch
            {
                return null;
            }
        }
        // 重载 _post
        private async Task<JObject> _post(string sub_url, MultipartFormDataContent multipartContent)
        {
            try
            {
                var response = await httpClient.PostAsync(_serverAddress + sub_url, multipartContent);
                var responseByte = await response.Content.ReadAsByteArrayAsync();
                string responseString = System.Text.Encoding.UTF8.GetString(responseByte);
                JObject resultJson = JObject.Parse(responseString);
                return resultJson;
            }
            catch
            {
                return null;
            }
        }

        private async Task<JObject> _get(string sub_url)
        {
            try
            {
                var response = await httpClient.GetAsync(_serverAddress + sub_url);
                var responseByte = await response.Content.ReadAsByteArrayAsync();
                string responseString = System.Text.Encoding.UTF8.GetString(responseByte);
                JObject resultJson = JObject.Parse(responseString);
                return resultJson;
            }
            catch
            {
                return null;
            }
        }
        private async Task<JObject> _delete(string sub_url)
        {
            try
            {
                var response = await httpClient.DeleteAsync(_serverAddress + sub_url);
                var responseByte = await response.Content.ReadAsByteArrayAsync();
                string responseString = System.Text.Encoding.UTF8.GetString(responseByte);
                JObject resultJson = JObject.Parse(responseString);
                return resultJson;
            }
            catch
            {
                return null;
            }
        }
        //method
        public User getUser()
        {
            return _user;
        }
        public string getServerAddress()
        {
            return _serverAddress;
        }
        public bool hasLogin()
        {
            return (_user != null) ? true : false;
        }

        public async Task<Result> login(string account, string password)
        {
            var values = new Dictionary<string, string>
            {
                {"account", account },
                {"password", password }
            };
            JObject resultJson = await _post("login/user", values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            if (result.error == false)
            {
                result.user_data.account = (string)resultJson["user_data"]["account"];
                result.user_data.password = (string)resultJson["user_data"]["password"];
                result.user_data.name = (string)resultJson["user_data"]["name"];
                result.user_data.student_id = (string)resultJson["user_data"]["student_id"];
                result.user_data.gender = (string)resultJson["user_data"]["gender"];
                result.user_data.image = (string)resultJson["user_data"]["image"];
                result.user_data.email = (string)resultJson["user_data"]["email"];
                result.user_data.phone = (string)resultJson["user_data"]["phone"];
                result.user_data.qq = (string)resultJson["user_data"]["qq"];
                result.user_data.wechat = (string)resultJson["user_data"]["wechat"];
                List<JToken> homeworkList = resultJson["user_data"]["homeworks"].Children().ToList();
                foreach (JToken token in homeworkList)
                {
                    result.user_data.homeworks.Add(JsonConvert.DeserializeObject<UserHomework>(token.ToString()));
                }
                List<JToken> relationshipList = resultJson["user_data"]["relationships"].Children().ToList();
                foreach (JToken token in relationshipList)
                {
                    result.user_data.relationships.Add(JsonConvert.DeserializeObject<Relationship>(token.ToString()));
                }

                _user = result.user_data;
            }
            return result;
        }

        public async Task<Result> registerUser(string account, string password)
        {
            var values = new Dictionary<string, string>
            {
                {"account", account },
                {"password", password }
            };
            JObject resultJson = await _post("register/user", values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }

        public async Task<BitmapImage> getImage(string sub_image_url)
        {
            try
            {
                string full_image_url = _serverAddress + ((sub_image_url != null) ? sub_image_url : "null");
                var response = await httpClient.GetAsync(full_image_url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                Windows.UI.Xaml.Media.Imaging.BitmapImage bmp = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                await bmp.SetSourceAsync(responseStream.AsRandomAccessStream());
                return bmp;
            }
            catch
            {
                return null;
            }
        }
        public void logout()
        {
            _user = null;
        }
        public async Task<Result> updateUser()
        {
            JObject resultJson = await _get("search/user");
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            if (result.error == false)
            {
                result.user_data.account = (string)resultJson["user_data"]["account"];
                result.user_data.password = (string)resultJson["user_data"]["password"];
                result.user_data.name = (string)resultJson["user_data"]["name"];
                result.user_data.student_id = (string)resultJson["user_data"]["student_id"];
                result.user_data.gender = (string)resultJson["user_data"]["gender"];
                result.user_data.image = (string)resultJson["user_data"]["image"];
                result.user_data.email = (string)resultJson["user_data"]["email"];
                result.user_data.phone = (string)resultJson["user_data"]["phone"];
                result.user_data.qq = (string)resultJson["user_data"]["qq"];
                result.user_data.wechat = (string)resultJson["user_data"]["wechat"];
                List<JToken> homeworkList = resultJson["user_data"]["homeworks"].Children().ToList();
                foreach (JToken token in homeworkList)
                {
                    result.user_data.homeworks.Add(JsonConvert.DeserializeObject<UserHomework>(token.ToString()));
                }
                List<JToken> relationshipList = resultJson["user_data"]["relationships"].Children().ToList();
                foreach (JToken token in relationshipList)
                {
                    result.user_data.relationships.Add(JsonConvert.DeserializeObject<Relationship>(token.ToString()));
                }

                _user = result.user_data;
            }
            return result;
        }
        public async Task<Result> registerOrganization(string account, string password)
        {
            var values = new Dictionary<string, string>
            {
                {"account", account },
                {"password", password }
            };
            JObject resultJson = await _post("register/organization", values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public Result searchUser(string userAccount)
        {
            // 用户搜索不做
            return new Result();
        }
        public async Task<Result> searchOrganization(string organizationAccount)
        {
            JObject resultJson = await _get("search/organization/account/" + organizationAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            if (result.error == false)
            {
                result.organization_data.account = (string)resultJson["organization_data"]["account"];
                result.organization_data.password = (string)resultJson["organization_data"]["password"];
                result.organization_data.name = (string)resultJson["organization_data"]["name"];
                result.organization_data.image = (string)resultJson["organization_data"]["image"];
                result.organization_data.need_password = (bool)resultJson["organization_data"]["need_password"];
            }
            return result;
        }
        public async Task<Result> searchOrganizationDetail(string organizationAccount)
        {
            JObject resultJson = await _get("search/organization/" + organizationAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            if (result.error == false)
            {
                result.organization_data._id = (string)resultJson["organization_data"]["_id"];
                result.organization_data.account = (string)resultJson["organization_data"]["account"];
                result.organization_data.password = (string)resultJson["organization_data"]["password"];
                result.organization_data.name = (string)resultJson["organization_data"]["name"];
                result.organization_data.image = (string)resultJson["organization_data"]["image"];
                result.organization_data.join_on = (string)resultJson["organization_data"]["join_on"];
                List<JToken> homeworkList = resultJson["organization_data"]["homeworks"].Children().ToList();
                foreach (JToken token in homeworkList)
                {
                    result.organization_data.homeworks.Add(JsonConvert.DeserializeObject<Homework>(token.ToString()));
                }
                List<JToken> noticeList = resultJson["organization_data"]["notices"].Children().ToList();
                foreach (JToken token in noticeList)
                {
                    result.organization_data.notices.Add(JsonConvert.DeserializeObject<Notice>(token.ToString()));
                }
                List<JToken> memberList = resultJson["organization_data"]["members"].Children().ToList();
                foreach (JToken token in memberList)
                {
                    result.organization_data.members.Add(JsonConvert.DeserializeObject<User>(token.ToString()));
                }
                List<JToken> voteList = resultJson["organization_data"]["votes"].Children().ToList();
                foreach (JToken voteToken in voteList)
                {
                    result.organization_data.votes.Add(JsonConvert.DeserializeObject<Vote>(voteToken.ToString()));
                }
            }
            return result;
        }
        public async Task<Result> userSetting(Dictionary<string, string> settingData)
        {
            JObject resultJson = await _post("settings/user", settingData);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> organizationSetting(string organizationAccount, Dictionary<string, string> settingData)
        {
            JObject resultJson = await _post("settings/organization/" + organizationAccount, settingData);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> joinWithoutPassword(string organizationAccount)
        {
            JObject resultJson = await _get("join/organization/" + organizationAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> joinWithPassword(string organizationAccount, string password)
        {
            var values = new Dictionary<string, string>
            {
                {"password", password }
            };
            JObject resultJson = await _post("join/organization/" + organizationAccount, values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> lookHomework(string organizationAccount, string homeworkId)
        {
            JObject resultJson = await _get("search/organization/" + organizationAccount +
                                            "/homework/" + homeworkId);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> lookNotice(string organizationAccount, string noticeId)
        {
            JObject resultJson = await _get("search/organization/" + organizationAccount +
											"/Notice/" + noticeId);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }

        public async Task<Result> complishHomework(string homeworkId, bool complishFlag)
        {
            // this dictionary may sholud be changed as <string, bool>
            var values = new Dictionary<string, string>
            {
                {"uncomplish", complishFlag.ToString() }
            };
            JObject resultJson = await _post("api/update/user/homework/" + homeworkId, values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> updateMemberPosition(string organizationAccount, string memberId, string position)
        {
            var values = new Dictionary<string, string>
            {
                {"position", position }
            };
            JObject resultJson = await _post("update/organization/" + organizationAccount +
                                             "/member/" + memberId, values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> upMember(string organizationAccount, string memberId)
        {
            return await updateMemberPosition(organizationAccount, memberId, "manager");
        }
        public async Task<Result> downMember(string organizationAccount, string memberId)
        {
            return await updateMemberPosition(organizationAccount, memberId, "member");
        }
        public async Task<Result> createHomework(string organizationAccount, Dictionary<string, string> homeworkData)
        {
            JObject resultJson = await _post("create/organization/" + organizationAccount +
                                             "/homework", homeworkData);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> createNotice(string organizationAccount, Dictionary<string, string> noticeData)
        {
            JObject resultJson = await _post("create/organization/" + organizationAccount +
                                             "/notice", noticeData);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }

        public async Task<Result> createNotice(string organizationAccount, MultipartFormDataContent multipartContent)
        {
            JObject resultJson = await _post("create/organization/" + organizationAccount +
                                             "/notice", multipartContent);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }

        public async Task<Result> createVote(string organizationAccount, string string_content)
        {
            JObject resultJson = await _post("create/organization/" + organizationAccount +
                                             "/vote", string_content);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> createVote(string organizationAccount, Dictionary<string, string> voteData)
        {
            JObject resultJson = await _post("create/organization/" + organizationAccount +
                                             "/vote", voteData);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> vote(string organizationAccount, string voteId, string optionId)
        {
            var values = new Dictionary<string, string>
            {
                {"vote_id", voteId },
                {"option_id", optionId }
            };
            JObject resultJson = await _post("vote/organization/" + organizationAccount, values);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> deleteHomework(string organizationAccount, string homeworkId)
        {
            JObject resultJson = await _delete("organization/" + organizationAccount + "/homework/" + homeworkId);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> deleteNotice(string organizationAccount, string noticeId)
        {
            JObject resultJson = await _delete("organization/" + organizationAccount + "/notice/" + noticeId);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> deleteVote(string organizationAccount, string voteId)
        {
            JObject resultJson = await _delete("organization/" + organizationAccount + "/vote/" + voteId);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> deleteMember(string organizationAccount, string memberAccount)
        {
            JObject resultJson = await _delete("organization/" + organizationAccount + "/member/" + memberAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> deleteOrganization(string organizationAccount)
        {
            JObject resultJson = await _delete("organization/" + organizationAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> quitOrganization(string organizationAccount)
        {
            JObject resultJson = await _delete("user/organization/" + organizationAccount);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }

        public async Task<Result> userSetting(MultipartFormDataContent multipartContent)
        {
            JObject resultJson = await _post("settings/user/", multipartContent);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
        public async Task<Result> organizationSetting(MultipartFormDataContent multipartContent, string organizationAccount)
        {
            JObject resultJson = await _post("settings/organization/"+ organizationAccount, multipartContent);
            Result result = new Result();
            if (resultJson == null)
            {
                result.error = true;
                result.message = "network error";
                return result;
            }
            result.error = (bool)resultJson["error"];
            result.message = (string)resultJson["message"];
            return result;
        }
    }
}
