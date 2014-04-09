using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Web.Util;

namespace Web
{
    /// <summary>
    /// MyHandler 的摘要说明
    /// </summary>
    public class MyHandler : IHttpHandler
    {
        #region 初始化
        private HttpContext context;
        private BaseBLL bll = new BaseBLL();

        private string getString(string key)
        {
            return this.context.Request[key];
        }

        private int getInt(string key)
        {
            int value = -1;
            int.TryParse(this.context.Request[key], out value);
            return value;
        }

        private DateTime getDateTime(string key)
        {
            DateTime date = Convert.ToDateTime("1900-01-01");
            string value = this.context.Request[key];
            if (!string.IsNullOrEmpty(value))
            {
                DateTime.TryParse(value, out date);
            }
            return date;
        }

        private bool getBool(string key)
        {
            bool result = false;
            bool.TryParse(this.context.Request[key].Trim(), out result);
            return result;
        }

        #endregion 初始化

        #region 方法入口
        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            string method = this.getString("method");
            JsonData Result = new JsonData();
            try
            {
                switch (method)
                {
                    case "loadTreeData"://获取导航菜单
                        this.loadTreeData(ref Result);
                        break;
                    case "addMenu"://添加菜单
                        this.addMenu(ref Result);
                        break;
                    case "updateMenuName"://重命名菜单
                        this.updateMenuName(ref Result);
                        break;
                    case "deleteMenu"://删除菜单节点
                        this.deleteMenu(ref Result);
                        break;
                    case "updateMenuOrder"://修改菜单顺序
                        this.updateMenuOrder(ref Result);
                        break;
                    case "syncWxMenu"://同步微信菜单
                        this.syncWxMenu(ref Result);
                        break;
                    case "modifyWxText"://添加微信文本
                        this.modifyWxText(ref Result);
                        break;
                    case "getWxTextList"://获取微信文本列表
                        this.getWxTextList(ref Result);
                        break;
                    case "getWxText"://获取微信文本
                        this.getWxText(ref Result);
                        break;
                    case "deleteWxText"://删除微信文本
                        this.deleteWxText(ref Result);
                        break;
                }
            }
            catch (Exception e)
            {
                Result.SetError(e.Message);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(Result.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion 方法入口

        #region 获取导航菜单
        private void loadTreeData(ref  JsonData Result)
        {
            IList<WxMenu> menus = bll.GetList<WxMenu>().OrderBy(s => s.OrderNum).ThenByDescending(s => s.UpdateTime).ToList<WxMenu>();
            StringBuilder treeStr = new StringBuilder();
            treeStr.Append("[{\"id\":\"r0\",\"parent\":\"#\",\"text\":\"微信自定义导航菜单\",\"data\":{\"id\":-1,\"pid\":-1,\"replyType\":\"-1\",\"replyID\":-1},\"state\":{\"opened\":true},\"type\":\"#\"}");
            for (int i = 0; i < menus.Count; i++)
            {
                if (i == 0)
                {
                    treeStr.Append(",");
                }
                var menu = menus[i];
                if (menu.PID == -1)
                {
                    treeStr.Append("{\"id\":\"m" + menu.ID + "\",\"parent\":\"r0\",\"text\":\"" + menu.BtnName + "\",\"data\":{\"id\":" + menu.ID + ",\"pid\":" + menu.PID + ",\"replyType\":\"" + menu.ReplyType + "\",\"replyID\":" + menu.ReplyID + "},\"state\":{\"opened\":true},\"type\":\"main\"}");
                }
                else
                {
                    treeStr.Append("{\"id\":\"s" + menu.ID + "\",\"parent\":\"m" + menu.PID + "\",\"text\":\"" + menu.BtnName + "\",\"data\":{\"id\":" + menu.ID + ",\"pid\":" + menu.PID + ",\"replyType\":\"" + menu.ReplyType + "\",\"replyID\":" + menu.ReplyID + "},\"state\":{\"opened\":true},\"type\":\"submenu\"}");
                }
                if (i < menus.Count - 1)
                {
                    treeStr.Append(",");
                }
            }
            treeStr.Append("]");
            Result.Set("data", treeStr.ToString());
        }
        #endregion

        #region 添加菜单
        private void addMenu(ref JsonData Result)
        {
            var pid = this.getInt("pid");
            var text = this.getString("text");
            var type = this.getString("type");
            var ordernum = this.getInt("ordernum");
            int id = bll.Add(new WxMenu() { PID = pid, BtnName = text, BtnType = "click", OrderNum = ordernum, ReplyType = -1, ReplyID = -1, UpdateTime = DateTime.Now });
            WxMenu obj = bll.GetModel<WxMenu>(id);
            if (type == "main")
            {
                obj.BtnKey = "m" + id;
            }
            else
            {
                obj.BtnKey = "s" + id;
            }
            bll.Update(obj);
        }
        #endregion

        #region 重命名菜单
        private void updateMenuName(ref JsonData Result)
        {
            var id = this.getInt("id");
            var text = this.getString("text");
            WxMenu m = bll.GetModel<WxMenu>(id);
            m.BtnName = text;
            bll.Update(m);
        }
        #endregion

        #region 删除菜单节点
        private void deleteMenu(ref JsonData Result)
        {
            var id = this.getInt("id");
            bll.Delete(bll.GetModel<WxMenu>(id));
            IList<WxMenu> menus = bll.GetListByCondition<WxMenu>("PID", id, "ID", true);
            if (menus != null && menus.Count > 0)
            {
                foreach (var m in menus)
                {
                    bll.Delete(m);
                }
            }
        }
        #endregion

        #region 修改菜单顺序
        private void updateMenuOrder(ref JsonData Result)
        {
            var id = this.getInt("id");
            var pid = this.getInt("pid");
            var ordernum = this.getInt("ordernum");

            WxMenu m = bll.GetModel<WxMenu>(id);
            m.PID = pid;
            m.OrderNum = ordernum;
            m.UpdateTime = DateTime.Now;
            bll.Update(m);
        }
        #endregion

        #region 同步微信菜单
        private void syncWxMenu(ref JsonData Result)
        {
            WeChatMenu wcm = new WeChatMenu();
            int errorcode = -1;
            string errorMsg = wcm.createMenu(ref errorcode);
            if (errorcode != 0)
            {
                Result.SetError(errorMsg);
            }
        }
        #endregion

        #region 添加微信文本
        private void modifyWxText(ref JsonData Result)
        {
            int id = this.getInt("id");
            string text = this.getString("text");
            string type = this.getString("type");
            if (type == "add")
            {
                bll.Add(new WxText() { Content = text, UpdateTime = DateTime.Now });
            }
            else
            {
                var wx = bll.GetModel<WxText>(id);
                wx.Content = text;
                wx.UpdateTime = DateTime.Now;
                bll.Update(wx);
            }
            var data = bll.GetList<WxText>().OrderByDescending(s => s.UpdateTime);
            Result.Set("data", data);
        }
        #endregion

        #region 获取微信文本列表
        private void getWxTextList(ref JsonData Result)
        {
            var data = bll.GetList<WxText>().OrderByDescending(s => s.UpdateTime);
            Result.Set("data", data);
        }
        #endregion

        #region 获取微信文本
        private void getWxText(ref JsonData Result)
        {
            int id = this.getInt("id");
            Result.Set("data", bll.GetModel<WxText>(id));
        }
        #endregion

        #region 删除微信文本
        private void deleteWxText(ref JsonData Result)
        {
            int id = this.getInt("id");
            bll.Delete(bll.GetModel<WxText>(id));
            var data = bll.GetList<WxText>().OrderByDescending(s => s.UpdateTime);
            Result.Set("data", data);
        }
        #endregion
    }
}