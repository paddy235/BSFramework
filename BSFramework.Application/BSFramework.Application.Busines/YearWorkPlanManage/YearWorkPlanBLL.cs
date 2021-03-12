using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.YearWorkPlan;
using BSFramework.Application.IService.YearWorkPlanManage;
using BSFramework.Application.Service.YearWorkPlanManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.YearWorkPlanManage
{
    public class YearWorkPlanBLL
    {
        private IYearWorkPlanService service = new YearWorkPlanService();

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<YearWorkPlanEntity> GetPlanList()
        {
            var data = service.GetPlanList();
            return data;
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            var dt = service.GetPageList(pagination, queryJson);
            return dt;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public YearWorkPlanEntity GetEntity(string keyValue)
        {

            return service.GetEntity(keyValue);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
        public void RemoveForm(string keyValue)
        {
            service.RemoveForm(keyValue);
        }

        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveForm(UserEntity user, YearWorkPlanEntity entity)
        {
            var nowTime = DateTime.Now;
            if (string.IsNullOrEmpty(entity.id))
            {
                //新增
                string keyValue = string.Empty;
                if (entity.progress.Contains("100"))
                {
                    entity.planfinish = nowTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    entity.planfinish = string.Empty;
                }
                entity.lastprogress = "0";
                entity.CREATEUSERID = user.UserId;
                entity.CREATEUSERNAME = user.RealName;
                entity.CREATEDATE = nowTime;
                entity.deptid = user.DepartmentId;
                entity.deptcode = user.DepartmentCode;
                entity.deptname = user.DepartmentName;

                entity.MODIFYDATE = nowTime;
                entity.MODIFYUSERID = user.UserId;
                entity.MODIFYUSERNAME = user.RealName;
                service.SaveForm(keyValue, entity);
            }
            else
            {
                List<YearWorkPlanEntity> list = new List<YearWorkPlanEntity>();
                string keyValue = entity.id;
                var old = service.GetEntity(keyValue);
                entity.id = string.Empty;
                if (old.progress.Contains("100"))
                {
                    if (!entity.progress.Contains("100"))
                    {
                        entity.planfinish = string.Empty;
                    }
                }
                if (entity.progress.Contains("100"))
                {
                    entity.planfinish = nowTime.ToString("yyyy-MM-dd");
                }
                entity.lastprogress = old.progress;
                old.MODIFYDATE = nowTime;
                old.MODIFYUSERID = user.UserId;
                entity.bookmark = old.bookmark;
                old.MODIFYUSERNAME = user.RealName;
                entity.MODIFYDATE = nowTime;
                entity.MODIFYUSERID = user.UserId;
                entity.MODIFYUSERNAME = user.RealName;
                entity.CREATEUSERID = user.UserId;
                entity.CREATEUSERNAME = user.RealName;
                entity.CREATEDATE = nowTime;
                entity.deptid = user.DepartmentId;
                entity.deptcode = user.DepartmentCode;
                entity.deptname = user.DepartmentName;
                var editstr = string.Empty;
                if (entity.plan != old.plan)
                {
                    editstr += "工作任务：" + old.plan + "    修改为：" + entity.plan;
                }
                if (entity.planstart != old.planstart)
                {
                    if (string.IsNullOrEmpty(editstr))
                    {
                        editstr += "计划开始时间：" + old.planstart + "    修改为：" + entity.planstart;

                    }
                    else
                    {
                        editstr += "&计划开始时间：" + old.planstart + "    修改为：" + entity.planstart;

                    }
                }
                if (entity.planend != old.planend)
                {
                    if (string.IsNullOrEmpty(editstr))
                    {
                        editstr += "计划结束时间：" + old.planend + "    修改为：" + entity.planend;

                    }
                    else
                    {
                        editstr += "&计划结束时间：" + old.planend + "    修改为：" + entity.planend;

                    }
                }
                old.editstr = editstr;
                list.Add(entity);
                list.Add(old);
                service.SaveFormList(list);
            }

        }
    }


}
