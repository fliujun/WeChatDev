using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DAO
{
    /// <summary>
    /// 功能：根据类库的应用环境不同(Windows应用还是Web应用)，动态创建类实例
    /// </summary>
    public class SessionConfigManage
    {
        private const string SESSION_ITEM_NAME = "SessionItemName";
        private static object _locker = new object();
        private static string _sessionItemName = string.Empty;
        private static string _assemblyName = string.Empty;

        static SessionConfigManage()
        {
            string configString = ConfigurationManager.AppSettings[SESSION_ITEM_NAME];
            string[] arr = configString.Split(',');
            _sessionItemName = arr[0];
            _assemblyName = arr[1];
        }
        /// <summary>
        /// 获取配置文件中名为SESSION_ITEM_NAME配置节的信息，记录的要加载的SessionManage的类全称
        /// </summary>
        /// <returns>实现ISessionManage接口的类的名称</returns>
        public static string SessionSourceItemName
        {
            get
            {
                lock (_locker)
                {
                    return _sessionItemName;
                }
            }
        }

        /// <summary>
        /// 获取配置文件中名为SESSION_ITEM_NAME配置节的信息，记录的要加载的SessionManage的类全称
        /// </summary>
        /// <returns>实现ISessionManage接口的类的程序集名称</returns>
        public static string AssemblyName
        {
            get
            {
                lock (_locker)
                {
                    return _assemblyName;
                }
            }
        }
    }
}
