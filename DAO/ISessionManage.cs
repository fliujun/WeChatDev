using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO
{
    public interface ISessionManage
    {
        /// <summary>
        /// 获取Session的一个实例
        /// </summary>
        /// <returns>返回实现NHibernate.ISession接口的类</returns>
        ISession Get();

        /// <summary>
        /// 设置Session的一个实例
        /// </summary>
        /// <param name="session">实现NHibernate.ISession接口的类</param>
        void Set(ISession session);
    }
}
