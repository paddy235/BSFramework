using Bst.Fx.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Fx.SchedulerEntities;
using Bst.Fx.IScheduler;
using TaskScheduler;
using Bst.Bzzd.DataSource;
using System.Configuration;
using System.Threading;

namespace Bst.Fx.Scheduler
{
    public class SchedulerTask : ISchedulerTask
    {
        private ITaskService ts;
        private string path;
        private ITaskFolder folder;
        private string client;

        public SchedulerTask()
        {
            ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);

            path = (ConfigurationManager.AppSettings["scheduler"] ?? @"\bossien\bzzd").ToString();
            folder = ts.GetFolder(path);
            client = ConfigurationManager.AppSettings["SchedulerPath"] ?? @"D:\Workspaces\bst\ElectricSafety\班组智能终端\Bst.Scheduler.Client\bin\Debug\Bst.Scheduler.Client.exe";
        }

        public void Execute(string name)
        {
            var task = this.FindTask(name);
            if (task == null) throw new Exception("未找到执行计划");

            task.Run(null);
        }

        public virtual List<SchedulerTaskEntity> GetTasks()
        {
            var result = new List<SchedulerTaskEntity>();

            var path = (ConfigurationManager.AppSettings["scheduler"] ?? @"\bossien\bzzd").ToString();
            var folder = ts.GetFolder(path);
            var tasks = folder.GetTasks(1);
            foreach (IRegisteredTask item in tasks)
            {
                result.Add(new SchedulerTaskEntity() { TaskName = item.Name, Status = ToStateString(item.State), LastRunTime = item.LastRunTime, NextRunTime = item.NextRunTime, LastResult = item.LastTaskResult.ToString(), Enabled = item.Enabled });
            }

            return result;
        }

        public SchedulerTaskEntity NewTask(SchedulerTaskEntity entity)
        {
            var folder = this.EnsureFolder();

            //var task = ts.NewTask(0);
            //task.RegistrationInfo = new RegistrationInfo();


            return entity;
        }

        private ITaskFolder EnsureFolder()
        {
            var root = ts.GetFolder("\\");
            var subFolders = root.GetFolders(0);
            ITaskFolder bossien = null;
            foreach (ITaskFolder item in subFolders)
            {
                if (item.Name == "bossien")
                {
                    bossien = item;
                    break;
                }
            }

            if (bossien == null)
                bossien = root.CreateFolder("bossien");

            ITaskFolder bzzd = null;
            subFolders = bossien.GetFolders(0);
            foreach (ITaskFolder item in subFolders)
            {
                if (item.Name == "bzzd")
                {
                    bzzd = item;
                    break;
                }
            }

            if (bzzd == null)
                bzzd = bossien.CreateFolder("bzzd");

            return bzzd;
        }

        private string ToStateString(_TASK_STATE state)
        {
            var status = "禁用";
            switch (state)
            {
                case _TASK_STATE.TASK_STATE_UNKNOWN:
                    status = "未知";
                    break;
                case _TASK_STATE.TASK_STATE_DISABLED:
                    status = "禁用";
                    break;
                case _TASK_STATE.TASK_STATE_QUEUED:
                    status = "排队中";
                    break;
                case _TASK_STATE.TASK_STATE_READY:
                    status = "准备就绪";
                    break;
                case _TASK_STATE.TASK_STATE_RUNNING:
                    status = "执行中";
                    break;
                default:
                    break;
            }
            return status;
        }

        private IRegisteredTask FindTask(string name)
        {
            var path = (ConfigurationManager.AppSettings["scheduler"] ?? @"\bossien\bzzd").ToString();
            var folder = ts.GetFolder(path);
            var task = folder.GetTask(name);
            return task;
        }

        public void Enable(SchedulerTaskEntity task)
        {
            var scheduler = this.FindTask(task.TaskName);
            scheduler.Enabled = task.Enabled;
        }

        public void Disable(SchedulerTaskEntity task)
        {
            var scheduler = this.FindTask(task.TaskName);
            scheduler.Enabled = task.Enabled;
        }

        public SchedulerTaskEntity GetDetail(string id)
        {
            var path = (ConfigurationManager.AppSettings["scheduler"] ?? @"\bossien\bzzd").ToString();
            var folder = ts.GetFolder(path);

            var task = folder.GetTask(id);
            var data = new SchedulerTaskEntity() { TaskName = task.Name };
            var trigger = task.Definition.Triggers[1];
            data.ExecuteTime = DateTime.ParseExact(trigger.StartBoundary, "yyyy-MM-ddTHH:mm:ss", Thread.CurrentThread.CurrentCulture);
            if (trigger is IDailyTrigger)
            {
                var trg = trigger as IDailyTrigger;
                data.Category = "天";
            }
            else if (trigger is IWeeklyTrigger)
            {
                var trg = trigger as IWeeklyTrigger;
                data.Category = "周";
                data.DayOfWeek = Calc2(trg.DaysOfWeek);
            }
            else if (trigger is IMonthlyTrigger)
            {
                var trg = trigger as IMonthlyTrigger;
                data.Category = "月";
                data.DaysOfMonth = Calc2(trg.DaysOfMonth);
                data.MonthsOfYear = Calc2(trg.MonthsOfYear);
            }

            return data;
        }

        public void Edit(SchedulerTaskEntity model)
        {
            var def = ts.NewTask(0);
            var username = ConfigurationManager.AppSettings["UserName"].ToString();
            var password = ConfigurationManager.AppSettings["Pasword"].ToString();

            switch (model.Category)
            {
                case "天":
                    var daily = def.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_DAILY) as IDailyTrigger;
                    daily.StartBoundary = model.ExecuteTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    break;
                case "周":
                    var weekly = def.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_WEEKLY) as IWeeklyTrigger;
                    weekly.StartBoundary = model.ExecuteTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    weekly.DaysOfWeek = (short)Calc(model.DayOfWeek);
                    break;
                case "月":
                    var monthly = def.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_MONTHLY) as IMonthlyTrigger;
                    monthly.StartBoundary = model.ExecuteTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    monthly.DaysOfMonth = Calc(model.DaysOfMonth);
                    monthly.MonthsOfYear = (short)Calc(model.MonthsOfYear);
                    break;
                default:
                    break;
            }

            var action = def.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC) as IExecAction;
            action.Path = client;
            action.Arguments = model.TaskName;

            var ss = folder.RegisterTaskDefinition(model.TaskName, def, (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE, username, password, _TASK_LOGON_TYPE.TASK_LOGON_PASSWORD, "");
        }

        private int Calc(int[] args)
        {
            var val = 0;
            foreach (var item in args)
            {
                val += (int)Math.Pow(2, item);
            }
            return val;
        }

        public int[] Calc2(int val)
        {
            var items = new List<int>();
            val = Calc3(val, items);
            while (val > 0)
            {
                val = Calc3(val, items);
            }
            items.Reverse();
            return items.ToArray();
        }

        public int Calc3(int val, List<int> items)
        {
            var i = 0;
            while (Math.Pow(2, i) <= val)
            {
                i++;
            }
            items.Add(i - 1);
            return val - (int)Math.Pow(2, i - 1);
        }

        private bool Exist(string name)
        {
            var task = folder.GetTask(name);
            return task != null;
        }
    }
}
