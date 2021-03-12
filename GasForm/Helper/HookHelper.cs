using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace GasForm.Helper
{

    public class HookHelper
    {
        #region 屏蔽Windows功能键（快捷键）
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        private static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;

        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，会在你截取之前获得键盘。 
        private static HookProc KeyBoardHookProcedure;
        //键盘Hook结构函数 
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //设置钩子 
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //抽掉钩子 
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        //调用下一个钩子 
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// 启动钩子，处理钩子事件(禁用Windows快捷键)
        /// </summary>
        public static void Hook_Start()
        {
            // 安装键盘钩子 
            if (hHook == 0)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);
                hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardHookProcedure,
                        GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //如果设置钩子失败. 
                if (hHook == 0)
                {
                    Hook_Clear();
                }
            }
        }

        /// <summary>
        /// 取消钩子事件，处理钩子事件(取消禁用Windows快捷键) 
        /// </summary>
        public static void Hook_Clear()
        {
            bool retKeyboard = true;
            if (hHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
            //如果去掉钩子失败. 
            //if (!retKeyboard) throw new Exception("Hook去除失败");
        }

        //这里可以添加自己想要的信息处理 
        private static int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                if (kbh.vkCode == 91) // 截获左win(开始菜单键) 
                {
                    return 1;
                }

                if (kbh.vkCode == 92)// 截获右win(开始菜单键) 
                {
                    return 1;
                }

                if (kbh.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control) //截获Ctrl+Esc 
                {
                    return 1;
                }

                if (kbh.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Alt) //截获Alt+Esc 
                {
                    return 1;
                }
                if (kbh.vkCode == (int)Keys.F4 && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+f4 
                {
                    return 1;
                }

                if (kbh.vkCode == (int)Keys.Tab && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+tab
                {
                    return 1;
                }

                if (kbh.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Shift) //截获Ctrl+Shift+Esc
                {
                    return 1;
                }
                if (kbh.vkCode == (int)Keys.Space && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+空格 
                {
                    return 1;
                }

                if (kbh.vkCode == 241) //截获F1 
                {
                    return 1;
                }

                if ((int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt + (int)Keys.Delete)      //截获Ctrl+Alt+Delete 
                {
                    return 1;
                }

                //if ((int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Shift) //截获Ctrl+Shift 
                //{
                //    return 1;
                //}

                if (kbh.vkCode == (int)Keys.Space && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt) //截获Ctrl+Alt+空格 
                {
                    return 1;
                }

                if (kbh.vkCode == 77 && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt + (int)Keys.Shift)      //截获M+Ctrl+Alt+Shift+M
                {
                    Hook_Clear();// (取消钩子)
                    TaskMgrLocking(false);
                }
            }

            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 是否禁用Windows任务管理器
        /// </summary>
        /// <param name="bLock">True:禁用；flase:启用</param>
        public static void TaskMgrLocking(bool bLock)
        {
            if (bLock)//屏蔽任务管理器、并且不出现windows提示信息“任务管理器已被管理员禁用”
            {
                Process p = new Process();
                p.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                p.StartInfo.FileName = "taskmgr.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
            }
            else//设置任务管理器为可启动状态 
            {
                Process[] p = Process.GetProcesses();

                foreach (Process p1 in p)
                {
                    try
                    {
                        if (p1.ProcessName.ToLower().Trim() == "taskmgr")//这里判断是任务管理器    
                        {
                            p1.Kill();
                            RegistryKey r = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                            r.SetValue("DisableTaskmgr", "0"); //设置任务管理器为可启动状态 
                            Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");

                        }
                    }
                    catch
                    {
                        return;
                    }
                }

            }

        }

        public static void Cutter()
        {
            //Process[] p = Process.GetProcesses();
            //bool 
            //foreach (Process p1 in p)
            //{
            //    try
            //    {
            //        if (p1.ProcessName.ToLower().Trim() == "ScreenCutter")//这里判断是任务管理器    
            //        {
            //            p1.Kill();
            //            RegistryKey r = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            //            r.SetValue("DisableTaskmgr", "0"); //设置任务管理器为可启动状态 
            //            Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");

            //        }
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
            string mainApp = Application.StartupPath + "\\ScreenCutter.exe";
            Process.Start(mainApp);
        }


        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）           
            uint fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );

        #endregion

    }

    /// <summary>
    /// 系统热键注册
    /// </summary>
    public class HotKey
    {
        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID（不能与其它ID重复）</param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="vk">定义热键的内容</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, Modifiers fsModifiers, Keys vk);

        /// <summary>
        /// 取消热键
        /// </summary>
        /// <param name="hWnd">要取消热键的窗口的句柄</param>
        /// <param name="id">要取消热键的ID</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 功能键组合
        /// </summary>
        [Flags]
        public enum Modifiers
        {
            None = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public event Action<object> HotKeyDown;

        public event Action<object, object> Error;

        public const uint WindowModifiterHotKey = 0x312;

        /// <summary>
        /// 热键注册ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 注册窗体
        /// </summary>
        public Form Form { get; private set; }

        /// <summary>
        /// 注册或取消注册的状态
        /// </summary>
        public bool State { get; private set; }

        /// <summary>
        /// 是否注册
        /// </summary>
        public bool IsRegister { get; private set; }

        /// <summary>
        /// 为窗体注册系统热键
        /// </summary>
        /// <param name="form">热键注册窗体</param>
        /// <param name="id">热键标识</param>
        public HotKey(Form form, int id)
        {
            Form = form;
            ID = id;
        }

        /// <summary>
        /// 为窗体注册系统热键
        /// </summary>
        /// <param name="form">热键注册窗体</param>
        public HotKey(Form form)
            : this(form, form.Handle.GetHashCode())
        {
        }

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="fs">功能键组合</param>
        /// <param name="key">普通键</param>
        /// <returns></returns>
        public bool Register(Modifiers fs, Keys key)
        {
            Modifiers fsModifiers = Modifiers.None;

            if ((fs & Modifiers.Alt) == Modifiers.Alt)
            {
                fsModifiers |= Modifiers.Alt;
            }
            if ((fs & Modifiers.Ctrl) == Modifiers.Ctrl)
            {
                fsModifiers |= Modifiers.Ctrl;
            }
            if ((fs & Modifiers.Shift) == Modifiers.Shift)
            {
                fsModifiers |= Modifiers.Shift;
            }
            if ((fs & Modifiers.Win) == Modifiers.Win)
            {
                fsModifiers |= Modifiers.Win;
            }

            State = RegisterHotKey(Form.Handle, ID, fsModifiers, key);

            if (!State)
            {
                OnError(Marshal.GetLastWin32Error());
            }

            IsRegister = State;

            return State;
        }

        /// <summary>
        /// 取消热键
        /// </summary>
        /// <returns></returns>
        public bool UnRegister()
        {
            if (!IsRegister)
            {
                //OnError(-1);
                return false;
            }

            State = UnregisterHotKey(Form.Handle, ID);

            if (!State)
            {
                OnError(Marshal.GetLastWin32Error());
            }

            IsRegister = !State;

            return State;
        }

        public void KeyDown(Message m)
        {
            if (m.Msg == WindowModifiterHotKey)
            {
                if ((int)m.WParam == ID)
                {
                    OnKeyDown();
                }
            }
        }

        private void OnKeyDown()
        {
            if (HotKeyDown != null)
            {
                HotKeyDown(this);
            }
        }

        public void OnError(int code)
        {
            if (Error != null)
            {
                Error(this, GetErrorMessage(code));
            }
        }

        private static string GetErrorMessage(int code)
        {
            string error = "";
            switch (code)
            {
                case -1:
                    error = "没有注册热键";
                    break;
                case 1409:
                    error = "注册热键冲突";
                    break;
                default:
                    break;
            }
            return error;
        }
    }
}
