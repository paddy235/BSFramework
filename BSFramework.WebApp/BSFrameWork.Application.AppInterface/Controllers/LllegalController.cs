using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class LllegalController : ApiController
    {

        LllegalBLL lbll = new LllegalBLL();
        LllegalAcceptBLL labll = new LllegalAcceptBLL();
        LllegalRefromBLL lrbll = new LllegalRefromBLL();
        DepartmentBLL dtbll = new DepartmentBLL();
        PeopleBLL pbll = new PeopleBLL();
        UserBLL ubll = new UserBLL();
        //private DataItemCache dataItemCache = new DataItemCache();
        // GET: /Lllegal/
        /// <summary>
        /// 查看违章详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalDetail([FromBody]JObject json)
        {
            try
            {
                LllegalRefromEntity refrom = new LllegalRefromEntity();
                LllegalAcceptEntity accept = new LllegalAcceptEntity();
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string LllegalId = dy.data.LllegalId;
                LllegalEntity entity = lbll.GetLllegalDetail(LllegalId);
                IList fileList = new FileInfoBLL().GetFilesByRecId(entity.ID, BSFramework.Util.Config.GetValue("AppUrl"));
                entity.fileList = fileList;
                //获取违章整改信息 
                //if (!string.IsNullOrEmpty(entity.RefromId))
                //{
                //    refrom = new LllegalRefromBLL().GetEntity(entity.RefromId);
                //    for (int i = 0; i < refrom.fileList.Count; i++)
                //    {
                //        refrom.fileList[i].FilePath = BSFramework.Util.Config.GetValue("AppUrl") + refrom.fileList[i].FilePath.TrimStart('~');
                //    }
                //}
                //else refrom = null;
                refrom = new LllegalRefromBLL().GetEntityByLllegalId(LllegalId);
                if (refrom != null)
                {
                    fileList = new FileInfoBLL().GetFilesByRecId(refrom.Id, BSFramework.Util.Config.GetValue("AppUrl"));
                    refrom.fileList = fileList;
                }
                accept = new LllegalAcceptBLL().GetEntityByLllegalId(LllegalId);
                if (accept != null)
                {

                    fileList = new FileInfoBLL().GetFilesByRecId(accept.Id, BSFramework.Util.Config.GetValue("AppUrl"));
                    accept.fileList = fileList;
                }
                return new { info = "成功", code = 0, data = new { lllegalEntity = entity, LllegalRefrom = refrom, LllegalAccept = accept } };
                //PeopleEntity p_entity = pbll.GetEntity(uid);

            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new LllegalEntity() };
            }

        }

        /// <summary>
        /// 查看违章详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalDetailNew([FromBody]JObject json)
        {
            try
            {
                LllegalRefromEntity refrom = new LllegalRefromEntity();
                LllegalAcceptEntity accept = new LllegalAcceptEntity();
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string LllegalId = dy.data;
                LllegalEntity entity = lbll.GetLllegalDetail(LllegalId);
                IList fileList = new FileInfoBLL().GetFilesByRecId(entity.ID, BSFramework.Util.Config.GetValue("AppUrl"));
                entity.fileList = fileList;
                //获取违章整改信息 
                //if (!string.IsNullOrEmpty(entity.RefromId))
                //{
                //    refrom = new LllegalRefromBLL().GetEntity(entity.RefromId);
                //    for (int i = 0; i < refrom.fileList.Count; i++)
                //    {
                //        refrom.fileList[i].FilePath = BSFramework.Util.Config.GetValue("AppUrl") + refrom.fileList[i].FilePath.TrimStart('~');
                //    }
                //}
                //else refrom = null;
                refrom = new LllegalRefromBLL().GetEntityByLllegalId(LllegalId);
                if (refrom != null)
                {
                    fileList = new FileInfoBLL().GetFilesByRecId(refrom.Id, BSFramework.Util.Config.GetValue("AppUrl"));
                    refrom.fileList = fileList;
                }
                accept = new LllegalAcceptBLL().GetEntityByLllegalId(LllegalId);
                if (accept != null)
                {

                    fileList = new FileInfoBLL().GetFilesByRecId(accept.Id, BSFramework.Util.Config.GetValue("AppUrl"));
                    accept.fileList = fileList;
                }
                return new { info = "成功", code = 0, data = new { lllegalEntity = entity, LllegalRefrom = refrom, LllegalAccept = accept } };
                //PeopleEntity p_entity = pbll.GetEntity(uid);

            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new LllegalEntity() };
            }

        }

        /// <summary>
        /// 查看违章详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalDetailbyUser([FromBody]JObject json)
        {
            try
            {
                //本人违章
                List<object> dataOne = new List<object>();
                //连带违章
                List<object> datatwo = new List<object>();
                //扣分 连带扣分 罚款 连带罚款
                var score = 0; var joinscore = 0;
                var fine = 0; var joinfine = 0;
                //剩余积分
                var remainingScore = 0;
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                // string LllegalId = dy.data;
                string userId = dy.userId;
                bool allowPaging = dy.allowPaging;
                int pageIndex = 0;
                int pageSize = 0;
                if (allowPaging)
                {
                     pageIndex =Convert.ToInt32(dy.pageIndex);
                     pageSize = Convert.ToInt32(dy.pageSize);
                }
                //本年
                var end = DateTime.Now;
                var start = new DateTime(end.Year, 1, 1);
                List<LllegalEntity> entity = lbll.GetLllegalDetailByUser(userId, start, end,allowPaging,pageIndex,pageSize);
                foreach (var item in entity)
                {
                    var LllegalScore = 0;
                    var LllegalFine = 0;
                    var Lllegal = new
                    {
                        Id = item.ID,
                        LllegalTime = item.LllegalTime,
                        LllegalAddress = item.LllegalAddress,
                        LllegalDescribe = item.LllegalDescribe,
                        LllegalScore = LllegalScore,
                        LllegalFine = LllegalFine
                    };
                    dataOne.Add(Lllegal);
                }

                return new { info = "成功", code = 0, data =new { basedata= dataOne, datatotal = dataOne.Count, joindata = datatwo, joindatatotal = datatwo.Count,  remainingScore = remainingScore,year = end.Year, score = score, joinscore = joinscore, fine = fine, joinfine = joinfine }  };
                //PeopleEntity p_entity = pbll.GetEntity(uid);

            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new LllegalEntity() };
            }

        }

        /// <summary>
        /// 新增违章登记
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddLllegalRegister()
        {

            try
            {
                //string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string lllegalEntity = JsonConvert.SerializeObject(dy.data.lllegalEntity);
                LllegalEntity entity = JsonConvert.DeserializeObject<LllegalEntity>(lllegalEntity);
                entity.ID = Guid.NewGuid().ToString();
                entity.FlowState = "待核准";
                //entity.Sub = dy.data.Sub; //"Company"/"Team"
                int total = 0;
                string num = "";
                string date = DateTime.Now.ToString("yyyyMMdd");
                var nlist = lbll.GetListNew("", 1, 19999, out total);
                nlist = nlist.Where(x => x.LllegalNumber.Contains(date)).OrderByDescending(x => x.LllegalNumber);
                if (nlist.Count() > 0)
                {
                    num = (Convert.ToInt64(nlist.First().LllegalNumber) + 1).ToString();
                }
                else
                {
                    num = date + "001";
                }
                entity.LllegalNumber = num;
                // entity.LllegalTeamId = "";
                lbll.AddLllegalRegister(entity);
                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/Lllegal/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Lllegal"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Lllegal");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Lllegal\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }
        /// <summary>
        /// 获取违章列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //根据登陆人员Id找到所属班组Id
                string userId = dy.userId;
                long pageSize = 0, pageIndex = 0;
                string from = dy.data.startTime;
                string to = dy.data.endTime;
                pageSize = dy.data.pageSize;
                pageIndex = dy.data.pageIndex;
                int total = 0;

                UserEntity user = ubll.GetEntity(userId);
                PeopleEntity p = pbll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                List<LllegalEntity> lList = new List<LllegalEntity>();
                if (dept.Nature != "班组")
                {
                    //if (dept.Description == "安全部")
                    //{
                    lList = lbll.GetLllegalList(from, to, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), "", user.UserId, out total).ToList();
                    //}

                }
                else
                {
                    //if (p.Quarters == "班长" || p.Quarters == "安全员")
                    //{
                    lList = lbll.GetLllegalList(from, to, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), user.DepartmentId, user.UserId, out total).ToList();
                    //}
                }
                for (int i = 0; i < lList.Count; i++)
                {
                    IList fileList = new FileInfoBLL().GetFilesByRecId(lList[i].ID, BSFramework.Util.Config.GetValue("AppUrl"));
                    lList[i].fileList = fileList;
                }
                lList.Select(t => new
                {
                    t.ID,
                    t.fileList,
                    t.LllegalAddress,
                    t.LllegalDepart,
                    t.LllegalDepartCode,
                    t.LllegalDescribe,
                    t.LllegalLevel,
                    t.LllegalNumber,
                    t.LllegalPerson,
                    t.LllegalPersonId,
                    t.LllegalTeam,
                    t.LllegalTeamId,
                    t.LllegalType,
                    t.RegisterPerson,
                    t.RegisterPersonId,
                    t.Remark,
                    LllegalTime = t.LllegalTime.ToString("yyyy-MM-dd")
                }).ToList();
                return new { info = "成功", code = 0, count = total, data = lList };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }
        public class Lentity
        {
            public DateTime date { get; set; }
            public int dcl { get; set; }
            public int all { get; set; }
        }
        [HttpPost]
        public object GetLllegalListByYear([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //根据登陆人员Id找到所属班组Id
                string userId = dy.userId;

                string from = dy.data.from;
                string to = dy.data.to;
                string type = dy.data.type;
                string level = dy.data.level;

                UserEntity user = ubll.GetEntity(userId);
                PeopleEntity p = pbll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                List<LllegalEntity> lList = new List<LllegalEntity>();
                int total = 0;
                if (string.IsNullOrEmpty(from))
                {
                    from = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrEmpty(to))
                {
                    to = new DateTime(DateTime.Now.Year, 12, 31).ToString("yyyy-MM-dd");
                }
                lList = lbll.GetLllegalList(from, to, 1, 10000, user.DepartmentId, user.UserId, out total).ToList();
                if (!string.IsNullOrEmpty(type))
                {
                    lList = lList.Where(x => x.LllegalType == type).ToList();
                }
                if (!string.IsNullOrEmpty(level))
                {
                    lList = lList.Where(x => x.LllegalLevel == level).ToList();
                }
                return new { info = "成功", code = 0, data = lList };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1 };
            }
        }
        /// <summary>
        /// 获取月份每天的数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalListByMonth([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //根据登陆人员Id找到所属班组Id
                string userId = dy.userId;
                string from = ""; string to = "";
                string month = dy.data.month;
                string year = dy.data.year;
                UserEntity user = ubll.GetEntity(userId);
                PeopleEntity p = pbll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                List<Lentity> list = new List<Lentity>();
                List<LllegalEntity> lList = new List<LllegalEntity>();
                List<Int32> count = new List<int>();


                int total = 0;
                int days = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
                for (int i = 0; i < days; i++)
                {
                    Lentity l = new Lentity();
                    from = new DateTime(int.Parse(year), int.Parse(month), i + 1).ToString("yyyy-MM-dd");
                    to = new DateTime(int.Parse(year), int.Parse(month), i + 1).ToString("yyyy-MM-dd");
                    lList = lbll.GetLllegalList(from, to, 1, 10000, user.DepartmentId, user.UserId, out total).ToList();
                    l.date = new DateTime(int.Parse(year), int.Parse(month), i + 1);
                    l.all = lList.Count(x => x.ApproveResult == "0");
                    l.dcl = lList.Count(x => x.FlowState == "待核准" || x.FlowState == "待整改" || x.FlowState == "待验收");
                    if (l.dcl > 0)
                    {
                        list.Add(l);
                    }
                }
                return new { info = "成功", code = 0, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1 };
            }
        }

        [HttpPost]
        public object GetLllegalListByDay([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //根据登陆人员Id找到所属班组Id
                string userId = dy.userId;
                string month = dy.data.month;
                string year = dy.data.year;
                string day = dy.data.day;
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;
                UserEntity user = ubll.GetEntity(userId);
                PeopleEntity p = pbll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                List<Lentity> list = new List<Lentity>();
                List<LllegalEntity> lList = new List<LllegalEntity>();
                List<Int32> count = new List<int>();

                int total = 0;

                string from = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day)).ToString("yyyy-MM-dd");
                string to = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day)).ToString("yyyy-MM-dd");
                lList = lbll.GetLllegalList(from, to, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), user.DepartmentId, user.UserId, out total).ToList();
                return new { info = "成功", code = 0, data = lList };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1 };
            }
        }

        /// <summary>
        /// 违章人员列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalPersonList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //long pageIndex = dy.data.pageIndex;//当前索引页
                //long pageSize = dy.data.pageSize;
                //bool allowPage = dy.data.allowPage;
                string userId = dy.userId;
                string name = dy.data.Name;
                string type = dy.data.Type;  //登记、整改
                UserEntity user = ubll.GetEntity(userId);
                List<UserEntity> u_list = new List<UserEntity>();
                PeopleEntity p = pbll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                if (type == "add")   //登记选择核准人，可以选择所有人员
                {
                    u_list = ubll.GetList().ToList();
                }
                else if (type == "edit")  //核准选择整改人，要根据部门判断
                {
                    if (dept.Nature != "班组") //非班组成员可选择所有人
                    {

                        u_list = ubll.GetList().ToList();
                    }
                    else
                    {
                        u_list = ubll.GetDeptUsers(user.DepartmentId).ToList(); //只能选择本班组成员
                    }
                }
                u_list = u_list.Where(x => x.RealName.Contains(name) && x.DepartmentId != "0").ToList();
                List<PeopleEntity> p_list = new List<PeopleEntity>();

                foreach (UserEntity u in u_list)   //返回PeopleEntity实体，非班组成员，bzid、bzname 为空
                {
                    PeopleEntity np = new PeopleEntity();
                    np.ID = u.UserId;
                    np.Name = u.RealName;

                    DepartmentEntity dp = new DepartmentBLL().GetEntity(u.DepartmentId);
                    if (dp == null) continue;

                    DepartmentEntity pdp = new DepartmentBLL().GetEntity(dp.ParentId);//父级部门
                    if (dp.Nature == "班组")
                    {
                        np.BZID = dp.DepartmentId;
                        np.BZName = dp.FullName;
                        np.DeptId = pdp.DepartmentId;
                        np.DeptName = pdp.FullName;
                    }
                    else //非班组成员
                    {
                        np.BZID = "";
                        np.BZName = "";
                        np.DeptId = dp.DepartmentId;
                        np.DeptName = dp.FullName;
                    }

                    p_list.Add(np);
                }
                int count = p_list.Count();
                //if (u_list.Count > 0)
                //{
                //    for (int i = 0; i < u_list.Count; i++)
                //    {
                //        PeopleEntity newPeople = pbll.GetEntity(u_list[i].UserId);
                //        if (newPeople != null)
                //            p_list.Add(newPeople);
                //    }
                //}
                //if (allowPage)
                //{
                //    p_list = p_list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                //}
                return new { info = "成功", code = 0, count = count, data = p_list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }

        }
        /// <summary> 
        /// 获取违章类型/违章级别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalTypeOrLevel([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string Type = dy.data.Type;
                string EnCode = string.Empty;
                if (Type == "level")
                {
                    EnCode = "LllegalLevel";
                }
                else
                {
                    EnCode = "LllegalType";
                }
                var entity = new DataItemBLL().GetEntityByCode(EnCode);
                var list = new DataItemDetailBLL().GetList(entity.ItemId);
                //var data = dataItemCache.GetDataItemList(EnCode);
                return new { info = "成功", code = 0, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        /// <summary> 
        /// 获取违章类型/违章级别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalTypeOrLevelNew([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string type = dy.data;
                string EnCode = string.Empty;
                if (type == "level")
                {
                    EnCode = "LllegalLevel";
                }
                else if (type == "type")
                {
                    EnCode = "LllegalType";
                }
                var entity = new DataItemBLL().GetEntityByCode(EnCode);
                var list = new DataItemDetailBLL().GetList(entity.ItemId);
                //var data = dataItemCache.GetDataItemList(EnCode);
                return new { info = "成功", code = 0, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        /// <summary>
        /// 违章核准
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateLllegalApprove()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                LllegalEntity entity = lbll.GetEntity(dy.data.LllegalId);
                string result = dy.data.ApproveResult;

                //核准信息
                if (result == "0")
                {
                    entity.FlowState = "待整改";
                }
                else
                {
                    entity.FlowState = "核准不通过";
                }
                entity.ApproveResult = dy.data.ApproveResult;
                entity.ReformPeople = dy.data.ReformPeople;
                entity.ReformPeopleId = dy.data.ReformPeopleId;

                // 2018-08-24  违章所属部门，修改为整改人所在部门
                UserEntity ruser = new UserBLL().GetEntity(entity.ReformPeopleId);
                entity.LllegalTeamId = ruser.DepartmentId;

                entity.ApproveReason = dy.data.ApproveReason;
                if (!string.IsNullOrEmpty(dy.data.ApproveDate))
                {
                    entity.ApproveDate = Convert.ToDateTime(dy.data.ApproveDate);
                }
                else
                {
                    entity.ApproveDate = null;
                }
                if (!string.IsNullOrEmpty(dy.data.ReformDate))
                {
                    entity.ReformDate = Convert.ToDateTime(dy.data.ReformDate);
                }
                else
                {
                    entity.ReformDate = null;
                }
                entity.ApprovePersonId = user.UserId;
                entity.ApprovePerson = user.RealName;

                entity.IsAssess = dy.data.IsAssess;
                if (entity.IsAssess == "y")
                {
                    entity.AssessMoney = dy.data.AssessMoney;  //考核金额
                }
                //流程状态



                lbll.SaveForm(entity.ID, entity);
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }


        /// <summary>
        /// 违章延期
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Defer()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                int days = Convert.ToInt32(dy.data.Days);
                LllegalEntity l = lbll.GetEntity(dy.data.LllegalId);
                //LllegalRefromEntity entity = lrbll.GetEntityByLllegalId(dy.data.LllegalId);

                //流程状态
                l.FlowState = "待核准";
                l.ReformDate = l.ReformDate.Value.AddDays(days);
                lbll.SaveForm(l.ID, l);
                return new { info = "成功", code = 0, count = 0 };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        /// <summary>
        /// 违章验收
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddLllegalAccept()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string AcceptEntity = JsonConvert.SerializeObject(dy.data.RefromEntity);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                //验收信息
                LllegalAcceptEntity entity = new LllegalAcceptEntity();


                entity.AcceptMind = dy.data.AcceptMind;
                entity.AcceptPeople = user.RealName;
                entity.AcceptPeopleId = user.UserId;
                entity.AcceptResult = dy.data.AcceptResult;
                entity.AcceptTime = Convert.ToDateTime(dy.data.AcceptTime);
                entity.LllegalId = dy.data.LllegalId;

                labll.SaveFrom(entity);



                //流程状态
                LllegalEntity l = lbll.GetEntity(entity.LllegalId);
                if (entity.AcceptResult == "0")
                {
                    l.FlowState = "验收通过";
                }
                else
                {
                    l.FlowState = "待整改";
                }
                lbll.SaveForm(entity.LllegalId, l);


                FileInfoBLL fileBll = new FileInfoBLL();
                LllegalAcceptEntity lr = labll.GetEntityByLllegalId(entity.LllegalId);
                if (lr != null)
                {
                    var flist = fileBll.GetFilesByRecIdNew(lr.Id);
                    foreach (FileInfoEntity f in flist)
                    {
                        fileBll.Delete(f.FileId);
                    }
                }

                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = lr.Id,
                        RecId = lr.Id,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/LllegalAccept/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalAccept"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalAccept");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalAccept\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }



        /// <summary>
        /// 获取违章核准、整改、验收 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetListByFlowState()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                int pageIndex = Convert.ToInt32(dy.data.pageIndex);//当前索引页
                int pageSize = Convert.ToInt32(dy.data.pageSize);
                string from = dy.data.startTime;
                string to = dy.data.endTime;
                int total = 0;
                string userid = dy.userId;
                string type = dy.data.Type;// 1-待核准，顺序类推
                UserEntity user = ubll.GetEntity(userid);
                PeopleEntity p = pbll.GetEntity(userid);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                string sub = "";
                if (dept.Nature != "班组")
                {
                    if (dept.Description == "安全部") //安全部成员，可以看到所有提交至HSE数据
                    {
                        sub = "Company";
                    }
                    else
                    {
                        sub = "0"; //其他公司级用户
                    }
                }
                else
                {
                    if (p.Quarters == "班长" || p.Quarters == "安全员")
                    {
                        sub = "Team";
                    }
                }
                IList<LllegalEntity> llist = lbll.GetListNoPage(type, userid, user.DepartmentId, "", "", sub, 1, 10000, out total).ToList();
                if (!string.IsNullOrEmpty(from))
                {
                    DateTime f = DateTime.Parse(from);
                    llist = llist.Where(x => x.LllegalTime >= f).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    DateTime t = DateTime.Parse(to);

                    t = t.AddDays(1);
                    llist = llist.Where(x => x.LllegalTime < t).ToList();
                }
                total = llist.Count;
                llist = llist.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                for (int i = 0; i < llist.Count; i++)
                {
                    IList fileList = new FileInfoBLL().GetFilesByRecId(llist[i].ID, BSFramework.Util.Config.GetValue("AppUrl"));
                    llist[i].fileList = fileList;
                }
                var list = llist;
                return new { info = "成功", code = 0, count = total, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        /// <summary>
        /// 首页统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetCounts()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                int total = 0;
                string userid = dy.userId;
                //string type = dy.data.Type;// 0-我上传的，1-待核准，顺序类推
                UserEntity user = ubll.GetEntity(userid);
                PeopleEntity p = pbll.GetEntity(userid);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                string breakRuleAdmin = isbreakruleadmin(user);
                string sub = "";
                if (dept.Nature != "班组")
                {
                    if (dept.Description == "安全部") //安全部成员，可以看到所有提交至HSE数据
                    {
                        sub = "Company";
                    }
                    else
                    {
                        sub = "0"; //其他公司级用户
                    }
                }
                else
                {
                    if (p.Quarters == "班长" || p.Quarters == "安全员")
                    {
                        sub = "Team";
                    }
                }
                ////int count1 = lbll.GetListNoPage("", userid, user.DepartmentId, "", "").Count();
                ////int count2 = lbll.GetListNoPage("0", userid, user.DepartmentId, "", "").Count();
                int count3 = lbll.GetListNoPage("1", userid, user.DepartmentId, "", "", sub, 1, 1000, out total).Count();
                int count4 = lbll.GetListNoPage("2", userid, user.DepartmentId, "", "", sub, 1, 1000, out total).Count();
                int count5 = lbll.GetListNoPage("3", userid, user.DepartmentId, "", "", sub, 1, 1000, out total).Count();
                //IList<LllegalEntity> llist = lbll.GetListNoPage(type, userid, user.DepartmentId, "", "","",1,10,out total).ToList();
                return new { info = "成功", code = 0, data = new { count1 = count3.ToString(), count2 = count4.ToString(), count3 = count5.ToString(), breakRuleAdmin = breakRuleAdmin } };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        private string isbreakruleadmin(UserEntity entity)
        {
            string r = "0";
            DepartmentEntity dept = new DepartmentBLL().GetEntity(entity.DepartmentId);
            //dept.Description == "安全部"  --只有安全部的人才可以看到
            if (dept.Nature != "班组")//非班组成员均可以看到所有菜单
            {
                r = "1";
            }
            else
            {
                PeopleEntity p = new PeopleBLL().GetEntity(entity.UserId);
                if (p.Quarters == "班长" || p.Quarters == "安全员")
                {
                    r = "1";
                }
            }
            return r;

        }
    }
}
