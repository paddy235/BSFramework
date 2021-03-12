using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.PerformanceManage.ViewModel;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Application.Service.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.PerformanceManage
{
    /// <summary>
    /// 绩效管理数据
    /// </summary>
    public class PerformanceBLL
    {

        private IPerformanceService service = new PerformanceService();
        private IPerformancesetupService Setupservice = new PerformancesetupService();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<PerformanceEntity> getScore(string time, string departmentid)
        {
            return service.getScore(time, departmentid).ToList();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public PerformanceEntity getScoreByuser(string time, string userid)
        {
            return service.getScoreByuser(time, userid);
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        public void operation(List<PerformanceEntity> entity)
        {
            try
            {
                service.operation(entity);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取绩效数据
        /// </summary>
        public object getScoreList(string time, string departmentid)
        {
            //所有数据

            var message = string.Empty;
            try
            {
                var allinfo = service.getScore(time, departmentid).OrderBy(x => x.planer).ThenBy(x => x.username).ToList();
                double total = 0;
                double avg = 0;
                foreach (var item in allinfo)
                {
                    item.score = item.score.Split(',')[0];
                    total = total + double.Parse(item.score);
                }
                if (allinfo.Count() != 0)
                {
                    avg = total / allinfo.Count();
                    avg = Math.Round(avg, 2);
                }
                total = Math.Round(total, 2);
                return new { code = 0, info = "操作成功", data = new { total = total, avg = avg, list = allinfo } };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };

            }


        }

        /// <summary>
        /// 获取个人绩效数据
        /// </summary>
        public object getScore(string userid, string time, string departmentid)
        {
            //所有数据
            var message = string.Empty;
            try
            {
                List<double> list = new List<double>();
                List<double> personallist = new List<double>();
                var month = Convert.ToInt32(time);
                for (int i = 1; i <= 12; i++)
                {
                    string getTime = new DateTime(month, i, 1).ToString("yyyy-MM-dd");
                    double total = 0;
                    double avg = 0;
                    double personalval = 0;
                    var allinfo = service.getScore(getTime, departmentid).OrderBy(x => x.planer).ThenBy(x => x.username).ToList();
                    foreach (var item in allinfo)
                    {
                        item.score = item.score.Split(',')[0];
                        if (item.userid == userid)
                        {
                            personalval = double.Parse(item.score);
                            personalval = Math.Round(personalval, 2);
                        }
                        total = total + double.Parse(item.score);
                    }
                    var num = allinfo.Count();
                    if (num != 0)
                    {
                        avg = total / num;
                        avg = Math.Round(avg, 2);
                    }
                    personallist.Add(personalval);
                    list.Add(avg);
                }
                return new { code = 0, info = "操作成功", data = new { list = list, personallist = personallist } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }


        /// <summary>
        /// 实时修改任务分数
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="Score">现在分数</param>
        /// <param name="type">类型名称 实际出勤（班次）值班（次） 完成任务数 任务总分</param>
        public void changeScore(string userid, string Score, string type)
        {

            try
            {
                var time = DateTime.Now;
                var usetime = new DateTime(time.Year, time.Month, 1).ToString("yyyy-MM-dd");
                var user = service.getUserScore(usetime, userid);
                if (user != null)
                {

                    var title = Setupservice.AllTitle(user.deparmentid).Where(x => x.isuse).ToList();
                    var one = title.FirstOrDefault(x => x.name == type);
                    var num = one.sort;
                    var getScore = user.score.Split(',').ToList();
                    if (num != -1)
                    {
                        getScore[num] = Score;
                        user.score = string.Join(",", getScore);
                        List<PerformanceEntity> entity = new List<PerformanceEntity>();
                        entity.Add(user);
                        service.operation(entity);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private UserWorkAllocationBLL bll = new UserWorkAllocationBLL();
        /// <summary>
        /// 获取部门班组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<DepartmentEntity> getDepartmentList(string id, string category)
        {

            if (string.IsNullOrEmpty(id))
            {
                var rootdpet = departmentBLL.GetRootDepartment();
                id = rootdpet.DepartmentId;
            }
            var depts = bll.GetSubDepartments(id, category);
            return depts;
        }

        public List<PerformanceEntity> getScore(string usetime, List<string> departIds)
        {
            return service.getScore(usetime, departIds);
        }

        public void Insert(List<PerformanceEntity> addData)
        {
            service.Insert(addData);
        }

        public void Delete(List<PerformanceEntity> delData)
        {
            service.Delete(delData);
        }

        public List<PerformanceModel> getYearScoreByuser(string year, string userid)
        {
            return service.getYearScoreByuser(year, userid);
        }
    }
}
