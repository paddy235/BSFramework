using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace Bst.Bzzd.Sync
{
    public class VideoUpload
    {
        private TaskSchedulerClass ts;

        public VideoUpload()
        {
            ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
        }

        public void Upload(string id)
        {
            var task = this.EnsureTask(id);

            //task.Run(null);

            //this.DeleteTask();

            //var folder = ts.GetFolder("\\");
            //var t1 = folder.GetTask("xxx");
            //var t2 = folder.GetTask("yyy");
        }

        private void DeleteTask()
        {
            var folder = ts.GetFolder("\\bossien\\bzzd");
            folder.DeleteTask("upload", 0);
            folder = ts.GetFolder("\\bossien");
            folder.DeleteFolder("bzzd", 0);
            folder = ts.GetFolder("\\");
            folder.DeleteFolder("bossien", 0);
        }

        private IRegisteredTask EnsureTask(string id)
        {
            var folder = this.EnsureFolder();
            var tasks = folder.GetTasks(0);
            IRegisteredTask task = null;
            foreach (IRegisteredTask item in tasks)
            {
                if (item.Name == "upload")
                {
                    task = item;
                    break;
                }
            }
            if (task != null)
                folder.DeleteTask("upload", 0);

            var register = ts.NewTask(0);
            register.RegistrationInfo.Author = "hutao";
            register.RegistrationInfo.Description = "这是一个定时任务";



            var trigger = register.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
            trigger.StartBoundary = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            var programpath = ConfigurationManager.AppSettings["programpath"].ToString();

            var action = (IExecAction)register.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = programpath;
            action.Arguments = id;

            register.Settings.ExecutionTimeLimit = "PT0S";
            register.Settings.DisallowStartIfOnBatteries = true;
            register.Settings.RunOnlyIfIdle = false;
            var t = folder.RegisterTaskDefinition("upload", register, (int)_TASK_CREATION.TASK_CREATE, "hutao", null, _TASK_LOGON_TYPE.TASK_LOGON_S4U, null);
            return t;
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

    }
}
