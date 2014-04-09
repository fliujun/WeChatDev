using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.WxHandler
{
    public class LocationService
    {
        public ResponseMessageNews GetResponseMessage(RequestMessageLocation requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            var markersList = new List<Markers>();
            markersList.Add(new Markers()
            {
                X = requestMessage.Location_X,
                Y = requestMessage.Location_Y,
                Color = "red",
                Label = "S",
                Size = MarkerSize.Default,
            });
            var mapSize = "480x600";
            var mapUrl = GoogleMapHelper.GetGoogleStaticMap(19 /*requestMessage.Scale*//*微信和GoogleMap的Scale不一致，这里建议使用固定值*/,
                                                            markersList, mapSize);
            responseMessage.Articles.Add(new Article()
            {
                Description = string.Format("您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                              requestMessage.Location_X, requestMessage.Location_Y,
                              requestMessage.Scale, requestMessage.Label),
                PicUrl = mapUrl,
                Title = "定位地点周边地图",
                Url = mapUrl
            });
            responseMessage.Articles.Add(new Article()
            {
                Title = "开发者官网",
                Description = "From Liujun",
                PicUrl = "https://mail.qq.com/cgi-bin/viewfile?f=687B756EADCF463FE682F979C2B6000909749CF39A4711F264A28B5A1AF42AA3A89436733680D5B4B5C7E67AFB48DC4D6B4222A3414F6D1EE58B48B1F62C186B12BE9FEDED6C71E2&sid=KlxWmjnm3npQVEES",
                Url = "http://www.fliujun.com"
            });

            return responseMessage;
        }
    }
}