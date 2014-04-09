using DAO;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Util
{
    public class BaseBLL
    {
        public ISession _session;

        public BaseBLL()
        {
            this._session = SessionFactory.GetSession();
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="obj">需要添加的对象</param>
        /// <returns>新添记录的ID</returns>
        public int Add(object obj)
        {
            int ID = -1;
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    ID = Convert.ToInt32(_session.Save(obj));
                    tx.Commit();
                }
                catch (HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }
            return ID;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj">需要修改的对象</param>
        public void Update(object obj)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(obj);
                    tx.Commit();
                }
                catch (HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj">需要删除的对象</param>
        public void Delete(object obj)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(obj);
                    tx.Commit();
                }
                catch (HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 根据ID获得记录
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ID">记录ID</param>
        /// <returns></returns>
        public T GetModel<T>(int ID)
        {
            return _session.Get<T>(ID);
        }

        /// <summary>
        /// 获得记录总数
        /// </summary>
        public IList<T> GetList<T>()
        {
            return _session.CreateCriteria(typeof(T)).List<T>();
        }

        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="conditionField">条件字段</param>
        /// <param name="conditionStr">条件</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="direction">是否为升序</param>
        /// <returns></returns>
        public IList<T> GetListByCondition<T>(string conditionField, string conditionStr, string sortFiled, bool direction)
        {
            return _session.CreateCriteria(typeof(T)).Add(Restrictions.Like(conditionField, conditionStr)).AddOrder(new NHibernate.Criterion.Order(sortFiled, direction)).List<T>();
        }

        public IList<T> GetListByCondition<T>(string conditionField, int conditionStr, string sortFiled, bool direction)
        {
            return _session.CreateCriteria(typeof(T)).Add(Restrictions.Eq(conditionField, conditionStr)).AddOrder(new NHibernate.Criterion.Order(sortFiled, direction)).List<T>();
        }

        /// <summary>
        /// 分页查询并排序返回列表和总页数、总记录数
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="direction">是否升序</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<T> GetListByPage<T>(int currentPageIndex, int pageSize, string sortFiled, bool direction, out int pageCount, out int recordCount)
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria.SetProjection(Projections.RowCount());//设置查询行数
            recordCount = (int)criteria.UniqueResult(); //获取返回行数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize > 0)
            {
                pageCount++;
            }
            criteria.SetProjection(null);//取消查询行数。返回查询数据集

            //设置排序
            criteria.AddOrder(new Order(sortFiled, direction));

            //设置分页          
            criteria.SetFirstResult((currentPageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize);
            return criteria.List<T>();
        }

        /// <summary>
        /// 按条件分页查询并排序返回列表和总页数、总记录数
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="direction">是否升序</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<T> GetListByPageAndCondition<T>(string conditionField, string condition, int currentPageIndex, int pageSize, string sortFiled, bool direction, out int pageCount, out int recordCount)
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria = criteria.Add(Restrictions.Like(conditionField, condition));
            criteria.SetProjection(Projections.RowCount());//设置查询行数
            recordCount = (int)criteria.UniqueResult(); //获取返回行数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize > 0)
            {
                pageCount++;
            }
            criteria.SetProjection(null);//取消查询行数。返回查询数据集

            //设置排序
            criteria.AddOrder(new Order(sortFiled, direction));

            //设置分页          
            criteria.SetFirstResult((currentPageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize);
            return criteria.List<T>();
        }

        public IList<T> GetListByPageAndCondition<T>(string conditionField, int condition, int currentPageIndex, int pageSize, string sortFiled, bool direction, out int pageCount, out int recordCount)
        {
            ICriteria criteria = _session.CreateCriteria(typeof(T));
            criteria = criteria.Add(Restrictions.Eq(conditionField, condition));
            criteria.SetProjection(Projections.RowCount());//设置查询行数
            recordCount = (int)criteria.UniqueResult(); //获取返回行数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize > 0)
            {
                pageCount++;
            }
            criteria.SetProjection(null);//取消查询行数。返回查询数据集

            //设置排序
            criteria.AddOrder(new Order(sortFiled, direction));

            //设置分页          
            criteria.SetFirstResult((currentPageIndex - 1) * pageSize)
                    .SetMaxResults(pageSize);
            return criteria.List<T>();
        }
    }
}