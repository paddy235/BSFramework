using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.ToolManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class ToolController : BaseApiController
    {
        //
        // GET: /Default1/
        ToolTypeBLL ttbll = new ToolTypeBLL();
        ToolInfoBLL tibll = new ToolInfoBLL();
        ToolBorrowBLL tbbll = new ToolBorrowBLL();
        ToolCheckBLL tcbll = new ToolCheckBLL();
        ToolRepairBLL trbll = new ToolRepairBLL();
        ToolInventoryBLL tinbll = new ToolInventoryBLL();
        UserBLL ubll = new UserBLL();
        DepartmentBLL dtbll = new DepartmentBLL();
        FileInfoBLL fileBll = new FileInfoBLL();
        /// <summary>
        /// 工器具种类列表（主页） 
        /// 大类列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllTools()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("data");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string name = dy.data.name;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<ToolTypeEntity> list = ttbll.GetPageList(name, user.DepartmentId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();


                return new { code = 0, info = "获取数据成功", count = total, data = new { toolTypeList = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 工器具型号列表
        /// 小类列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolsById()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string name = dy.data.name;
                string tid = dy.data.tId;
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<ToolInfoEntity> list = tibll.GetPageList(name, tid, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();


                return new { code = 0, info = "获取数据成功", count = total, data = new { toolInfoList = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增工器具类别（大类）
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object AddToolType()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string name = dy.data.Name;
                string path = dy.data.Path;
                string inventoryId = dy.data.Id;
                string id = Guid.NewGuid().ToString();
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                ToolTypeEntity tt = new ToolTypeEntity();
                tt.BZId = dept.DepartmentId;
                tt.CreateDate = DateTime.Now;
                tt.Name = name;
                tt.ID = id;
                tt.InventoryId = inventoryId;
                tt.Path = path;


                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                if (files.Count > 0)
                {
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = tt.ID,
                        RecId = tt.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    tt.Path = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                }
                ttbll.SaveForm(string.Empty, tt);
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 修改工器具类别（大类）
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object EditToolType()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string ttentity = JsonConvert.SerializeObject(dy.data);

                ToolTypeEntity entity = JsonConvert.DeserializeObject<ToolTypeEntity>(ttentity);

                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                if (files.Count > 0)
                {
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    entity.Path = BSFramework.Util.Config.GetValue("FilePath") + "/Content/toolpic/" + fileId + ext;
                }
                ttbll.SaveForm(entity.ID, entity);
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 新增工器具型号（小类）
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object AddToolInfo()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.toolInfo);
                string typeid = dy.data.typeId;
                ToolInfoEntity entity = JsonConvert.DeserializeObject<ToolInfoEntity>(tientity);

                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                }
                entity.TypeId = typeid;
                entity.CreateDate = DateTime.Now;
                entity.CurrentNumber = entity.Total;
                entity.BZID = dept.DepartmentId;
                entity.BZName = dept.FullName;

                string hgzpath = "";
                string cerpath = "";
                string checkpath = "";

                string sm = dy.data.files.sm;
                string hg = dy.data.files.hg;
                string jy = dy.data.files.jy;
                if (!string.IsNullOrEmpty(sm))
                {
                    var fileentity = fileBll.GetEntity(sm);
                    cerpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.Certificate = fileentity.FileName;
                }
                if (!string.IsNullOrEmpty(hg))
                {
                    var fileentity = fileBll.GetEntity(hg);
                    hgzpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.HGZ = fileentity.FileName;
                }
                if (!string.IsNullOrEmpty(jy))
                {
                    var fileentity = fileBll.GetEntity(jy);
                    checkpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.CheckReport = fileentity.FileName;
                }
                #region
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                //保存附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    var fname = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);


                    //name = name.Substring(3, name.Length - 3);
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = name,
                        FilePath = "~/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);

                    if (fname.Substring(0, 2) == "hg")
                    {
                        hgzpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.HGZ = name;
                    }
                    else if (fname.Substring(0, 2) == "jy")
                    {
                        checkpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.CheckReport = name;
                    }
                    else if (fname.Substring(0, 2) == "sm")
                    {
                        cerpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.Certificate = name;
                    }
                }
                #endregion
                entity.CheckPath = checkpath;
                entity.CerPath = cerpath;
                entity.HGZPath = hgzpath;


                //20181203   保存工器具编号信息
                var numbers = entity.Numbers.Split(',');
                foreach (string number in numbers)
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        var tnlist = tibll.GetToolNumberList("").Where(x => x.Number == number && x.IsBreak != true);
                        if (tnlist.Count() > 0) return new { code = 2, info = "编号" + number + "已经存在，请重新输入！", data = new { } };
                    }
                }
                foreach (string number in numbers)
                {
                    if (!string.IsNullOrEmpty(number))
                    {

                        var toolnumber = new ToolNumberEntity();
                        toolnumber.ID = Guid.NewGuid().ToString();
                        toolnumber.ToolId = entity.ID;
                        toolnumber.Number = number;
                        toolnumber.IsBreak = false;
                        tibll.SaveToolNumber(toolnumber.ID, toolnumber);
                    }
                }
                tibll.SaveForm(entity.ID, entity);
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 修改工器具型号（小类）
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object EditToolInfo()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.toolInfo);

                ToolInfoEntity entity = JsonConvert.DeserializeObject<ToolInfoEntity>(tientity);

                string hgzpath = entity.HGZPath;
                string cerpath = entity.CerPath;
                string checkpath = entity.CheckPath;
                string sm = dy.data.files.sm;
                string hg = dy.data.files.hg;
                string jy = dy.data.files.jy;
                if (!string.IsNullOrEmpty(sm))
                {
                    var fileentity = fileBll.GetEntity(sm);
                    cerpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.Certificate = fileentity.FileName;
                }
                if (!string.IsNullOrEmpty(hg))
                {
                    var fileentity = fileBll.GetEntity(hg);
                    hgzpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.HGZ = fileentity.FileName;
                }
                if (!string.IsNullOrEmpty(jy))
                {
                    var fileentity = fileBll.GetEntity(jy);
                    checkpath = fileentity.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    entity.CheckReport = fileentity.FileName;
                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                //保存附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    var fname = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);


                    //name = name.Substring(3, name.Length - 3);
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = name,
                        FilePath = "~/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);

                    if (fname.Substring(0, 2) == "hg")
                    {
                        hgzpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.HGZ = name;
                    }
                    else if (fname.Substring(0, 2) == "jy")
                    {
                        checkpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.CheckReport = name;
                    }
                    else if (fname.Substring(0, 2) == "sm")
                    {
                        cerpath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + fileId + ext;
                        entity.Certificate = name;
                    }
                }
                entity.CheckPath = checkpath;
                entity.CerPath = cerpath;
                entity.HGZPath = hgzpath;
                tibll.SaveForm(entity.ID, entity);
                return new { code = 0, info = "修改成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 删除工器具类别（大类）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerMonitor(6, "安卓终端删除工器具类别")]
        public object DeleteToolType()
        {
            var user = OperatorProvider.Provider.Current();

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;

                var list = tibll.GetList(id).ToList();
                int total = 0;
                foreach (ToolInfoEntity ti in list) //遍历工器具型号，判断是否有未归还的借用记录
                {
                    var tblist = tbbll.GetListByUserId(user.UserId, "", ti.ID, 1, 10000, out total).Where(x => x.BackDate == null);
                    if (tblist.Count() > 0)
                    {
                        return new { code = 1, info = "存在借用且未归还的工器具，无法删除！", data = new { } };
                    }
                }
                foreach (ToolInfoEntity ti in list) //所有工器具型号
                {
                    //删除编号信息
                    var nlist = tibll.GetToolNumberList(ti.ID).ToList();
                    tibll.RemoveToolNumberList(nlist);
                    tibll.RemoveFormList(list);
                }

                ttbll.RemoveForm(id);
                return new { code = 0, info = "删除成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 删除工器具型号（小类）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerMonitor(6, "安卓终端删除工器具型号")]
        public object DeleteToolInfo()
        {
            var user = OperatorProvider.Provider.Current();

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                int total = 0;
                var tblist = tbbll.GetListByUserId(user.UserId, "", id, 1, 10000, out total).Where(x => x.BackDate == null);
                if (tblist.Count() > 0)
                {
                    return new { code = 1, info = "存在借用且未归还的工器具，无法删除！", data = new { } };
                }
                var list = tibll.GetToolNumberList(id).ToList();
                tibll.RemoveToolNumberList(list);
                tibll.RemoveForm(id);
                return new { code = 0, info = "删除成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }



        /// <summary>
        /// 获取所有借用列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetBorrowsByUser()
        {
            //var user = OperatorProvider.Provider.Current();

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string name = dy.data.name;
                string userId = dy.userId;
                //string tid = dy.data.tId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<ToolBorrowEntity> list = tbbll.GetListByUserId(user.UserId, user.DepartmentId, "", 1, 10000, out total).Where(x => x.BackDate == null).ToList();
                total = list.Count;
                list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();


                return new { code = 0, info = "获取数据成功", count = total, data = new { toolBorrowList = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 工器具借用
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Borrow()
        {
            try
            {
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;

                string tid = dy.data.tId;
                UserEntity user = new UserBLL().GetEntity(userId);
                ToolInfoEntity t = tibll.GetEntity(tid);
                ToolBorrowEntity b = new ToolBorrowEntity();
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                int total = Convert.ToInt32(t.Total);
                int current = Convert.ToInt32(t.CurrentNumber);
                if (current > 0)
                {
                    t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) - 1).ToString();
                    tibll.SaveForm(t.ID, t);

                    b.ID = Guid.NewGuid().ToString();

                    b.IsGood = "";
                    b.Remark = "";
                    b.ToolName = t.Name;
                    b.ToolSpec = t.Spec;
                    b.BorrwoPerson = user.RealName;
                    b.BorrwoPersonId = user.UserId;
                    b.BorrwoDate = DateTime.Now;
                    b.BackDate = null;
                    b.TypeId = t.ID;
                    b.BZId = user.DepartmentId;
                    b.BZName = dept.FullName;
                    tbbll.SaveForm(string.Empty, b);
                    var messagebll = new MessageBLL();
                    messagebll.SendMessage("工器具借用消息", b.ID);
                    return new { code = 0, info = "借用成功" };
                }
                else
                {
                    return new { code = 1, info = "库存不足，无法借用" };
                }

            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 工器具借用
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object BorrowMore()
        {
            try
            {
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                var blist = dy.data.tools;
                string remark = dy.data.remark;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                ToolInfoEntity t = new ToolInfoEntity();
                ToolBorrowEntity b = new ToolBorrowEntity();
                int number = 0;
                string tid = "";
                int total = 0;
                int current = 0;
                for (int i = 0; i < blist.Count; i++)
                {
                    number = Convert.ToInt32(blist[i].number);
                    tid = blist[i].tId;

                    t = tibll.GetEntity(tid);
                    total = Convert.ToInt32(t.Total);
                    current = Convert.ToInt32(t.CurrentNumber);
                    if (current > 0)
                    {
                        t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) - number).ToString();
                        tibll.SaveForm(t.ID, t);

                        b.IsGood = "";
                        b.Remark = "";
                        b.ToolName = t.Name;
                        b.ToolSpec = t.Spec;
                        b.BorrwoPerson = user.RealName;
                        b.BorrwoPersonId = user.UserId;
                        b.BorrwoDate = DateTime.Now;
                        b.BackDate = null;
                        b.TypeId = t.ID;
                        b.BZId = user.DepartmentId;
                        b.BZName = dept.FullName;
                        b.Instruction = remark;
                        for (int j = 0; j < number; j++)
                        {
                            b.ID = Guid.NewGuid().ToString();
                            tbbll.SaveForm(string.Empty, b);
                        }
                        var messagebll = new MessageBLL();
                        messagebll.SendMessage("工器具借用消息", b.ID);
                    }
                }

                return new { code = 0, info = "借用成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 工器具归还
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Back([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);

                string userId = dy.userId;
                string tid = dy.data.tId;

                string state = dy.data.state;
                UserEntity user = new UserBLL().GetEntity(userId);
                ToolBorrowEntity tb = tbbll.GetEntity(tid);
                ToolInfoEntity t = tibll.GetEntity(tb.TypeId);
                if (t != null)
                {
                    t.State = state;
                    int total = Convert.ToInt32(t.Total);
                    int current = Convert.ToInt32(t.CurrentNumber);
                    if (current < total)
                    {
                        t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) + 1).ToString();  //更新工具当前数量

                        tibll.SaveForm(t.ID, t);


                        tb.BackDate = DateTime.Now;
                        //tb.IsGood = isgood;
                        tb.Remark = state;
                        tbbll.SaveForm(tb.ID, tb);

                        if (tb.Remark == "损坏" || tb.Remark == "失效")
                        {
                            var messagebll = new MessageBLL();
                            messagebll.SendMessage("工器具归还消息", tb.ID);
                        }
                        return new { code = 0, info = "归还成功" };
                    }
                    else
                    {
                        return new { code = 1, info = "数量超出总数，归还失败" };
                    }
                }
                else
                {
                    tb.BackDate = DateTime.Now;
                    //tb.IsGood = isgood;
                    tb.Remark = state;
                    tbbll.SaveForm(tb.ID, tb);
                    return new { code = 0, info = "归还成功" };
                }
            }
            catch (Exception ex)
            {
                return new { code = 2, info = ex.Message };
            }

        }

        /// <summary>
        /// 工器具归还
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object BackMore()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);

                string userId = dy.userId;
                // string tids = dy.data.tId;
                string ids = dy.data.bId;
                //string number = dy.data.number;
                string state = dy.data.state;
                UserEntity user = new UserBLL().GetEntity(userId);
                //ToolBorrowEntity tb = tbbll.GetEntity(tid);
                //ToolInfoEntity t = tibll.GetEntity(tb.TypeId);


                string[] arrId = ids.Split(',');
                foreach (string id in arrId)
                {
                    ToolBorrowEntity tb = tbbll.GetEntity(id);
                    ToolInfoEntity t = tibll.GetEntity(tb.TypeId);
                    t.State = state;
                    int total = Convert.ToInt32(t.Total);
                    int current = Convert.ToInt32(t.CurrentNumber);
                    //if (current < total)
                    //{
                    t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) + 1).ToString();  //更新工具当前数量

                    tibll.SaveForm(t.ID, t);


                    tb.BackDate = DateTime.Now;
                    //tb.IsGood = isgood;
                    tb.Remark = state;
                    tbbll.SaveForm(tb.ID, tb);
                    if (tb.Remark == "损坏" || tb.Remark == "失效")
                    {
                        var messagebll = new MessageBLL();
                        messagebll.SendMessage("工器具归还消息", tb.ID);
                    }
                    //}
                }
                return new { code = 0, info = "归还成功" };

            }
            catch (Exception ex)
            {
                return new { code = 2, info = ex.Message };
            }

        }


        [HttpPost]
        public object BackAll()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);

                string userId = dy.userId;

                string state = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total1 = 0;
                List<ToolBorrowEntity> list = tbbll.GetListByUserId(userId, user.DepartmentId, "", 1, 10000, out total1).Where(x => x.BackDate == null).ToList();
                //string[] arrId = ids.Split(',');
                string id = "";
                foreach (ToolBorrowEntity tb in list)
                {
                    ToolInfoEntity t = tibll.GetEntity(tb.TypeId);
                    t.State = state;
                    int total = Convert.ToInt32(t.Total);
                    int current = Convert.ToInt32(t.CurrentNumber);
                    if (current < total)
                    {
                        t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) + 1).ToString();  //更新工具当前数量

                        tibll.SaveForm(t.ID, t);


                        tb.BackDate = DateTime.Now;
                        //tb.IsGood = isgood;
                        tb.Remark = state;
                        tbbll.SaveForm(tb.ID, tb);

                    }
                }
                return new { code = 0, info = "归还成功" };

            }
            catch (Exception ex)
            {
                return new { code = 2, info = ex.Message };
            }

        }

        #region 安卓终端  获取列表数据
        /// <summary>
        /// 工器具种类列表（主页） 
        /// 大类列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllToolsNew()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("data");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string name = dy.data.name;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                if (!allowPaging) pageSize = 10000;
                List<ToolTypeEntity> list = ttbll.GetPageList(name, user.DepartmentId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();


                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }



        /// <summary>
        /// 工器具型号列表
        /// 小类列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolsByIdNew()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            //string res = json.Value<string>("json");
            //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            long pageIndex = dy.pageIndex;//当前索引页
            long pageSize = dy.pageSize;//每页记录数
            bool allowPaging = dy.allowPaging;
            string name = dy.data.name;
            string tid = dy.data.tId;
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);

            int total = 0;
            if (!allowPaging) pageSize = 10000;
            List<ToolInfoEntity> list = tibll.GetPageList(name, tid, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();


            return new { code = 0, info = "获取数据成功", count = total, data = list };
        }
        [HttpPost]
        public object GetImages()
        {
            try
            {
                IList<string> urls = new List<string>();
                string path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/static/images/tools/photo-";
                for (int i = 1; i < 22; i++)
                {
                    urls.Add(path + i + ".png");
                }


                return new { code = 0, info = "成功", data = urls };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取所有借用列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetBorrowsByUserNew()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string name = dy.data.name;
                string state = dy.data.state;
                string userId = dy.userId;
                string from = dy.data.from;
                string to = dy.data.to;
                //string type = dy.data.type;
                //string tid = dy.data.tId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                if (!allowPaging) pageSize = 10000;
                List<ToolBorrowEntity> list = tbbll.GetListByUserId(user.UserId, "", "", 1, 10000, out total).ToList();
                //if (type == "my")
                //{
                //    list = tbbll.GetListByUserId(userId, "", 1, 10000, out total).ToList();
                //}
                if (state == "1")
                {
                    list = tbbll.GetListByUserId(userId, user.DepartmentId, "", 1, 10000, out total).ToList();
                    list = list.Where(x => x.BackDate == null).ToList();
                }
                if (state == "0")
                {
                    list = tbbll.GetListByUserId(userId, user.DepartmentId, "", 1, 10000, out total).ToList();
                    list = list.Where(x => x.BackDate != null).ToList();
                }
                if (state == "2") //台账，全部
                {

                }



                if (!string.IsNullOrEmpty(from))
                {
                    DateTime f = DateTime.Parse(from);
                    list = list.Where(x => x.BorrwoDate > f).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    DateTime t = DateTime.Parse(to);

                    t = t.AddDays(1);

                    list = list.Where(x => x.BorrwoDate < t).ToList();
                }


                ToolInfoEntity ti = new ToolInfoEntity();
                ToolTypeEntity tt = new ToolTypeEntity();
                foreach (ToolBorrowEntity tb in list)
                {
                    ti = tibll.GetEntity(tb.TypeId);
                    if (ti != null)
                    {
                        tt = ttbll.GetEntity(ti.TypeId);
                        if (tt != null)
                        {
                            tb.Name = tt.Name;
                        }
                        else
                        {
                            tb.Name = "";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(x => x.BZId.Contains(name) || x.ToolName.Contains(name)).ToList();
                }
                total = list.Count();
                //分页数据
                list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        #endregion


        /// <summary>
        /// 获取所有工器具库数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>

        [HttpPost]
        public object GetAllToolInventory()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string type = dy.data.type;
                string name = dy.data.name;
                //string from = dy.data.from;
                //string to = dy.data.to;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = tinbll.GetList().Where(x => user.DepartmentCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DepartmentCode));

                if (!string.IsNullOrEmpty(type))
                {
                    list = list.Where(x => x.Type == type);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(x => x.Name.Contains(name));
                }
                int total = list.Count();
                if (allowPaging)
                {
                    //分页数据
                    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                }
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        [HttpPost]
        public object InsertToolInventory()
        {
            string res = HttpContext.Current.Request["json"];
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        type = string.Empty,//工器具类别
                        name = string.Empty//工器具名称
                    }
                });
                UserEntity user = new UserBLL().GetEntity(rq.userid);
                if (user == null) throw new ArgumentNullException("userid");
                if (rq.data == null) throw new ArgumentNullException("json.data");
                if (rq.data.type == null) throw new ArgumentNullException("json.data.type");
                if (rq.data.name == null) throw new ArgumentNullException("json.data.name");

                ToolInventoryEntity toolInventory = new ToolInventoryEntity()
                {
                    ID = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    BZId = user.DepartmentId,
                    DeptCode = user.DepartmentCode,
                    DeptId = user.DepartmentId,
                    Type = rq.data.type,
                    Name = rq.data.name,
                };
                //保存图标
                HttpPostedFile imgFile = HttpContext.Current.Request.Files["img"];
                if (imgFile != null && imgFile.ContentLength > 0 && imgFile.ContentType.Contains("image"))
                {
                    string ext = System.IO.Path.GetExtension(imgFile.FileName);//文件扩展名
                    string name = Guid.NewGuid().ToString();
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    var path = BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + name + ext;
                    imgFile.SaveAs(path);
                    //保存附件信息
                    var webPath = BSFramework.Util.Config.GetValue("AppUrl") + "/Resource/Content/toolpic/" + name + ext;
                    toolInventory.Path = webPath;
                }
                else
                {
                    return new { Code = -1, Info = "操作失败", Message = "上传的图标不是图片类型文件" };
                }

                tinbll.SaveForm(toolInventory.ID, toolInventory);
                return new { Code = 0, Info = "操作成功", Message = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { Code = 0, Info = "操作失败", ex.Message };
            }
        }

        /// <summary> 
        /// 获取工器具类型（库）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolTypes()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                var entity = new DataItemBLL().GetEntityByName("工器具类型");
                var list = new DataItemDetailBLL().GetList(entity.ItemId);
                list.Select(x => new
                {
                    x.ItemName
                });
                //var data = dataItemCache.GetDataItemList(EnCode);
                return new { info = "成功", code = 0, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }


        /// <summary>
        /// 检验台账
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllToolCheck()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            long pageIndex = dy.pageIndex;//当前索引页
            long pageSize = dy.pageSize;//每页记录数
            bool allowPaging = dy.allowPaging;

            UserBLL ubll = new UserBLL();
            DepartmentBLL dtbll = new DepartmentBLL();
            string userId = dy.userId;
            string name = dy.data.name;
            string from = dy.data.from;
            string to = dy.data.to;
            UserEntity user = ubll.GetEntity(userId);
            DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
            int total = 0;
            var list = tcbll.GetList().Where(x => x.BZId == user.DepartmentId);
            foreach (ToolCheckEntity t in list)
            {
                var toolinfo = tibll.GetEntity(t.ToolId);
                if (toolinfo != null)
                {
                    t.Spec = toolinfo.Spec;
                    t.Name = toolinfo.Name;
                }

            }
            if (!string.IsNullOrEmpty(from))
            {
                list = list.Where(x => x.CheckDate >= DateTime.Parse(from));
            }
            if (!string.IsNullOrEmpty(to))
            {
                list = list.Where(x => x.CheckDate < DateTime.Parse(from).AddDays(1));
            }
            total = list.Count();
            list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
            return new { code = 0, info = "获取数据成功", count = total, data = list };

        }
        /// <summary>
        /// 获取单个工器具检验记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolCheckById()
        {
            try
            {
                string path = BSFramework.Util.Config.GetValue("AppUrl");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                var list = tcbll.GetList().Where(x => x.ToolId == id).OrderByDescending(x => x.CreateDate);
                foreach (var i in list)
                {
                    i.Files = fileBll.GetFilesByRecIdNew(i.ID);
                    foreach (FileInfoEntity f in i.Files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", path);
                    }
                }
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取单个工器具维修记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolRepairById()
        {
            try
            {
                string path = BSFramework.Util.Config.GetValue("AppUrl");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                var list = trbll.GetList().Where(x => x.ToolId == id).OrderByDescending(x => x.CreateDate);
                foreach (var i in list)
                {
                    i.Files = fileBll.GetFilesByRecIdNew(i.ID);
                    foreach (FileInfoEntity f in i.Files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", path);
                    }
                }
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 维修台账
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllToolRepair([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data.name;
                string from = dy.data.from;
                string to = dy.data.to;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                int total = 0;
                var list = trbll.GetList().Where(x => x.BZId == user.DepartmentId);
                foreach (ToolRepairEntity t in list)
                {
                    var toolinfo = tibll.GetEntity(t.ToolId);
                    if (toolinfo != null)
                    {
                        t.Spec = toolinfo.Spec;
                        t.Name = toolinfo.Name;
                    }

                }
                if (!string.IsNullOrEmpty(from))
                {
                    list = list.Where(x => x.RepairDate >= DateTime.Parse(from));
                }
                if (!string.IsNullOrEmpty(to))
                {
                    list = list.Where(x => x.RepairDate < DateTime.Parse(from).AddDays(1));
                }
                total = list.Count();
                list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增检验记录
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object AddToolCheck()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string id = Guid.NewGuid().ToString();
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                ToolCheckEntity tc = new ToolCheckEntity();
                tc.ID = id;
                tc.BZId = user.DepartmentId;
                tc.CreateDate = DateTime.Now;
                tc.CreateUserId = user.UserId;
                tc.CheckResult = dy.data.CheckResult;
                tc.Numbers = dy.data.Numbers;
                tc.ValiDate = Convert.ToDateTime(dy.data.ValiDate);
                tc.CheckPeople = dy.data.CheckPeople;
                //tc.CheckPeopleId = dy.data.CheckPeopleId;
                tc.CheckDate = Convert.ToDateTime(dy.data.CheckDate);
                tc.ToolId = dy.data.toolId;
                // tcbll.SaveForm(tc.ID, tc);

                string fileid = dy.data.ReportUploadId;
                if (!string.IsNullOrEmpty(fileid))
                {
                    var file = fileBll.GetEntity(fileid);
                    if (file != null)
                    {
                        tc.ID = file.RecId;
                        tc.Path = file.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    }
                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                if (files.Count > 0)
                {
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = tc.ID,
                        RecId = tc.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    tc.Path = fi.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                }

                tcbll.SaveForm(tc.ID, tc);
                ToolInfoEntity ti = tibll.GetEntity(tc.ToolId);
                ti.ValiDate = tc.ValiDate;
                tibll.SaveForm(ti.ID, ti);
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增维修记录
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object AddToolRepair()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string id = Guid.NewGuid().ToString();
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                ToolRepairEntity tc = new ToolRepairEntity();
                tc.ID = id;
                tc.BZId = user.DepartmentId;
                tc.Amount = Convert.ToInt32(dy.data.Amount);
                tc.RepairResult = dy.data.RepairResult;
                tc.Numbers = dy.data.Numbers;
                tc.RepairDate = Convert.ToDateTime(dy.data.RepairDate);
                tc.RepairPeople = dy.data.RepairPeople;
                //tc.RepairPeopleId = dy.data.RepairPeopleId;
                tc.CreateDate = DateTime.Now;
                tc.CreateUserId = user.UserId;
                tc.ToolId = dy.data.toolId;
                // tcbll.SaveForm(tc.ID, tc);

                ToolInfoEntity ti = tibll.GetEntity(tc.ToolId);
                if (tc.RepairResult == "报废")  //更新数量
                {

                    if (ti != null)
                    {
                        ti.Total = (int.Parse(ti.Total) - tc.Amount).ToString();
                    }
                    var nlist = ti.Numbers.Split(',').ToList();
                    var nernumber = "";
                    var numbers = tc.Numbers.Split(',');
                    foreach (string number in numbers)   //修改编号信息
                    {
                        if (!string.IsNullOrEmpty(number))
                        {
                            nlist.Remove(number);
                            var tn = tibll.GetToolNumberList(ti.ID).Where(x => x.Number == number).FirstOrDefault();
                            if (tn != null)
                            {
                                tn.IsBreak = true;
                                tibll.SaveToolNumber(tn.ID, tn);
                            }
                            ti.BreakNumbers += number + ",";
                        }
                    }
                    foreach (string a in nlist)
                    {
                        nernumber += a + ",";
                    }
                    if (nernumber.Length > 1)
                    {
                        nernumber = nernumber.Substring(0, nernumber.Length - 1);
                    }
                    if (ti.BreakNumbers.Length > 1) ti.BreakNumbers = ti.BreakNumbers.Substring(0, ti.BreakNumbers.Length - 1);
                    ti.Numbers = nernumber;

                }


                string fileid = dy.data.ReportUploadId;
                if (!string.IsNullOrEmpty(fileid))
                {
                    var file = fileBll.GetEntity(fileid);
                    if (file != null)
                    {
                        tc.ID = file.RecId;

                        tc.Path = file.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                    }
                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                if (files.Count > 0)
                {
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = tc.ID,
                        RecId = tc.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/toolpic/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\toolpic\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    tc.Path = fi.FilePath.Replace("~/", BSFramework.Util.Config.GetValue("AppUrl"));
                }
                trbll.SaveForm(tc.ID, tc);

                tibll.SaveForm(ti.ID, ti);
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetToolInventoryInfo()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                var tooltype = ttbll.GetEntity(id);
                var toolinventory = tinbll.GetEntity(tooltype.InventoryId);
                var file = fileBll.GetFilesByRecIdNew(toolinventory.ID).Where(x => x.Description == "2").ToList().FirstOrDefault();
                if (file != null)
                {
                    string msds = BSFramework.Util.Config.GetValue("AppUrl") + file.FilePath.TrimStart('~');
                    var urlPDF = Config.GetValue("pdfview");
                    toolinventory.File = urlPDF + file.FileId;
                }
                return new { info = "成功", code = 0, data = toolinventory };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        [HttpPost]
        public object GetFileId()
        {
            try
            {
                NewFileInfoBLL nfbll = new NewFileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                NewFileInfoEntity f = new NewFileInfoEntity();
                f.ID = Guid.NewGuid().ToString();
                f.CreateDate = DateTime.Now;
                f.Amount = (Int32)dy.data.Amount;
                f.Instruction = dy.data.Instruction;
                f.IsImg = dy.data.IsImg;
                f.Title = dy.data.Title;
                f.CreateUser = user.RealName;
                f.CreateUserId = user.UserId;
                f.RecId = Guid.NewGuid().ToString();
                nfbll.SaveForm(f.ID, f);
                return new { info = "成功", code = 0, data = f.ID };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        [HttpPost]
        public object GetNewFile()
        {
            try
            {
                NewFileInfoBLL nfbll = new NewFileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                var f = nfbll.GetEntity(id);
                return new { info = "成功", code = 0, data = f };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        //[HttpPost]
        //public object UpLoadFile()
        //{
        //    try
        //    {
        //        NewFileInfoBLL nfbll = new NewFileInfoBLL();
        //        dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
        //        string id = dy.data;
        //        FileInfoBLL fileBll = new FileInfoBLL();
        //        HttpFileCollection files = HttpContext.Current.Request.Files;
        //        FileInfoEntity fi = null;
        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            HttpPostedFile hf = files[i];
        //            string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
        //            string fileId = Guid.NewGuid().ToString();//上传后文件名
        //            fi = new FileInfoEntity
        //            {
        //                FileId = fileId,
        //                FolderId = id,
        //                RecId = id,
        //                FileName = System.IO.Path.GetFileName(hf.FileName),
        //                FilePath = "~/Resource/AppFile/ToolFile/" + fileId + ext,
        //                FileType = System.IO.Path.GetExtension(hf.FileName),
        //                FileExtensions = ext,
        //                FileSize = hf.ContentLength.ToString(),
        //                DeleteMark = 0
        //            };

        //            //上传附件到服务器
        //            if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ToolFile"))
        //            {
        //                System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ToolFile");
        //            }
        //            hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ToolFile\\" + fileId + ext);
        //            //保存附件信息
        //            fileBll.SaveForm(fi);
        //        }
        //        return new { info = "成功", code = 0, data = new { } };
        //    }
        //    catch (Exception ex)
        //    {

        //        return new { info = "失败：" + ex.Message, code = 1, data = new { } };
        //    }
        //}

        [HttpPost]
        public object GetNewFileList([FromBody] JObject json)
        {
            try
            {
                NewFileInfoBLL nfbll = new NewFileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list = fileBll.GetFilesByRecIdNew(id);
                foreach (FileInfoEntity f in list)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                return new { info = "成功", code = 0, data = list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        #region 工器具导入
        /// <summary>
        /// 批量导入工器具
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object ToolBatchExport()
        {
            string res = HttpContext.Current.Request["json"];
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        deptid = string.Empty,
                    }
                });
                //数据验证
                if (rq == null) throw new ArgumentNullException("json");
                if (string.IsNullOrWhiteSpace(rq.userid)) throw new ArgumentNullException("userid");
                if (rq.data == null) throw new ArgumentNullException("data");
                if (string.IsNullOrWhiteSpace(rq.data.deptid)) throw new ArgumentNullException("data.deptid");
                DepartmentEntity dept = new DepartmentBLL().GetEntity(rq.data.deptid);
                if (dept == null) throw new Exception("传入的部门Id不对");


                Style cellStyle = new Style();
                cellStyle.ForegroundColor = ColorTranslator.FromHtml("#ff0000");
                cellStyle.Pattern = BackgroundType.Solid;

                List<ToolInfoEntity> insertTools = new List<ToolInfoEntity>();//要插入到数据库里面的数据
                HttpFileCollection fileCollection = HttpContext.Current.Request.Files;
                bool valid = true;
                if (fileCollection != null && fileCollection.Count > 0)
                {

                    List<ToolTypeEntity> toolTypes = ttbll.GetList(dept.DepartmentId).ToList();//该班组底下所有的工器具类别

                    var file = fileCollection[0];
                    var book = new Workbook(file.InputStream);
                    var sheet = book.Worksheets[0];
                    for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        #region 数据验证
                        if (i < 2) continue;
                        if (string.IsNullOrWhiteSpace(sheet.Cells[i, 0].StringValue))
                        {
                            //var style = sheet.Cells[i, 0].GetStyle();
                            //style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#ff0000");
                            //style.Pattern = BackgroundType.Solid;
                            ////sheet.Cells[i, 0].SetStyle(style, new StyleFlag() { All = true });
                            //var range = sheet.Cells.CreateRange(i, 0, 1, 1);
                            //range.ApplyStyle(cellStyle, new StyleFlag() { All = true });
                            sheet.Cells[i, 0].SetStyle(cellStyle);
                            sheet.Cells[i, 0].Value += "（不能为空）";
                            valid = false;
                            /*throw new Exception(string.Format("{0}行“工器具类别“不能为空",i+1));*/
                        }
                        if (string.IsNullOrWhiteSpace(sheet.Cells[i, 1].StringValue))
                        {
                            sheet.Cells[i, 1].SetStyle(cellStyle); /*throw new Exception(string.Format("{0}行“工器具名称“不能为空", i + 1));*/
                            sheet.Cells[i, 1].Value += "（不能为空）";
                            valid = false;
                        }
                        if (string.IsNullOrWhiteSpace(sheet.Cells[i, 2].StringValue))
                        {
                            sheet.Cells[i, 2].SetStyle(cellStyle); /*throw new Exception(string.Format("{0}行“型号“不能为空", i + 1));*/
                            sheet.Cells[i, 2].Value += "（不能为空）";
                            valid = false;
                        }
                        if (string.IsNullOrWhiteSpace(sheet.Cells[i, 3].StringValue))
                        {
                            sheet.Cells[i, 3].SetStyle(cellStyle); /*throw new Exception(string.Format("{0}行“数量“不能为空", i + 1));*/
                            sheet.Cells[i, 3].Value += "（不能为空）";
                            valid = false;
                        }
                        //数量验证
                        int amout = 0;
                        if (!int.TryParse(sheet.Cells[i, 3].StringValue, out amout))
                        {
                            sheet.Cells[i, 3].SetStyle(cellStyle);
                            sheet.Cells[i, 3].Value += "（输入的不是整数）";
                            //throw new Exception(string.Format("{0}行“数量“输入的不是整数", i + 1));
                            valid = false;
                        }
                        //工器具分类验证
                        ToolTypeEntity toolType = toolTypes.FirstOrDefault(x => x.Name.Equals(sheet.Cells[i, 0].StringValue));
                        if (toolType == null)
                        {
                            sheet.Cells[i, 0].SetStyle(cellStyle);
                            sheet.Cells[i, 0].Value += "（工器具类别不存在，请确定该分类已经在系统添加）";
                            valid = false;
                            //throw new Exception(string.Format("{0}行“工器具类别“不存在，请确定该分类已经添加", i + 1));
                        }
                        //出厂日期验证
                        DateTime? outDate = null;
                        DateTime tryTime = DateTime.MinValue;
                        if (!string.IsNullOrWhiteSpace(sheet.Cells[i, 6].StringValue))
                        {
                            if (!DateTime.TryParse(sheet.Cells[i, 6].StringValue, out tryTime))
                            {
                                sheet.Cells[i, 6].SetStyle(cellStyle);
                                sheet.Cells[i, 6].Value += "（不是有效的时间格式。(格式：yyyy-MM-dd)）";
                                valid = false;
                                //throw new Exception(string.Format("{0}行“出厂日期“不是有效的时间格式。(格式：yyyy-MM-dd)", i + 1));
                            }
                            else
                            {
                                outDate = tryTime;
                            }
                        }
                        //有效日期验证
                        DateTime? ValiDate = null;
                        if (!string.IsNullOrWhiteSpace(sheet.Cells[i, 9].StringValue))
                        {
                            if (!DateTime.TryParse(sheet.Cells[i, 9].StringValue, out tryTime))
                            {
                                sheet.Cells[i, 9].SetStyle(cellStyle);
                                sheet.Cells[i, 9].Value += "（不是有效的时间格式。(格式：yyyy-MM-dd)）";
                                valid = false;
                                //throw new Exception(string.Format("{0}行“有效日期“不是有效的时间格式。(格式：yyyy-MM-dd)", i + 1));
                            }
                            else
                            {
                                ValiDate = tryTime;
                            }
                        }
                        //有效检验日期
                        DateTime? checkDate = null;
                        if (!string.IsNullOrWhiteSpace(sheet.Cells[i, 11].StringValue))
                        {
                            if (!DateTime.TryParse(sheet.Cells[i, 11].StringValue, out tryTime))
                            {
                                sheet.Cells[i, 11].SetStyle(cellStyle);
                                sheet.Cells[i, 11].Value += "（不是有效的时间格式。(格式：yyyy-MM-dd)）";
                                valid = false;
                                //throw new Exception(string.Format("{0}行“有效日期“不是有效的时间格式。(格式：yyyy-MM-dd)", i + 1));
                            }
                            else
                            {
                                checkDate = tryTime;
                            }
                        }
                        //检验提醒
                        var Remind = sheet.Cells[i, 10].StringValue;
                        //检验周期
                        var CheckCycle = sheet.Cells[i, 8].StringValue;
                        //4、若要定期检验，则检验周期、有效期、检验提醒、下次检验日期为必填。
                        //5、若不需要定期检验，则检验周期、有效期、检验提醒、下次检验日期均不填。 
                        if (checkDate != null || ValiDate != null || !string.IsNullOrEmpty(CheckCycle) || !string.IsNullOrEmpty(Remind))
                        {
                            if (checkDate == null)
                            {
                                sheet.Cells[i, 11].SetStyle(cellStyle);
                                sheet.Cells[i, 11].Value += "定期检验，请写完全检验日期！";
                                valid = false;
                            }
                            if (ValiDate == null)
                            {
                                sheet.Cells[i, 11].SetStyle(cellStyle);
                                sheet.Cells[i, 11].Value += "定期检验，请写完全有效期！";
                                valid = false;
                            }
                            if (string.IsNullOrEmpty(CheckCycle))
                            {
                                sheet.Cells[i, 11].SetStyle(cellStyle);
                                sheet.Cells[i, 11].Value += "定期检验，请写完全检验周期！";
                                valid = false;
                            }
                            if (string.IsNullOrEmpty(Remind))
                            {
                                sheet.Cells[i, 11].SetStyle(cellStyle);
                                sheet.Cells[i, 11].Value += "定期检验，请写完全检验提醒！";
                                valid = false;
                            }
                        }

                        #endregion
                        ToolInfoEntity toolInfo = new ToolInfoEntity()
                        {
                            ID = Guid.NewGuid().ToString(),
                            BZID = dept.DepartmentId,
                            BZName = dept.FullName,
                            Amount = amout,
                            Name = sheet.Cells[i, 1].StringValue,
                            TypeId = toolType?.ID,
                            Spec = sheet.Cells[i, 2].StringValue,
                            Numbers = sheet.Cells[i, 4].StringValue,
                            DepositPlace = sheet.Cells[i, 5].StringValue,
                            OutDate = outDate,
                            ProFactory = sheet.Cells[i, 7].StringValue,
                            CheckCycle = CheckCycle,
                            ValiDate = ValiDate,
                            Remind = Remind,
                            CreateDate = DateTime.Now,
                            ToolcheckDate = checkDate,
                            Total = amout.ToString(),
                            CurrentNumber = amout.ToString(),
                            RegDate = null
                        };
                        insertTools.Add(toolInfo);
                    }
                    if (valid)
                    {
                        tibll.Insert(insertTools.ToArray());
                    }
                    else
                    {
                        var filedir = Config.GetValue("FilePath");
                        filedir = Path.Combine(filedir, "AppFile", "BathExprotTool");
                        var Server = HttpContext.Current.Server;
                        if (!Directory.Exists(filedir)) Directory.CreateDirectory(filedir);
                        var filename = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                        filedir += "\\" + filename;
                        book.Save(filedir);
                        return new { Code = -2, Info = "操作成功", data = Config.GetValue("AppUrl") + "/Resource/AppFile/BathExprotTool" + filename };
                    }
                    return new { Code = 0, Info = "操作成功", data = string.Format("成功导入{0}条数据", insertTools.Count) };
                }
                else
                {
                    throw new ArgumentNullException("File");
                }
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "上传失败", data = ex.Message };
            }

        }

        /// <summary>
        /// 下载工器具批量导入模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetTemplate()
        {
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            url += "Content/export/工器具批量导入模板.xlsx";
            return url;
        }

        /// <summary>
        /// 获取工器具分类
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetToolType([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                if (dy.data == null) throw new ArgumentNullException("data");
                if (dy.data.deptid == null || string.IsNullOrWhiteSpace(dy.data.deptid)) throw new ArgumentNullException("data.deptid");
                DepartmentEntity dept = new DepartmentBLL().GetEntity(dy.data.deptid);
                if (dept == null) throw new Exception("未找到对应的班组");
                List<ToolTypeEntity> toolTypes = ttbll.GetList(dept.DepartmentId).ToList();//该班组底下所有的工器具类别
                return new { code = 0, data = toolTypes, info = "请求成功", count = toolTypes.Count };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "请求失败", data = ex.Message, count = 0 };
            }
        }
        #endregion

    }
}
