﻿using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.OndutyManage
{
    public interface IOndutyService
    {
        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        List<OndutyMeetEntity> GetPagesList(Pagination pagination, string queryJson, string userid);
        List<OndutyEntity> GetList(DateTime start, DateTime end, string userid,string deptid);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
         OndutyEntity GetEntity(string keyValue);
  
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
         void RemoveForm(string keyValue);


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        void SaveForm(string keyValue, OndutyEntity entity);
    

    }
}
