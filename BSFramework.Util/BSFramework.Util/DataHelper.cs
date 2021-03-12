using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BSFramework.Util
{
    /// <summary>
    /// 数据源转换
    /// </summary>
    public class DataHelper
    {
        #region IList如何转成List<T>
        /// <summary>
        /// IList如何转成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> IListToList<T>(IList list)
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return new List<T>(array);
        }
        #endregion

        #region DataTable根据条件过滤表的内容
        /// <summary>
        /// 根据条件过滤表的内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable DataFilter(DataTable dt, string condition)
        {
            if (DataHelper.IsExistRows(dt))
            {
                if (condition.Trim() == "")
                {
                    return dt;
                }
                else
                {
                    DataTable newdt = new DataTable();
                    newdt = dt.Clone();
                    DataRow[] dr = dt.Select(condition);
                    for (int i = 0; i < dr.Length; i++)
                    {
                        newdt.ImportRow((DataRow)dr[i]);
                    }
                    return newdt;
                }
            }
            else
            {
                return null;
            }
        }
        public static DataTable DataFilter(DataTable dt, string condition, string sort)
        {
            if (DataHelper.IsExistRows(dt))
            {
                DataTable newdt = new DataTable();
                newdt = dt.Clone();
                DataRow[] dr = dt.Select(condition, sort);
                for (int i = 0; i < dr.Length; i++)
                {
                    newdt.ImportRow((DataRow)dr[i]);
                }
                return newdt;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 检查DataTable 是否有数据行
        /// <summary>
        /// 检查DataTable 是否有数据行
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool IsExistRows(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return true;

            return false;
        }
        #endregion

        #region DataTable 转 DataTableToHashtable
        /// <summary>
        /// DataTable 转 DataTableToHashtable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Hashtable DataTableToHashtable(DataTable dt)
        {
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName;
                    ht[key] = dr[key];
                }
            }
            return ht;
        }
        #endregion

        #region List转换DataTable
        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
        #endregion

        #region DataTable/DataSet 转 XML
        /// <summary>
        /// DataTable 转 XML
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToXML(DataTable dt)
        {
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    dt.WriteXml(writer);
                    return writer.ToString();
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// DataSet 转 XML
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToXML(DataSet ds)
        {
            if (ds != null)
            {
                System.IO.StringWriter writer = new System.IO.StringWriter();
                ds.WriteXml(writer);
                return writer.ToString();
            }
            return String.Empty;
        }
        #endregion

        #region DataRow  转  HashTable
        /// <summary>
        /// DataRow  转  HashTable
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static Hashtable DataRowToHashTable(DataRow dr)
        {
            Hashtable htReturn = new Hashtable(dr.ItemArray.Length);
            foreach (DataColumn dc in dr.Table.Columns)
                htReturn.Add(dc.ColumnName, dr[dc.ColumnName]);
            return htReturn;
        }
        #endregion

        #region 返回排序委托
        /// <summary>
        /// 返回排序委托
        /// </summary>
        /// <param name="columName">需要的排序的列名</param>
        /// <returns></returns>
        public static Func<T, string> FuncSort<T>(string columName)
        {
            //排序列
            Func<T, string> convert = delegate (T model)
            {
                var val = model.GetType().GetProperty(columName).GetValue(model, null);
                var colVal = val?.ToString() ?? "";
                return colVal;
            };
            return convert;
        }
        #endregion

        #region 单个实体转换过程
        /// <summary>
        /// 单个实体转换过程
        /// </summary>
        /// <typeparam name="T">代表要待转换的对象</typeparam>
        /// <typeparam name="M">代表转换后的对象</typeparam>
        /// <param name="ObjT">参数中传入被转换的对象</param>
        /// <returns></returns>
        public static M InputToOutput<T, M>(T ObjT)
            where T : new()
            where M : new()
        {
            string tempName = string.Empty;  //临时保存属性名称
            //获取T的所有属性和值
            M Result = new M();  //定义返回值
            PropertyInfo[] propertys = Result.GetType().GetProperties();
            //循环获取T的属性及其值
            foreach (PropertyInfo pr in propertys)
            {
                tempName = pr.Name;//将属性名称赋值给临时变量
                PropertyInfo findPro = ObjT.GetType().GetProperty(tempName);//从带转换实体获取属性
                if (findPro != null)  //如果获取到了:尝试执行赋值过程
                {
                    if (!pr.CanWrite) continue;  //如果属性不可写，跳出
                    //取值:参数不匹配-------------
                    object value = findPro.GetValue(ObjT, null);
                    //如果为非空，则赋值给对象的属性
                    if (value != DBNull.Value)
                        pr.SetValue(Result, value, null);
                }
            }
            return Result;
        }
        #endregion

        #region 两个集合转换过程
        public static List<M> ListInputToOutput<T, M>(List<T> objTlist)
            where T : new()
            where M : new()
        {
            //定义返回的集合
            List<M> ListM = new List<M>();
            //定义临时M类变量
            M TempM = new M();
            foreach (T item in objTlist)
            {
                TempM = InputToOutput<T, M>(item);
                ListM.Add(TempM);//加入结果集合
            }
            return ListM;
        }
        #endregion

        #region IQueryable数据分页


        ///// <summary>
        ///// 分页  
        ///// </summary>
        ///// <typeparam name="T">要分页的数据的实体类型</typeparam>
        ///// <param name="PageSize">每页个数</param>
        ///// <param name="CurPage">当前页码</param>
        ///// <param name="Objs">查询结果</param>
        ///// <param name="TotalCount">数据总条数</param>
        ///// <returns></returns>
        //public static List<TSource> DataPaging<TSource>(int PageSize, int CurPage, IQueryable<TSource> Objs,  out int TotalCount) 
        //{
        //    TotalCount = Objs.Count();
        //    return Objs.Skip(PageSize * (CurPage - 1)).Take(PageSize * CurPage).ToList();
        //}

        /// <summary>
        /// 排序 分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="PageSize">每页个数</param>
        /// <param name="CurPage">当前页码</param>
        /// <param name="Objs">查询结果</param>
        /// <param name="orderColumn">排序字段 </param>
        /// <param name="isAsc">是否是正序</param>
        /// <param name="TotalCount">返回总记录条数</param>
        /// <returns></returns>
        public static List<TSource> DataPaging<TSource, TKey>(int PageSize, int CurPage, IQueryable<TSource> Objs, Expression<Func<TSource, TKey>> orderColumn, out int TotalCount, bool isAsc = true) where TSource : class, new()
        {
            TotalCount = Objs.Count();
            if (isAsc)
            {
                return Objs.OrderBy(orderColumn).Skip(PageSize * (CurPage - 1)).Take(PageSize * CurPage).ToList();
            }
            else
            {
                return Objs.OrderByDescending(orderColumn).Skip(PageSize * (CurPage - 1)).Take(PageSize * CurPage).ToList();
            }
        }

        /// <summary>
        /// 排序 分页  适用于多字段排序，排序后再传入方法分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="PageSize">每页个数</param>
        /// <param name="CurPage">当前页码</param>
        /// <param name="Objs">排序后的查询数据</param>
        /// <param name="TotalCount">返回总记录条数</param>
        /// <returns></returns>
        public static List<TSource> DataPaging<TSource>(int PageSize, int CurPage, IOrderedQueryable<TSource> Objs, out int TotalCount) where TSource : class, new()
        {
            TotalCount = Objs.Count();
            return Objs.Skip(PageSize * (CurPage - 1)).Take(PageSize * CurPage).ToList();

        }

        #endregion

        #region linq 查询结果转换成DataTable 方法
        public static DataTable ConvertToTable(IQueryable query)
        {
            DataTable dtList = new DataTable();
            bool isAdd = false;
            PropertyInfo[] objProterties = null;
            foreach (var item in query)
            {
                if (!isAdd)
                {
                    objProterties = item.GetType().GetProperties();
                    foreach (var itemProterty in objProterties)
                    {
                        Type type = null;
                        if (itemProterty.PropertyType != typeof(string) && itemProterty.PropertyType != typeof(int) && itemProterty.PropertyType != typeof(DateTime))
                        {
                            type = typeof(string);
                        }
                        else
                        {
                            type = itemProterty.PropertyType;
                        }
                        dtList.Columns.Add(itemProterty.Name, type);
                    }
                    isAdd = true;
                }
                var row = dtList.NewRow();
                foreach (var pi in objProterties)
                {
                    row[pi.Name] = pi.GetValue(item, null);
                }
                dtList.Rows.Add(row);
            }
            return dtList;
        }
        #endregion
    }
}
