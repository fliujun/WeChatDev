using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO
{
    /// <summary>
    /// 功能：此类用于Windows应用，NHibernate提供的Session有两个缺陷：
    ///       一方面是线程不安全的，另一方面每次数据库操作创建一个Session对程序性能有影响。
    ///       因此通过线程变量获取一个NHibernate Session的多个线程安全的实例，而且线程变量使用后即释放掉。
    /// </summary>
    public class WinFormNHSession : ISessionManage
    {
        [ThreadStatic]
        private static ISession _threadSession = null;

        public WinFormNHSession()
        {
        }

        /// <summary>
        /// 获取存储到线程变量中的实现NHibernate.ISession接口的类实例
        /// </summary>
        /// <returns>实现NHibernate.ISession接口的线程安全的类实例，当用户之前没有调用Set方法会返回Null</returns>
        public ISession Get()
        {
            if (_threadSession != null)
            {
                if (_threadSession.IsConnected)
                {
                    _threadSession.Reconnect();
                }
            }
            return _threadSession;
        }

        /// <summary>
        /// 存储实现NHibernate.ISession接口的类实例到线程变量中
        /// </summary>
        /// <param name="session">实现NHibernate.ISession接口的类实例</param>
        public void Set(ISession session)
        {
            if (_threadSession.IsConnected)
            {
                session.Disconnect();
            }
            _threadSession = session;
        }
    }
}
