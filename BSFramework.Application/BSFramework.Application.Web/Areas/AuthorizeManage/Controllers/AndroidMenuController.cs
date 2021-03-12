using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.AuthorizeManage.Controllers
{
    public class AndroidMenuController : MvcControllerBase
    {
        private AndroidmenuBLL bll = new AndroidmenuBLL();

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///终端菜单管理表格
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        public ActionResult GetMenuTreeJson(string keyword)
        {
            var treeList = new List<TreeGridEntity>();
            var dataMenu = bll.GetList().ToList().OrderBy(x => x.Sort);
            foreach (var item in dataMenu)
            {
                TreeGridEntity tree = new TreeGridEntity();
                bool hasChildren = dataMenu.Count(t => t.ParentId == item.MenuId) == 0 ? false : true;
                tree.id = item.MenuId;
                tree.text = item.MenuName;
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    tree.parentId = "0";
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.expanded = true;
                tree.hasChildren = hasChildren;
                string itemJson = item.ToJson();
                //itemJson = itemJson.Insert(1, "\"Sort\":\"Sort\",");
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                treeList = treeList.TreeWhere(t => t.text.Contains(keyword), "id", "parentId");
            }
            var content = treeList.TreeJson();
            return Content(content);

        }

        public ActionResult Form()
        {
            ViewBag.Guid = Guid.NewGuid();
            return View();
        }
        public ActionResult SaveForm(AndroidmenuEntity  entity)
        {
            entity.Icon = Url.Content("~" + entity.Icon);
            bll.addAndroidmenu(entity);
            return Success("操作成功。");
        }
        public ActionResult UploadFileNew(string uptype, string id)
        {

            FileInfoBLL fileBll = new FileInfoBLL();
            IList<FileInfoEntity> fl = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == uptype).ToList();
            foreach (FileInfoEntity fe in fl)
            {
                string filepath = fileBll.Delete(fe.FileId);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath("~" + filepath)))
                    System.IO.File.Delete(Server.MapPath("~" + filepath));
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;
            if (uptype == "0" && !type.Contains("image"))  //图片
            {
                return Success("1");
            }

            if (uptype == "1" && !type.Contains("mp4"))
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath =string.Format("~/Content/AndroidMenufile/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = id,
                RecId = id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = uptype
            };
            fileBll.SaveForm(fi);
            return Success("上传成功。", new { path = Url.Content(virtualPath), name = fi.FileName });
        }
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                var del = bll.delAndroidmenu(keyValue);
                FileInfoBLL fileBll = new FileInfoBLL();
                foreach (FileInfoEntity fe in del)
                {
                    string filepath = fileBll.Delete(fe.FileId);
                    if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath("~" + filepath)))
                        System.IO.File.Delete(Server.MapPath("~" + filepath));
                }
                return Success("删除成功。");
            }
            catch (Exception ex)
            {

                return Success("删除失败。"+ex.Message);
            }   
         
        }

        public ActionResult Edit(string keyValue) {

            var dataMenu = bll.GetList().FirstOrDefault(x=>x.MenuId== keyValue);
            dataMenu.worktype = "1";
            return View(dataMenu);

        }
    }


}
