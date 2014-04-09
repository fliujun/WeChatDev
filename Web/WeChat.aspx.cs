using Senparc.Weixin.MP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web.WxHandler;

namespace EcaMobile.WeChat
{
    public partial class WeChat : System.Web.UI.Page
    {
        private readonly string Token = "ecamobile";

        protected void Page_Load(object sender, EventArgs e)
        {
            string signature = Request["signature"];
            string timestamp = Request["timestamp"];
            string nonce = Request["nonce"];
            string echostr = Request["echostr"];

            if (Request.HttpMethod == "GET")
            {
                if (CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    Response.Write(echostr);
                }
                else
                {
                    Response.Write("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                                "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
                Response.End();
            }
            else
            {
                if (!CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    Response.Write("参数错误！");
                    return;
                }

                var maxRecordCount = 10;
                var messageHandler = new CustomMessageHandler(Request.InputStream, maxRecordCount);
                messageHandler.Execute();
                Response.Write(messageHandler.ResponseDocument.ToString());
                Response.End();
            }
        }
    }
}