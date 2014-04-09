using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO
{
    /// <summary>
    /// 功能：管理多个实现ISessionManage接口的类工厂，根据读取的要加载的类名称信息，进行动态的创建Session
    /// </summary>
    public class SessionFactory
    {
        private static ISession session = null;
        private static ISessionManage sessionManage = null;

        static SessionFactory()
        {
            Init();
        }

        /// <summary>
        /// 获取实现NHibernate.ISession接口的Session实例
        /// </summary>
        /// <returns>返回实现NHibernate.ISession接口的类实例</returns>
        public static ISession GetSession()
        {
            session = sessionManage.Get();

            if (session == null)
            {
                session = NHibernateSession.GetNHibernateSession();
                sessionManage.Set(session);
            }

            return session;
        }

        private static void Init()
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.Load(SessionConfigManage.AssemblyName);
            sessionManage = (ISessionManage)ass.CreateInstance(SessionConfigManage.SessionSourceItemName);
        }
    }
}
