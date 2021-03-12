
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// �� �����Ű�
    /// </summary>
    public interface IWorkSettingService
    {
        /// <summary>
        /// ��ʼ����Ӱ��
        /// </summary>
        /// <param name="entityList"></param>
        void saveForm(List<WorkSettingEntity> entityList);

        /// <summary>
        /// ��ȡ����б�
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkSettingEntity> GetList(string depId);

        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <returns></returns>
        WorkSettingEntity getEntitybySetUp(string keyvalue,string deptid);
        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <returns></returns>
        WorkSettingEntity getEntity(string keyvalue);
        /// <summary>
        /// ɾ��һ�����İ��
        /// </summary>
        /// <param name="keyValue">����</param>
        void RemoveForm(string keyValue);
    }
}
