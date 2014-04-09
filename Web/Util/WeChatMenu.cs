using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Web.Util
{
    public class WeChatMenu
    {
        private string appId;
        private string appSecret;
        private BaseBLL bll = new BaseBLL();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        public WeChatMenu()
        {
            IList<WxBase> wx = bll.GetList<WxBase>();
            if (wx != null && wx.Count > 0)
            {
                this.appId = wx[0].AppId;
                this.appSecret = wx[0].AppSecret;
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="labels">大小为18，顺序是1、1-1、1-2...2、2-1...3、3-1、3-2、3-3、3-4、3-5</param>
        /// <returns></returns>
        public string createMenu(ref int errorcode)
        {
            string postUrl = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";
            postUrl = string.Format(postUrl, GetAccessToken());
            string menuInfo = getMenuInfo();
            if (string.IsNullOrEmpty(menuInfo))
            {
                errorcode = -1;
                return "您还没有设置菜单";
            }
            else
            {
                Dictionary<string, object> data = JsonData.JsonToDictionary(postWebRequest(postUrl, menuInfo));
                errorcode = int.Parse(data["errcode"].ToString());
                return data["errmsg"].ToString();
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="labels">大小为18，顺序是1、1-1、1-2...2、2-1...3、3-1、3-2、3-3、3-4、3-5</param>
        /// <returns></returns>
        public string deleteMenu(ref int errorcode)
        {
            string postUrl = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}";
            postUrl = string.Format(postUrl, GetAccessToken());
            string menuInfo = getMenuInfo();
            errorcode = -1;
            if (string.IsNullOrEmpty(menuInfo))
            {
                errorcode = -1;
                return "您还没有设置菜单";
            }
            else
            {
                Dictionary<string, object> data = JsonData.JsonToDictionary(postWebRequest(postUrl, menuInfo));
                errorcode = int.Parse(data["errcode"].ToString());
                return data["errmsg"].ToString();
            }
        }

        /// <summary>
        /// 获取自定义菜单的凭证
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            string accessToken = string.Empty;
            string getUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            getUrl = string.Format(getUrl, appId, appSecret);
            Uri uri = new Uri(getUrl);
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.Method = "POST";

            //获取返回信息
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string returnJason = streamReader.ReadToEnd();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(returnJason);
            object value;
            if (json.TryGetValue("access_token", out value))
            {
                accessToken = value.ToString();
            }
            return accessToken;
        }

        /// <summary>
        /// 获取自定义菜单Json字符串
        /// </summary>
        /// <returns>自定义菜单Json字符串</returns>
        private string getMenuInfo()
        {
            List<WxMenu> list = bll.GetList<WxMenu>().ToList<WxMenu>();
            StringBuilder menuStr = new StringBuilder();
            if (list != null && list.Count > 0)
            {
                menuStr.Append("{\"button\": [");
                var root = list.Where(s => s.PID == -1).OrderBy(s => s.OrderNum).ThenByDescending(s => s.UpdateTime).ToList<WxMenu>();
                for (int i = 0; i < root.Count(); i++)
                {
                    var main = list[i];
                    List<WxMenu> subs = list.Where(s => s.PID == main.ID).OrderBy(s => s.OrderNum).ThenByDescending(s => s.UpdateTime).ToList<WxMenu>();
                    if (subs.Count > 0)
                    {
                        menuStr.Append(getBtnStr(main, true));
                        for (int j = 0; j < subs.Count; j++)
                        {
                            menuStr.Append(getBtnStr(subs[j], false));
                            if (j < subs.Count - 1)
                            {
                                menuStr.Append(",");
                            }
                        }
                        menuStr.Append("]}");
                    }
                    else
                    {
                        menuStr.Append(getBtnStr(main, false));
                    }
                    if (i < root.Count() - 1)
                    {
                        menuStr.Append(",");
                    }
                }
                menuStr.Append("]}");
            }
            return menuStr.ToString();
        }

        private string getBtnStr(WxMenu btn, bool hasChild)
        {
            string btnstr = "";
            if (hasChild)
            {
                btnstr = "{\"name\":\"" + btn.BtnName + "\",\"sub_button\":[";
            }
            else
            {
                if (btn.BtnType == "view")
                {
                    btnstr = "{\"type\":\"view\",\"name\":\"" + btn.BtnName + "\",\"url\":\"" + btn.ReplyUrl + "\"}";
                }
                else
                {
                    btnstr = "{\"type\":\"click\",\"name\":\"" + btn.BtnName + "\",\"key\":\"" + btn.BtnKey + "\"}";
                }
            }
            return btnstr;
        }

        /// <summary>
        /// 提交请求创建菜单
        /// </summary>
        /// <param name="postUrl">提交的地址</param>
        /// <param name="menuInfo">自定义菜单Json字符串</param>
        /// <returns></returns>
        private string postWebRequest(string postUrl, string menuInfo)
        {
            string returnValue = string.Empty;
            try
            {
                byte[] byteData = Encoding.UTF8.GetBytes(menuInfo);
                Uri uri = new Uri(postUrl);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteData.Length;
                //定义Stream信息
                Stream stream = webReq.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Close();
                //获取返回信息
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                returnValue = streamReader.ReadToEnd();
                //关闭信息
                streamReader.Close();
                response.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                returnValue = ex.ToString();
            }
            return returnValue;
        }
    }
}