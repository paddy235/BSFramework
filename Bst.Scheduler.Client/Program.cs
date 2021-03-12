using Bst.Fx.IWarning;
using Bst.Fx.Uploading;
using Bst.Fx.Warning;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Scheduler.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0) return;

            Console.WriteLine("定时服务程序正在运行，请不要关闭！");

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(args[0]);

            try
            {
                if (args == null || args.Length == 0) return;

                switch (args[0])
                {
                    case "周工作总结":
                        var build1 = new ReportBuild();
                        build1.Build(args[0]);
                        break;
                    case "月工作总结":
                        var build2 = new ReportBuild();
                        build2.Build(args[0]);
                        break;
                    case "7S推送计划":
                        var sevens = new SevenS();
                        sevens.sendMessage(args[0]);
                        break;
                    case "7S照片计划":
                        var sevens2 = new SevenS();
                        sevens2.StartPicture(args[0]);
                        break;
                    case "绩效管理":
                        var Performance = new Performance();
                        Performance.BaseMessage(args[0]);
                        break;
                    case "绩效管理v2.0":
                        var PerformanceSconed = new PerformanceSecond();
                        PerformanceSconed.BaseMessage(args[0]);
                        break;
                    case "预警执行":
                        IWarningService warning = new WarningService();
                        warning.Execute();
                        break;
                    case "预警消息":
                        IWarningService warning2 = new WarningService();
                        warning2.SendMessage();
                        break;
                    case "班务会":
                        IActivityWarningService service1 = new ActivityWarningService1();
                        service1.Execute();
                        break;
                    case "制度学习":
                        IActivityWarningService service2 = new ActivityWarningService2();
                        service2.Execute();
                        break;
                    case "安全日活动":
                        IActivityWarningService service3 = new ActivityWarningService3();
                        service3.Execute();
                        break;
                    case "节能记录":
                        IActivityWarningService service4 = new ActivityWarningService4();
                        service4.Execute();
                        break;
                    case "上级精神宣贯":
                        IActivityWarningService service5 = new ActivityWarningService5();
                        service5.Execute();
                        break;
                    case "政治学习":
                        IActivityWarningService service6 = new ActivityWarningService6();
                        service6.Execute();
                        break;
                    case "民主管理会":
                        IActivityWarningService service7 = new ActivityWarningService7();
                        service7.Execute();
                        break;
                    case "劳动保护监督":
                        IActivityWarningService service8 = new ActivityWarningService8();
                        service8.Execute();
                        break;
                    case "技术讲课":
                        IEducationWarningService eduservice1 = new EducationWarningService1();
                        eduservice1.Execute(args[0]);
                        break;
                    case "技术问答":
                        IEducationWarningService eduservice2 = new EducationWarningService2();
                        eduservice2.Execute(args[0]);
                        break;
                    case "事故预想":
                        IEducationWarningService eduservice3 = new EducationWarningService3();
                        eduservice3.Execute(args[0]);
                        break;
                    case "反事故演习":
                        IEducationWarningService eduservice4 = new EducationWarningService4();
                        eduservice4.Execute(args[0]);
                        break;
                    case "人身风险预控周评":
                        IEvaluateService evaluateservice = new EvaluateService();
                        evaluateservice.Execute(args[0]);
                        break;
                    case "人身风险预控月评":
                        IEvaluateService evaluateservice2 = new EvaluateService();
                        evaluateservice2.Execute(args[0]);
                        break;
                    case "部门定期任务库":
                        var dpetTask = new DeptCycleTaks();
                        dpetTask.Start(args[0]);
                        break;
                    case "每天未签到记录":
                        var calc1 = new UnSigninCalculator();
                        calc1.CallService("每天");
                        break;
                    case "每周未签到记录":
                        var calc2 = new UnSigninCalculator();
                        calc2.CallService("每周");
                        break;
                    case "每月未签到记录":
                        var calc3 = new UnSigninCalculator();
                        calc3.CallService("每月");
                        break;

                    case "定期排班生成":
                        var workorder = new WorkOrder();
                        workorder.Produce("定期排班生成");
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                logger.Error("计划执行程序异常，{0}", e.Message);
            }
        }
    }
}
