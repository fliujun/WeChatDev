using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO
{
    /// <summary>
    /// 功能：加载嵌入资源(Xml配置文件),打开一个SessionFactory,获取NHibernate的Session实例
    /// </summary>
    public class NHibernateSession
    {
        private static Configuration cfg = null;
        private static ISessionFactory sessionFactory = null;

        static NHibernateSession()
        {
            cfg = new Configuration().Configure();
            sessionFactory = cfg.BuildSessionFactory();
        }

        /// <summary>
        /// 获取NHibernate的Session实例
        /// </summary>
        /// <returns></returns>
        public static ISession GetNHibernateSession()
        {
            return sessionFactory.OpenSession();
        }
    }
}
