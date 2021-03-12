using BSFramework.Application.Code;
using BSFramework.Application.Entity.FlowManage;
using BSFramework.Application.IService.FlowManage;
using BSFramework.Application.Service.FlowManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Data;

namespace BSFramework.Application.Busines.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例操作
    /// </summary>
    public class WFRuntimeBLL
    {
        private WFProcessInstanceIService wfProcessInstanceService = new WFProcessInstanceService();
        private WFProcessSchemeIService wfProcessSchemeService = new WFProcessSchemeService();
        private WFRuntimeIService wfRuntimeService = new WFRuntimeService();
        #region 获取数据
        /// <summary>
        /// 获取流程实例分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询条件</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return wfProcessInstanceService.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 获取流程实例分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="type">3草稿</param>
        /// <returns></returns>
        public DataTable GetPageList(string userid, Pagination pagination, string queryJson, string type)
        {
            return wfProcessInstanceService.GetPageList(userid, pagination, queryJson, type);
        }
        /// <summary>
        /// 获取登录者需要处理的流程
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetToMeBeforePageList(string userid, Pagination pagination, string queryJson)
        {
            return wfProcessInstanceService.GetToMeBeforePageList(userid, pagination, queryJson);
        }
        /// <summary>
        /// 获取登录者已经处理的流程
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetToMeAfterPageList(string userid, Pagination pagination, string queryJson)
        {
            return wfProcessInstanceService.GetToMeAfterPageList(userid, pagination, queryJson);
        }
        /// <summary>
        /// 获取实例进程模板的实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public WFProcessSchemeEntity GetProcessSchemeEntity(string keyValue)
        {
            return wfProcessSchemeService.GetEntity(keyValue);
        }
        /// <summary>
        /// 已办流程进度查看，根据当前访问人的权限查看表单内容
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public WFProcessSchemeEntity GetProcessSchemeEntityByUserId(string keyValue)
        {
            try
            {
                WFProcessSchemeEntity entity = wfProcessSchemeService.GetEntity(keyValue);
                entity.SchemeContent = wfRuntimeService.GetProcessSchemeContentByUserId(entity.SchemeContent, OperatorProvider.Provider.Current().UserId);
                return entity;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 已办流程进度查看，根据当前节点的权限查看表单内容
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="isPermission"></param>
        /// <returns></returns>
        public WFProcessSchemeEntity GetProcessSchemeEntityByNodeId(string keyValue, string nodeId)
        {
            try
            {
                WFProcessSchemeEntity entity = wfProcessSchemeService.GetEntity(keyValue);
                entity.SchemeContent = wfRuntimeService.GetProcessSchemeContentByNodeId(entity.SchemeContent, nodeId);
                return entity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取实例进程信息的实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public WFProcessInstanceEntity GetProcessInstanceEntity(string keyValue)
        {
            return wfProcessInstanceService.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="WFSchemeInfoId">流程模板表Id</param>
        /// <param name="Data">表单数据</param>
        /// <param name="Type">1正常,3草稿</param>
        /// <returns></returns>
        public int CreateProcess(string userid, string username, string wfSchemeInfoId, WFProcessInstanceEntity wfProcessInstanceEntity, string frmData)
        {
            try
            {
                if (wfProcessInstanceEntity.EnabledMark == 1)//发起流程
                {
                    wfRuntimeService.CreateInstance(userid, username, Guid.NewGuid(), wfSchemeInfoId, wfProcessInstanceEntity, frmData);
                }
                else if (wfProcessInstanceEntity.EnabledMark == 3)//草稿
                {
                    wfRuntimeService.CreateRoughdraft(Guid.NewGuid(), wfSchemeInfoId, wfProcessInstanceEntity, frmData);
                }
                return 1;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 编辑草稿实例
        /// </summary>
        /// <param name="keyVlaue">实例信息Id(主键)</param>
        /// <param name="frmData">表单数据</param>
        /// <returns></returns>
        public int EditionRoughdraftProcess(string userid, string username, string keyVlaue, WFProcessInstanceEntity wfProcessInstanceEntity, string frmData)
        {
            try
            {
                wfProcessInstanceEntity.Id = keyVlaue;
                if (wfProcessInstanceEntity.EnabledMark == 1)
                {
                    wfRuntimeService.CreateInstance(userid, username, wfProcessInstanceEntity, frmData);
                }
                else if (wfProcessInstanceEntity.EnabledMark == 3)//继续保存为草稿
                {
                    wfRuntimeService.EditionRoughdraft(wfProcessInstanceEntity, frmData);
                }
                return 1;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 审核流程
        /// </summary>
        /// <param name="ProcessId">审核的流程Id</param>
        /// <param name="verificationData">审核内容</param>
        /// <returns></returns>
        public void VerificationProcess(string userid, string username, string processId, string verificationData)
        {
            try
            {
                dynamic verificationDataJson = verificationData.ToJson();

                //驳回
                if (verificationDataJson.VerificationFinally.Value == "3")
                {
                    string _nodeId = "";
                    if (verificationDataJson.NodeRejectStep != null)
                    {
                        _nodeId = verificationDataJson.NodeRejectStep.Value;
                    }
                    wfRuntimeService.NodeReject(userid, username, processId, _nodeId, verificationDataJson.VerificationOpinion.Value);
                }
                else if (verificationDataJson.VerificationFinally.Value == "2")//表示不同意
                {
                    wfRuntimeService.NodeVerification(userid, username, processId, false, verificationDataJson.VerificationOpinion.Value);
                }
                else if (verificationDataJson.VerificationFinally.Value == "1")//表示同意
                {
                    wfRuntimeService.NodeVerification(userid, username, processId, true, verificationDataJson.VerificationOpinion.Value);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 流程指派
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="makeLists"></param>
        public void DesignateProcess(string processId, string makeLists)
        {
            wfProcessInstanceService.DesignateProcess(processId, makeLists);
        }
        /// <summary>
        /// 删除工作流实例进程
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public int DeleteProcess(string keyValue)
        {
            return wfProcessInstanceService.DeleteProcess(keyValue);
        }
        /// <summary>
        /// 虚拟操作实例
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="state">0暂停,1启用,2取消（召回）</param>
        /// <returns></returns>
        public int OperateVirtualProcess(string username, string keyValue, int state)
        {
            return wfProcessInstanceService.OperateVirtualProcess(username, keyValue, state);
        }
        #endregion
    }
}
