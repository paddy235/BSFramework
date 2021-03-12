using BST.Bzzd.Client.Properties;
using CefSharp;
using CefSharp.WinForms;
using GasForm.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BST.Bzzd.Client
{
    public delegate void CardHandler(byte nCardType, string CardNumber);	//声明委托

    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.StdCall)]
    public delegate void findCardCallback(byte nCardType, string CardNumber);//声明委托
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Gas : Form
    {
        IntPtr pHandle;//设备通讯句柄；
        ICCard cardOp;
        findCardCallback cewp;

        string URL = "";
        Byte[] pName = new Byte[30];
        Byte[] pSex = new Byte[3];
        Byte[] pIDNumber = new Byte[30];
        int ret = 0;
        private HotKey _hk;//热键
        public Gas()
        {
            #region 初始化
            InitializeComponent();
            cardOp = new ICCard();
            cardOp.FindCardEvent += cardOp_FindCardEvent;
            cewp = new findCardCallback(cardOp.findCard);
            //IntPtr tempHandle;
            //tempHandle = ICCard.device_open(0, 9600);
            //if ((int)tempHandle > 0)
            //{
            //    pHandle = tempHandle;

            //}
            //传递事件接口
            //ICCard.getInterface(cewp);

            #endregion

            #region 热键启动
            //_hk = new HotKey(this, 100);
            //_hk.HotKeyDown += new Action<object>(hk_HotKeyDown);
            //_hk.Error += new Action<object, object>(hk_Error);
            //_hk.Register(HotKey.Modifiers.Ctrl | HotKey.Modifiers.Alt, Keys.A);
            #endregion
        }

        /// <summary>
        /// 弹出错误提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hk_Error(object sender, object e)
        {
            MessageBox.Show(e.ToString());
        }

        void cardOp_FindCardEvent(byte nCardType, string CardNumber)
        {
            if (1 == 1)//身份证
            {
                StringBuilder sFilePath = new StringBuilder("C:\\photo");
                Byte[] pMessage = new Byte[100];
                int ret = ICCard.IDCard_ReadCard_ExTwo(pHandle, true, sFilePath, pMessage);
                if (ret == 0)
                {
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 0, pName);
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 1, pSex);
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 5, pIDNumber);
                }
            }
        }

        /// <summary>
        /// 获取身份证信息
        /// </summary>
        /// <returns></returns>
        public string ReadCard()
        {
            string res = "1";
            try
            {
                #region 初始化
                //InitializeComponent();
                cardOp = new ICCard();
                cardOp.FindCardEvent += cardOp_FindCardEvent;
                cewp = new findCardCallback(cardOp.findCard);
                IntPtr tempHandle;
                tempHandle = ICCard.device_open(0, 9600);
                if ((int)tempHandle > 0)
                {
                    pHandle = tempHandle;
                }

                //传递事件接口
                ICCard.getInterface(cewp);

                //StringBuilder sFilePath = new StringBuilder("C:\\photo");
                DirectoryInfo rootDir = Directory.GetParent(Environment.CurrentDirectory);
                string root = rootDir.Parent.FullName;
                StringBuilder sFilePath = new StringBuilder(root + @"\CardImg");

                Byte[] pMessage = new Byte[100];
                int ret = ICCard.IDCard_ReadCard_ExTwo(pHandle, true, sFilePath, pMessage);
                if (ret == 0)
                {
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 0, pName);
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 1, pSex);
                    ret = ICCard.IDCard_GetCardInfo(pHandle, 5, pIDNumber);
                }

                #endregion

                string name = System.Text.Encoding.Default.GetString(pName).Replace("\0", "").Trim();
                if (ret == 0 && !string.IsNullOrEmpty(name))
                {
                    res = System.Text.Encoding.Default.GetString(pName).Replace("\0", "").Trim() + ',';
                    res += System.Text.Encoding.Default.GetString(pSex).Replace("\0", "").Trim() + ',';
                    //res += System.Text.Encoding.Default.GetString(pIDNumber).Replace("\0", "").Trim();
                    string IDNumber = System.Text.Encoding.Default.GetString(pIDNumber).Replace("\0", "").Trim();
                    res += IDNumber;
                    #region 获取身份证照片
                    string path = @"" + sFilePath.ToString() + @"\zp.bmp";//源文件
                    string paths = root + @"\CardImgOK";
                    if (!Directory.Exists(paths))
                    {
                        Directory.CreateDirectory(paths);
                    }
                    string path2 = paths + @"\" + IDNumber + ".bmp";//新文件
                    FileInfo fi1 = new FileInfo(path);
                    FileInfo fi2 = new FileInfo(path2);
                    fi1.CopyTo(path2, true);
                    #endregion
                }
                ICCard.device_close(pHandle);
                pName = new Byte[30];
                pSex = new Byte[3];
                pIDNumber = new Byte[30];
                return res;
            }
            catch (Exception er)
            {
                return er.Message;
            }
        }

        /// <summary>
        /// 写日志(初始化加油站信息)
        /// </summary>
        /// <param name="msg"></param>
        public string WriteLogText(string msg)
        {
            string res = "1";
            string logpath = Program.GetAppSetting("LogPath");
            string name = "加油站";
            try
            {
                if (!Directory.Exists(logpath))
                {
                    Directory.CreateDirectory(logpath);
                }
                object obj = new object();
                lock (obj)
                {
                    using (FileStream file = new FileStream(logpath + name + ".txt", FileMode.Create))
                    {
                        StreamWriter swr = new StreamWriter(file);
                        swr.WriteLine(msg);
                        swr.Close();
                        swr.Dispose();
                        file.Close();
                        file.Dispose();
                    }
                }
                res = "0";
            }
            catch { res = "1"; }
            return res;
        }

        /// <summary>
        /// 读取初始化文件中的部门id  如果没有则返回空
        /// </summary>
        /// <returns></returns>
        public string ReadLog()
        {
            string logpath = Program.GetAppSetting("LogPath");
            string name = "加油站";
            string url = logpath + name + ".txt";
            try
            {
                if (!File.Exists(url))
                {
                    return "";
                }
                using (StreamReader sr = new StreamReader(url, Encoding.Default))
                {
                    string depid = "";
                    depid = sr.ReadLine();
                    return depid;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }

        public void Test(string url)
        {
            URL = url;
            // System.Diagnostics.Process.Start("chrome.exe", "http://localhost/BSFramework/DayCheckManage/EmergencyPlan/FormZD?recid=a0953785-f514-4a17-9697-6eb48958283a&backUrl=%2FBSFramework%2FDayCheckManage%2FEmergencyPlan%2FIndexZD&name=55555555555&orderNum=3&fileUrl=55555555555_20180529192534.docx&topUrl=/BSFramework&deptid=12547757-c03d-42e0-a612-a2bf2caee691");
            Thread thd = new Thread(new ParameterizedThreadStart(Show));
            thd.SetApartmentState(ApartmentState.STA);
            thd.IsBackground = true;
            thd.Start();
            thd.Join();

        }

        public void Mbox()
        {
            MessageBox.Show("测试调用后台方法");
        }

        private void Show(object obj)
        {
            //Form2 fr = new Form2(URL);
            //fr.ShowDialog();
        }
        public void ShowInput()
        {
            TabTip.ShowInputPanel();
        }

        public void CloseInput()
        {
            TabTip.HideInputPanel();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gas_Load(object sender, EventArgs e)
        {

            //webBrowser.IsWebBrowserContextMenuEnabled = false;
            //webBrowser.WebBrowserShortcutsEnabled = false;
            //webBrowser.ScriptErrorsSuppressed = true;
            //string depid = ReadLog();
            string url = "";
            //if (depid == "")
            //{
            //    url = Program.GetAppSetting("WebIn", "0");
            //}
            //else
            //{
                url = Program.GetAppSetting("WebUrl", "0");
            //}

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            ChromiumWebBrowser browser = new ChromiumWebBrowser(url);
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            //定义Web调用属性
            BindingOptions bo = new BindingOptions();
            bo.CamelCaseJavascriptNames = false;
            browser.RegisterJsObject("JsObj", new Gas(), bo);
            //browser.RegisterJsObject("TestObj", new Gas(), bo);
            //键盘、任务栏禁用
            HookHelper.Hook_Start();
            HookHelper.TaskMgrLocking(true);

            #region 注释代码
            var set = new Settings();

            //CefSettings set = new CefSettings() {
            //    CachePath = Directory.GetCurrentDirectory() + @"\Cache"
            //};
            //set.PersistSessionCookies = true;
            //set.CefCommandLineArgs.Add("ppapi-flash-path", @"\pepflashplayer.dll");
            //if (!Cef.IsInitialized)
            //{
            //    Cef.Initialize(set);
            //}
            //Uri ul = new Uri(url, UriKind.RelativeOrAbsolute);
            //if (webBrowser.Url == null)
            //{
            //    //this.webBrowser.Url = ul;
            //    ChromiumWebBrowser browser = new ChromiumWebBrowser(url);
            //    this.Controls.Add(browser);
            //    browser.Dock = DockStyle.Fill;
            //}
            //else
            //{
            //    this.webBrowser.Navigate(ul);
            //}
            #endregion
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gas_FormClosed(object sender, FormClosedEventArgs e)
        {
            //键盘、任务栏启用
            HookHelper.Hook_Clear();
            HookHelper.TaskMgrLocking(false);
            //_hk.UnRegister();
        }

        /// <summary>
        /// 获取读取身份证Dll文件
        /// </summary>
        public class ICCard
        {
            public event CardHandler FindCardEvent;//声明事件
            [DllImport("mtx_32.dll")]
            public static extern IntPtr device_open(Int16 port, int baud);//建立连接
            [DllImport("mtx_32.dll")]
            public static extern Int16 device_close(IntPtr icdev);//断开连接
            [DllImport("mtx_32.dll", CharSet = CharSet.Ansi)]
            public static extern int IDCard_ReadCard_ExTwo(
                                         IntPtr icdev,
                                         bool bIsNeedBmpFile,
                                         StringBuilder sFilePath,
                                         Byte[] message);
            [DllImport("mtx_32.dll", CharSet = CharSet.Ansi)]
            public static extern int IDCard_GetCardInfo(IntPtr icdev, int index, byte[] infodata);

            [DllImport("mtx_32.dll")]
            public static extern void getInterface(findCardCallback fCardFunc);//检测卡回调

            //触发事件 
            public void findCard(byte nCardType, string IDNumber)
            {
                if (FindCardEvent != null)
                {	//如果有对象注册
                    FindCardEvent(nCardType, IDNumber);	//调用所有注册对象的方法
                }
            }
        }

        /// <summary>
        /// 调用系统浏览器打开网页
        /// </summary>
        /// <param name="url"></param>
        public void OpenBrowserUrl(string url)
        {
            try
            {
                var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
                if (IntPtr.Size == 4)
                {
                    // 32位注册表路径
                    openKey = @"SOFTWARE\Google\Chrome";
                }
                RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
                // 谷歌浏览器就用谷歌打开，没找到就用系统默认的浏览器
                // 谷歌卸载了，注册表还没有清空，程序会返回一个"系统找不到指定的文件。"的bug
                if (appPath != null)
                {
                    var result = Process.Start("chrome.exe", url);
                    if (result == null)
                    {
                        MessageBox.Show("请安装谷歌浏览器！");
                    }
                }
                else
                {
                    var result = Process.Start("chrome.exe", url);
                    if (result == null)
                    {
                        MessageBox.Show("请安装谷歌浏览器！");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请安装谷歌浏览器！");
            }

        }
    }
}
