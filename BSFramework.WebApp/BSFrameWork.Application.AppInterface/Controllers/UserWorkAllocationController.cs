using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class UserWorkAllocationController : ApiController
    {
        private UserWorkAllocationBLL bll = new UserWorkAllocationBLL();
        //peoplecontroller GetQuarters  获取岗位
        /// <summary>
        ///获取转岗详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectGetDetail()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string data = dy.data;
            try
            {
                var entity = bll.GetDetail(data);
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new UserWorkAllocationEntity() };

            }
        }
        /// <summary>
        ///获取转岗详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectGetDetailByUser()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            try
            {
                var entity = bll.GetDetailByUser(userId);
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new UserWorkAllocationEntity() };

            }
        }
        /// <summary>
        /// 获取部门班组  获取全部后截取
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetbzDept()
        {

            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string id = dy.data.id;
            string category = dy.data.category;
            try
            {
                var list = default(List<string>);
                if (string.IsNullOrEmpty(category))
                {
                    category = "部门,班组";
                }
                list = category.Split(',').ToList();
                var departmentBLL = new DepartmentBLL();

                UserEntity user = new UserBLL().GetEntity(userId);
                var parDpet = departmentBLL.GetAuthorizationDepartmentApp(user.DepartmentId);

                var entity = bll.GetSubDepartments(parDpet.DepartmentId, category);
                var treeList = new List<TreeEntity>();
                foreach (var item in entity)
                {
                    TreeEntity tree = new TreeEntity();
                    bool hasChildren = entity.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                    tree.id = item.DepartmentId;
                    tree.text = item.FullName;
                    tree.value = item.DepartmentId;
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item.Nature == list[0] ? "0" : item.ParentId;
                    treeList.Add(tree);
                }


                return new { info = "查询成功：", code = result, data = treeList };
            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1 };
            }

        }
        /// <summary>
        /// 获取部门班组 获获取中一层一层截取
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDept()
        {

            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string id = dy.data.id;
            string category = dy.data.category;
            try
            {
                var list = default(List<string>);
                if (string.IsNullOrEmpty(category))
                {
                    category = "省级,厂级,部门,班组";
                }
                var departmentBLL = new DepartmentBLL();

                UserEntity user = new UserBLL().GetEntity(userId);
                var parDpet = departmentBLL.GetAuthorizationDepartmentApp(user.DepartmentId);
                var entity = bll.getDepartmentList(parDpet.DepartmentId, category);
                var treeList = new List<TreeEntity>();
                foreach (var item in entity)
                {
                    TreeEntity tree = new TreeEntity();
                    bool hasChildren = entity.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                    tree.id = item.DepartmentId;
                    tree.text = item.FullName;
                    tree.value = item.DepartmentId;
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item.ParentId;
                    treeList.Add(tree);
                }
                return new { info = "查询成功：", code = result, data = treeList };
            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1 };
            }

        }

        /// <summary>
        ///新增转岗信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object OperationEntity()
        {
            var result = 0;
            var message = string.Empty;
            string res = HttpContext.Current.Request.Form["json"];
            var json = JObject.Parse(res.ToString());
            string userId = json.Value<string>("userId");
            var entity = JsonConvert.DeserializeObject<UserWorkAllocationEntity>(json["data"].ToString());
            try
            {
                var id = bll.OperationEntity(entity, userId);
                return new { info = "操作成功", code = result, data = id };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };

            }

        }

        /// <summary>
        ///提交完成转岗信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object update()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string id = dy.data.id;
            string quarters = dy.data.quarters;
            string RoleDutyName = dy.data.RoleDutyName;
            string quartersid = dy.data.quartersid;
            string RoleDutyId = dy.data.RoleDutyId;
            try
            {
                var entity = bll.GetDetail(id);
                if (string.IsNullOrEmpty(entity.leavetime))
                {

                
                var data = getSKQuarters(entity.userId, entity.departmentid);
                var list = data.Select(x => new
                {
                    Id = x.RoleId,
                    Text = x.FullName,
                    Value = x.EnCode
                }).ToList();
                var codeList = list.Where(x => quartersid.Contains(x.Id)).Select(x => x.Value).ToList();
                entity.Code = string.Join(",", codeList);
                entity.iscomplete = true;
                entity.quarters = quarters;
                entity.RoleDutyName = RoleDutyName;
                entity.quartersid = quartersid;
                entity.RoleDutyId = RoleDutyId;
                }
                var info = bll.OperationBll(entity, userId);
                if (info != "操作成功")
                {
                    return new { info = info, code = 1 };
                }
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };

            }

        }
        public List<RoleEntity> getSKQuarters(string userid, string deptid)
        {
            var dict = new
            {
                //data = "4ad1d7d0-50af-4966-8c48-48570075f009",
                //userid = "System",
                //tokenid = ""
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetJobByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            List<RoleEntity> data = JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            data = data.OrderBy(x => x.EnCode).ToList();
            return data;

            //DataItemBLL dataItemBLL = new DataItemBLL();
            //DataItemDetailBLL dataItemDetailBLL = new DataItemDetailBLL();
            //PostBLL pbll = new PostBLL();
            //JobBLL jbll = new JobBLL();


            //var user = OperatorProvider.Provider.Current();
            //var data = pbll.GetQuartersList(user.OrganizeId).OrderBy(x => x.EnCode).ToList();
            //return data;

        }
        public object updatetoerchtms()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            json = json.Replace("Id", "id");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userid;
            string id = dy.data.id;

            string quarters = dy.data.quarters;
            string RoleDutyName = dy.data.RoleDutyName;
            string quartersid = dy.data.quartersid;
            string RoleDutyId = dy.data.RoleDutyid;
            try
            {
                var entity = new UserWorkAllocationEntity();

                if (json.Contains("leavetime"))
                {
                    entity.userId = dy.data.userid;
                    entity.leavetime = dy.data.leavetime;
                    entity.leaveremark = dy.data.leaveremark;
                }
                else
                {
                    entity = bll.GetDetail(id);
                    var data = getSKQuarters(entity.userId, entity.departmentid);
                    var list = data.Select(x => new
                    {
                        Id = x.RoleId,
                        Text = x.FullName,
                        Value = x.EnCode
                    }).ToList();
                    var codeList = list.Where(x => quartersid.Contains(x.Id)).Select(x => x.Value).ToList();
                    entity.Code = string.Join(",", codeList);
                    entity.iscomplete = true;
                    entity.quarters = quarters;
                    entity.RoleDutyName = RoleDutyName;
                    entity.quartersid = quartersid;
                    entity.RoleDutyId = RoleDutyId;
                }

                var info = bll.OperationBlltoerchtms(entity);
                if (info != "操作成功")
                {
                    return new { info = info, code = 1 };
                }
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };

            }
        }
    }
}
