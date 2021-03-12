using BSFramework.Application.Busines.CertificateManage;
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.AuthorizeManage.Controllers
{
    /// <summary>
    /// 证书类别
    /// </summary>
    public class CertificateController : MvcControllerBase
    {
        private CertificateBLL bll = new CertificateBLL();

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult GetCertificateTreeJson(string keyword)
        {
            var treeList = new List<TreeGridEntity>();
            var dataMenu = bll.GetCertificate();
            foreach (var item in dataMenu)
            {
                TreeGridEntity tree = new TreeGridEntity();
                bool hasChildren = dataMenu.Count(t => t.ParentId == item.CertificateId) == 0 ? false : true;
                tree.id = item.CertificateId;
                tree.text = item.CertificateName;
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
        /// <summary>
        /// 新增页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            return View();
        }

        /// <summary>
        /// 新增修改类别
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SaveForm(CertificateEntity entity)
        {
            bll.addCertificate(entity);
            return Success("操作成功。");
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult RemoveForm(string keyValue)
        {
            bll.delCertificate(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 修改页面
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult Edit(string keyValue)
        {

            var Certificate = bll.GetCertificate().FirstOrDefault(x => x.CertificateId == keyValue);
            return View(Certificate);

        }
    }
}
