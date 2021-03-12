using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Common;
using BSFramework.Data;
using System.Text;
using System.Configuration;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Service.WorkMeeting
{
    /// <summary>
    /// �� �����Ű�
    /// </summary>
    public class WorkSettingService : RepositoryFactory<WorkSettingEntity>, IWorkSettingService
    {
        /// <summary>
        /// ��ʼ����Ӱ��
        /// </summary>
        /// <param name="entityList"></param>
        public void saveForm(List<WorkSettingEntity> entityList)
        {
            if (entityList.Count > 0)
            {
                foreach (var item in entityList)
                {
                    if (!String.IsNullOrEmpty(item.WorkSettingId))
                    {
                        item.Modify(item.WorkSettingId);
                        this.BaseRepository().Update(item);
                    }
                    else
                    {
                        item.Create();
                        this.BaseRepository().Insert(item);
                    }
                }
            }

        }
        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <returns></returns>
        public WorkSettingEntity getEntity(string keyvalue) {

        return this.BaseRepository().FindEntity(keyvalue);
        }

        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <returns></returns>
        public WorkSettingEntity getEntitybySetUp(string keyvalue,string deptid) {
            return this.BaseRepository().IQueryable().FirstOrDefault(row => row.WorkSetupId == keyvalue&&row.DeparMentId== deptid);

        }
        /// <summary>
        /// ��ȡ����б�
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkSettingEntity> GetList(string depId)
        {

            if (string.IsNullOrEmpty(depId))
            {
                return this.BaseRepository().IQueryable().ToList();
            }
            return this.BaseRepository().IQueryable().Where(row=>row.DeparMentId==depId).ToList();

        }
        /// <summary>
        /// ɾ��һ�����İ��
        /// </summary>
        /// <param name="keyValue">����</param>
        public void RemoveForm(string keyValue)
        {
            var dataList = this.BaseRepository().IQueryable(t => t.BookMarks == keyValue).ToList();
            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    this.BaseRepository().Delete(item.WorkSettingId);
                }
            }



        }

    }
}
