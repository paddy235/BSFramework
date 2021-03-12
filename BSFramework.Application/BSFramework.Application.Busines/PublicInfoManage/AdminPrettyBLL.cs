using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Cache.Factory;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    public class AdminPrettyBLL
    {
        private IAdminPrettyService service = new AdminPrettyService();
        private IEduBaseInfoService eduBaseInfoService = new EduBaseInfoService();
        private WorkmeetingIService workmeetingService = new WorkmeetingService();
        private ActivityIService activityIService = new ActivityService();
        private EdActivityIService edActivityIService = new EdActivityService();
        private DangerIService dangerService = new DangerService();


        public List<KeyValue> FindCount(string deptcode, string[] depts)
        {
            var user = OperatorProvider.Provider.Current();

            List<KeyValue> skData = new List<KeyValue>();
            try
            {
                var data = new
                {
                    userid = user.UserId
                    //userid = "006c685c-9090-4a43-8d2f-0634488a409e"
                };
                string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getBZPlatformIndexInfo"), "json=" + JsonConvert.SerializeObject(data));
                var ret = JsonConvert.DeserializeObject<ResponesModel>(res);
                if (ret.code !="-1")
                {
                    skData = JsonConvert.DeserializeObject<List<KeyValue>>(ret.data.ToString());
                    skData.ForEach(x =>
                    {
                        x.dataType = 1;
                    });
                }
            }
            catch (Exception ex)
            {

            }

            //var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var end = start.AddMonths(1);

            ////技术讲课
            //var jsjk = eduBaseInfoService.Count(depts, new string[] { "1" }, start, end);
            ////技术问答
            //var jswd = eduBaseInfoService.Count(depts, new string[] { "2", "5" }, start, end);
            ////事故预想
            //var sgyx = eduBaseInfoService.Count(depts, new string[] { "3" }, start, end);
            ////反事故演习
            //var fsgyx = eduBaseInfoService.Count(depts, new string[] { "4" }, start, end);
            ////班前班后会
            //var bqbhh = workmeetingService.Count(depts, start, end);
            ////安全日活动
            //var aqrhd = activityIService.Count(depts, "安全日活动", start, end);
            ////安全学习日
            //var aqxxr = edActivityIService.Count(depts, "安全学习日", start, end);
            ////危险预知训练
            //var wxyzxl = dangerService.Count(depts, start, end);


            //var result = new List<KeyValue> {
            //    new KeyValue { key = "BZ_JSJK", value = jsjk.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_JSWD", value = jswd.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_FSGXX", value = fsgyx.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_BQBHH", value = bqbhh.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_AQRHD", value = aqrhd.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_AQXXR", value = aqxxr.ToString(), dataType = 0 },
            //    new KeyValue { key = "BZ_WXYZXL", value = wxyzxl.ToString(), dataType = 0 },
            //};

            //if (skData != null) result.AddRange(skData);


            List<KeyValue> bzdata = service.FindCount(deptcode);
            bzdata.ForEach(x =>
            {
                x.dataType = 0;
            });
            if (skData != null)
            {
                bzdata.AddRange(skData);
            }
            return bzdata;

            //return result;
        }

        public List<KeyValue> FindCount1(string deptId, string TerminalType)
        {
            List<KeyValue> bzdata = service.FindCount1(deptId, TerminalType);
            return bzdata;
        }

        /// <summary>
        /// 获取所有的指标，平台。终端。app 所有的指标的数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="deptId">用户所在的部门Id</param>
        /// <param name="codeAndTerminalType">key: 指标编码code     value：查询类型  0月   1季度  2 年</param>
        /// <returns></returns>
        public List<KeyValue> FindAllCount(string userId, string deptId, Dictionary<string, string> codeAndTerminalType)
        {
            List<KeyValue> bzdata = new List<KeyValue>();


            List<KeyValue> skData = new List<KeyValue>();
            var data = new
            {
                userid = userId
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getBZPlatformIndexInfo"), "json=" + JsonConvert.SerializeObject(data));
            var ret = JsonConvert.DeserializeObject<ResponesModel>(res);
            if (ret.code != "-1")
            {
                skData = JsonConvert.DeserializeObject<List<KeyValue>>(ret.data.ToString());
                skData.ForEach(x =>
                {
                    x.dataType = 1;
                });
            }
            bzdata.AddRange(skData);
            var depts = new DepartmentBLL().GetSubDepartments(new string[] { deptId });
            var allBZcount = service.FindBZAllCount(codeAndTerminalType, depts.Select(x => x.DepartmentId).ToList());
            bzdata.AddRange(allBZcount);
            return bzdata;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchDeptId">所查询的指标的部门ID 一般为电厂的Id</param>
        /// <param name="userId">当前用户的Id 取数据用</param>
        /// <param name="indexType">指标类型 0管理平台  1安卓终端  2手机APP </param>
        /// <param name="userDeptId">当前用户的所属的部门Id   取数据用</param>
        /// <param name="terminalType">查询类型         0月 1季  2年</param>
        /// <param name="templet">模板 所属的模板  1第一套  2 第二套 以此类推</param>
        /// <returns></returns>
        public List<IndexManageModel> GetIndexData(string searchDeptId, string userId, int indexType, string userDeptId, string terminalType, int? templet)
        {
            TerminalDataSetBLL terminalDataSetBLL = new TerminalDataSetBLL();
            IndexManageBLL indexManageBLL = new IndexManageBLL();

            List<TerminalDataSetEntity> entitys = terminalDataSetBLL.GetList().Where(p => p.IsOpen == 1).ToList(); //先取所有的指标
                                                                                                                   //再取所有的分类
            List<IndexManageEntity> manageEntities = indexManageBLL.GetList(searchDeptId, indexType, null, templet).Where(p => p.IsShow == 1).ToList();
            //获取分类底下的指标
            List<IndexAssocationEntity> indexAssocationEntities = new IIndexAssocationBLL().GetListByTitleId(manageEntities.Select(p => p.Id).ToArray());
            //查询并组装指标值的数据
            var dataSetIds = indexAssocationEntities.Select(x => x.DataSetId);//所有的指标的Ids
            Dictionary<string, string> dic = new Dictionary<string, string>();
            entitys.Where(x => dataSetIds.Contains(x.Id)).ToList().ForEach(x => {
                dic.Add(x.Code + "-" + x.Name, terminalType);
            });//应该要绑定的所有的指标

            List<KeyValue> keyValues = FindAllCount(userId, userDeptId, dic);
            //List<KeyValue> keyValues = FindAllCount(userId, userDeptId, terminalType);
            List<IndexManageModel> models = new List<IndexManageModel>();
            if (manageEntities != null && manageEntities.Count > 0)
            {
                manageEntities.ForEach(title =>
                {
                    //生成分类
                    IndexManageModel model = new IndexManageModel()
                    {
                        TitleId = title.Id,
                        TitleName = title.Title,
                        Srot = title.Sort
                    };
                    //生成指标
                    var datasetIds = indexAssocationEntities.Where(p => p.TitleId == title.Id).Select(p => p.DataSetId);
                    List<TerminalDataSetEntity> terminals = entitys.Where(x => datasetIds.Contains(x.Id)).ToList();
                    List<IndexModel> indexModels = new List<IndexModel>();
                    terminals.ForEach(terminal =>
                    {
                        //组装数据

                        IndexModel indexModel = new IndexModel()
                        {
                            Key = terminal.Code,
                            Name = terminal.Name,
                            Sort = terminal.Sort,
                            IsBZ = terminal.IsBZ,
                            Unit = terminal.Unint,
                            Icon = string.IsNullOrWhiteSpace(terminal.IconUrl) ? null : Config.GetValue("AppUrl") + terminal.IconUrl,
                            Address = terminal.Address
                            //Value = thisKv == null ? "0" : thisKv.value
                        };
                        if (terminal.IsBZ == "1" && keyValues != null && keyValues.Count > 0)
                        {
                            var thisKv = keyValues.FirstOrDefault(p => p.key == terminal.Code);
                            indexModel.Value = thisKv == null ? "0" : thisKv.value;
                        }
                        indexModels.Add(indexModel);
                    });
                    model.AddChilds(indexModels);
                    models.Add(model);
                });
            }
            return models;
        }

        //public List<IndexModel> GetIndexData { get; set; }
    }
    public class ResponesModel
    {
        public string code { get; set; }
        public string info { get; set; }
        public int? count { get; set; }
        public dynamic data { get; set; }
    }


}
