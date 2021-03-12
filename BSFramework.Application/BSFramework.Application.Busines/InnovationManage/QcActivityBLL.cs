using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Application.Service.InnovationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.InnovationManage
{
    public class QcActivityBLL
    {
        private IQcActivityService service = new QcActivityService();
        private FileInfoBLL fileBll = new FileInfoBLL();
        private UserBLL userbll = new UserBLL();
        private DepartmentBLL deptBll = new DepartmentBLL();
        /// <summary>
        /// 获取qc数据
        /// </summary>
        /// <returns></returns>
        public List<QcActivityEntity> getQcList(Dictionary<string, string> keyValue, Pagination pagination, bool ispage = false)
        {
            if (!ispage)
            {
                if (pagination == null)
                {
                    pagination = new Pagination();
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
            }
            var data = service.getQcList(keyValue, pagination);
            foreach (var item in data)
            {

                var fileList = fileBll.GetFilesByRecIdNew(item.qcid);
                item.Files = fileList.Where(x => x.Description == "文件").ToList();
                item.Photos = fileList.Where(x => x.Description == "照片").ToList();
            }
            return data;

        }
        public DepartmentEntity getUserDept(string deptid)
        {
            return deptBll.GetEntity(deptid);
        }

        /// <summary>
        ///获取数据根据id
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public QcActivityEntity getQcById(string keyvalue)
        {
            var data = service.getQcById(keyvalue);

            var fileList = fileBll.GetFilesByRecIdNew(data.qcid);
            data.Files = fileList.Where(x => x.Description == "文件").ToList();
            data.Photos = fileList.Where(x => x.Description == "照片").ToList();

            return data;

        }

        /// <summary>
        /// 新增数据qc活动数据
        /// </summary>
        public void addEntity(QcActivityEntity qc)
        {
            workDept(qc);
            service.addEntity(qc);

        }
        private void workDept(QcActivityEntity qc)
        {

            qc.workdeptid = string.Empty;
            if (!string.IsNullOrEmpty(qc.grouppersonid))
            {

                if (!string.IsNullOrEmpty(qc.groupbossid))
                {
                    var dept = userbll.GetEntity(qc.groupbossid);
                    qc.workdeptid = dept.DepartmentId;
                }
               
                var deptList = qc.grouppersonid.Split(',').ToList();
                foreach (var item in deptList)
                {
                    var getdept = userbll.GetEntity(item);
                    if (getdept==null)
                    {
                        continue;
                    }
                    if (qc.workdeptid.Contains(getdept.DepartmentId))
                    {
                        continue;
                    }

                    qc.workdeptid = qc.workdeptid + "," + getdept.DepartmentId;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(qc.groupbossid))
                {
                    var getdept = userbll.GetEntity(qc.groupbossid);
                    qc.workdeptid = getdept.DepartmentId;
                }
           
            }
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="qc"></param>
        public void EditEntity(QcActivityEntity qc)
        {
            workDept(qc);
            service.EditEntity(qc);

        }
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void delEntity(string keyValue)
        {

            var qc = getQcById(keyValue);
            delEntity(qc);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="qc"></param>
        public void delEntity(QcActivityEntity qc)
        {

            service.delEntity(qc);

        }

        public int GetQcTimes(string deptid)
        {
            return service.GetQcTimes(deptid);
        }
    }
}
