using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Web;

namespace DAO
{
        /// <summary>
        /// 功能：此类用于Web应用，NHibernate提供的Session有两个缺陷：
        ///       一方面是线程不安全的，另一方面每次数据库操作创建一个Session对程序性能有影响。
        ///       因此通过将Session绑定到HttpContext上，这样每个用户具有唯一的一个Session，而且
        ///       在用户的请求结束后关闭Session并自己释放掉。
        /// </summary>
    public class WebNHSession : ISessionManage
        {
            public WebNHSession()
            {

            }

            /// <summary>
            /// 获取存储到HttpContext中的实现NHibernate.ISession接口的类实例
            /// </summary>
            /// <returns>实现NHibernate.ISession接口的类实例，当用户之前没有调用Set方法会返回Null</returns>
            public ISession Get()
            {
                return (ISession)HttpContext.Current.Items[SessionConfigManage.SessionSourceItemName];
            }

            /// <summary>
            /// 存储实现NHibernate.ISession接口的类实例到HttpContext中
            /// </summary>
            /// <param name="session">实现NHibernate.ISession接口的类实例</param>
            public void Set(ISession session)
            {
                if (session != null)
                {
                    HttpContext.Current.Items.Add(SessionConfigManage.SessionSourceItemName, session);
                }
                else
                {
                    HttpContext.Current.Items.Remove(SessionConfigManage.SessionSourceItemName);
                }
        }
    }
}
