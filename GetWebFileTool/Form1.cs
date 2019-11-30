using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace GetWebSiteTool
{
    public partial class Form1 : Form
    {
        const int DOWNFILE_SUCCESS = 0;
        const int DOWNFILE_ERROR = -1;
        const int DOWNFILE_EXISTS = -2;
        const int DOWNFILE_WRITERR = -3;

        //获取网址部分，正则匹配组索引
        const int URL_PART_ALL = 0;     //全部网址
        const int URL_PART_DOMAIN = 1;  //域名，如：http://domain.com/ 或 //domain.com/
        const int URL_PART_PROTOCOL = 2;//协议，如：http:、https、（空串）
        const int URL_PART_PATH = 3;    //相对路径（含文件名）
        const int URL_PART_PARAM = 4;     //网址参数部分，如：?abc=123
        const int URL_PART_EXT = 5;     //文件后缀（不带.号）


        static string localPath = "";  //本地路径
        static string domainName = ""; //网址域名
        static int taskTotal = 1; //总共任务数
        static int taskDone = 0;  //完成任务数
        static Queue<String> urls = new Queue<String>(); // 任务队列(url)
        static Object locker = new Object(); //线程锁
        static DateTime startTime;
        static string cur_config_file = ""; // 当前使用配置文件
        static bool isAddParamName = false;

        SaveWebFileDelegate SaveWebFileMethod = null;
        StringBuilder statusLogBuf = new StringBuilder(); // 状态日志缓存区

        /// <summary>
        /// 配置文件命名
        /// </summary>
        private const string CONFIG_FILE = "Config.ini";
        /// <summary>
        /// 配置结点名
        /// </summary>
        private const string INI_SESSION = "DS_GetWebFileTool";

        public Form1()
        {
            InitializeComponent();

            cur_config_file = AppDomain.CurrentDomain.BaseDirectory + CONFIG_FILE;
            toolTip1.SetToolTip(lnkLabel, cur_config_file);

            //读取配置文件
            ReadConfig(cur_config_file);

            if (txtFolder.Text.Trim().Length <= 0)
            {
                txtFolder.Text = AppDomain.CurrentDomain.BaseDirectory + "download\\";
            }

            SaveWebFileMethod = new SaveWebFileDelegate(SaveWebFile);

            brwStatus.DocumentText = "";
            brwStatus.Hide();
            txtStatus.Text = "";
            txtStatus.Show();
        }

        //保存目录
        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFolderDlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            DialogResult dlgResult = openFolderDlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                txtFolder.Text = openFolderDlg.SelectedPath;
            }
        }

        //清空日志
        private void txtClear_Click(object sender, EventArgs e)
        {
            statusLogBuf.Length = 0;
            txtStatus.Text = "";
            brwStatus.DocumentText = "";
        }

        //分析下载
        private void btnGet_Click(object sender, EventArgs e)
        {
            int threadNum = 1; // 默认线程数
            int threadNo = 1; // 当前线程序号
            if (txtURL.Text.Trim() == "")
            {
                MessageBox.Show("请输入分析网址！");
                txtURL.Focus();
                return;
            }

            if (txtURL.Text.IndexOf("(*)") > 0)
            {
                if (txtNum1.Text.Trim() == "")
                {
                    MessageBox.Show("请输入开始序号！");
                    txtNum1.Focus();
                    return;
                }

                if (txtNum2.Text.Trim() == "")
                {
                    MessageBox.Show("请输入结束序号！");
                    txtNum2.Focus();
                    return;
                }

                if (txtLen.Text.Trim() == "")
                {
                    MessageBox.Show("请输入通配符长度！");
                    txtLen.Focus();
                    return;
                }

                if (txtThreadNum.Text.Trim() == "")
                {
                    MessageBox.Show("请输入开启的线程数！");
                    txtThreadNum.Focus();
                    return;
                }
                threadNum = int.Parse(txtThreadNum.Text);

                if (threadNum <= 0)
                {
                    MessageBox.Show("开启的线程数应大于0！");
                    txtThreadNum.Focus();
                    return;
                }
            }


            if (txtFolder.Text.Trim() == "")
            {
                MessageBox.Show("请选择操作目录！");
                txtFolder.Focus();
                return;
            }

            if (txtRegEx2.Text.Trim().Length > 0)
            {
                if (txtRegEx.Text.Trim().Length <= 0 || chkGather.Checked == false)
                {
                    MessageBox.Show("请输入第一层正规表达式，并勾选抓取数据！");
                    return;
                }
            }
            if (txtRegEx.Text.Trim().Length <= 0 && chkGather.Checked)
            {
                MessageBox.Show("请输入第一层正规表达式！");
                return;
            }
            if (txtRegEx2.Text.Trim().Length <= 0 && chkGather2.Checked)
            {
                MessageBox.Show("请输入第二层正规表达式！");
                return;
            }

            if (txtRegEx.Text.Trim().Length > 0 || txtRegEx2.Text.Trim().Length > 0)
            {
                if (txtMaxCount.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("请输入匹配上限值（0表示不限）！");
                    txtMaxCount.Focus();
                    return;
                }
            }

            //URL地址
            string url = txtURL.Text.Trim();

            // 如果是网址只给出域名，则补全最后的斜杠
            if (url.Replace("//", "").IndexOf("/") < 0)
            {
                url += "/";
                txtURL.Text = url;
            }

            //网址域名，如：http://moretuan.net/
            domainName = getURLPart(url, URL_PART_DOMAIN);

            //本地路径，如：D:\\Demo\
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";

            //是否追加网址参数到文件名
            isAddParamName = chkAddParamName.Checked;

            //任务数
            taskTotal = 1;
            //已完成
            taskDone = 0;

            startTime = DateTime.Now;
            logStatus("开始任务...\r\n");
            string strGather = chkGather.Checked ? "true" : "false";
            int level = 1; //迭代层级

            if (url.IndexOf("(*)") > 0)
            {
                int len = int.Parse(txtLen.Text);
                int num1 = int.Parse(txtNum1.Text);
                int num2 = int.Parse(txtNum2.Text);
                if (num1 > num2)
                {
                    MessageBox.Show("开始序号应小于等于结束序号！");
                    txtNum2.Focus();
                    return;
                }

                // 比线程数多出的任务，先放入任务队列
                urls.Clear();
                taskTotal = 0;
                for (int i = num1; i <= num2; i++)
                {
                    taskTotal++;
                    if (taskTotal <= threadNum)
                    {
                        continue;
                    }
                    string num = AddZero(i.ToString(), len);
                    string url2 = url.Replace("(*)", num);
                    urls.Enqueue(url2);
                }
                int topNum = Math.Min(taskTotal, threadNum);
                for (int i = num1; i <= num2; i++)
                {
                    if (topNum <= 0)
                    {
                        break;
                    }
                    topNum--;
                    string num = AddZero(i.ToString(), len);
                    string url2 = url.Replace("(*)", num);

                    string filename = getURLPart(url2, URL_PART_PATH).Replace('/', '\\');
                    string savePath = localPath + filename;

                    logStatus("下载线程" + threadNo + "：" + filename + "\r\n");
                    SaveWebFileMethod.BeginInvoke(url2, savePath, new AsyncCallback(DownFinished), new string[] { url2, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, (threadNo++).ToString(), level.ToString() });
                }
            }
            else
            {
                string filename = getURLPart(url, URL_PART_PATH).Replace('/', '\\');
                string savePath = localPath + filename;

                logStatus("下载线程" + threadNo + "：" + filename + "\r\n");
                SaveWebFileMethod.BeginInvoke(url, savePath, new AsyncCallback(DownFinished), new string[] { url, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, threadNo.ToString(), level.ToString() });
            }
        }


        //完成下载（非阻塞调用）
        private void DownFinished(IAsyncResult result)
        {
            int code = SaveWebFileMethod.EndInvoke(result);
            DownFinished_End(result, code);
        }

        //完成下载（阻塞调用）
        private void DownFinished_End(IAsyncResult result, int code)
        {
            string status = getDownFileStatus(code);

            string[] strArry = result.AsyncState as string[];
            string url = strArry[0];
            string regEx = strArry[1]; //正则表达式
            int grpId = int.Parse(strArry[2]); //匹配组序号
            bool isGather = bool.Parse(strArry[3]); //是否抓取文件中数据
            int threadNo = int.Parse(strArry[4]); //线程序号
            int level = int.Parse(strArry[5]); //迭代层级

            //string url = result.AsyncState.ToString();
            //string regEx = txtRegEx.Text.Trim();
            string filename = getURLPart(url, URL_PART_PATH).Replace('/', '\\');

            if (level == 1)
            {
                lock (locker)
                {
                    taskDone++;
                    logStatus("完成线程" + threadNo + "：" + filename + " (" + status + ")\r\n");
                    logStatus("任务进度：" + taskDone + "/" + taskTotal + "\r\n");
                }
            }
            else
            {
                logStatus("完成下载：" + filename + " (" + status + ")\r\n");
            }

            string extFile = getURLPart(filename, URL_PART_EXT);
            // bool isTextFile = ("txt|xml|htm|html|shtml|asp|aspx|php|jsp|js|css".IndexOf(extFile) >= 0);

            //获取文件(通过regEx分析)，最多只分析到level=2层
            if (regEx != "" && (code == DOWNFILE_SUCCESS || code == DOWNFILE_EXISTS) && level < 3 /* && isTextFile*/)
            {
                logStatus("开始正则分析：" + filename + " ∧∧∧∧∧\r\n");
                string urlPath = url.Substring(0, url.LastIndexOf("/") + 1);
                getFileByRegex(filename, urlPath, regEx, grpId, isGather, threadNo, level);
                logStatus("完成正则分析：" + filename + " ∨∨∨∨∨\r\n\r\n");
            }

            // 检查第一层（多线程）执行情况
            if (level == 1)
            {
                lock (locker)
                {
                    if (taskDone >= taskTotal)
                    {
                        double useTime = (DateTime.Now - startTime).TotalMilliseconds;
                        string strUseTime = getUseTime(Convert.ToInt64(useTime));
                        logStatus("已完成，任务总数：" + taskTotal + "，耗时：" + strUseTime + "\r\n");
                    }
                }

                if (urls.Count > 0)
                {
                    string url2 = urls.Dequeue();
                    string filename2 = getURLPart(url2, URL_PART_PATH).Replace('/', '\\');
                    string savePath2 = localPath + filename2;
                    string strGather = chkGather.Checked ? "true" : "false";

                    logStatus("下载线程" + threadNo + "：" + filename2 + "\r\n");
                    SaveWebFileMethod.BeginInvoke(url2, savePath2, new AsyncCallback(DownFinished), new string[] { url2, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, threadNo.ToString(), "1" });
                }
            }
        }

        /// <summary>
        /// 获取文件(通过正则表达式分析)
        /// </summary>
        /// <param name="filename">本地文件名</param>
        /// <param name="urlPath">网络路径</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="grpId">匹配组id</param>
        /// <param name="isGether">是否抓取文件中数据</param>
        /// <param name="threadNo">线程序号</param>
        /// <param name="level">迭代层级</param>
        private void getFileByRegex(string filename, string urlPath, string pattern, int grpId, bool isGether, int threadNo, int level)
        {
            StreamReader sr = File.OpenText(localPath + filename);
            string content = sr.ReadToEnd();
            sr.Close();

            if (content.Length <= 0)
            {
                logStatus("读取文件数据失败\r\n");
                return;
            }

            // 抓取整个页面内嵌文件，记录输出内容
            string content_out = content;

            string filePath = getURLPart(urlPath + "test.htm", URL_PART_PATH).Replace('/', '\\');
            filePath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            level++; //加深一层
            int curCount = 0, maxCount = int.Parse(txtMaxCount.Text);
            foreach (Match match in matches)
            {
                string getUrl = "", localFile = "";
                string file1 = match.Groups[grpId].Value;
                if (file1 == null || file1.Trim().Length <= 0)
                    continue;

                curCount++;
                // 如果不抓取就只打印并返回
                if (!isGether)
                {
                    logStatus("匹配输出=" + file1 + "\r\n");
                    continue;
                }

                if (maxCount > 0 && curCount > maxCount)
                {
                    logStatus("超出匹配上限（" + maxCount + "），预计舍弃数量（" + (matches.Count - maxCount) + "）\r\n");
                    break;
                }

                if (file1.StartsWith("//")) //如：//www.moretuan.net/tuan/feed.php
                {
                    getUrl = "http:" + file1;
                    localFile = getURLPart(getUrl, URL_PART_PATH).Replace('/', '\\');
                }
                else if (file1.StartsWith("http://") || file1.StartsWith("https://")) //如：http://www.moretuan.net/tuan/feed.php
                {
                    getUrl = file1;
                    localFile = getURLPart(getUrl, URL_PART_PATH).Replace('/', '\\');
                }
                else if (file1.StartsWith("/"))      //根目录形式，如：/tuan/feed.php
                {
                    //如："http://moretuan.net/" + "tuan/feed.php"
                    getUrl = domainName + file1.TrimStart('/');

                    //保存图片文件，如："tuan\feed.php"
                    localFile = getURLPart(getUrl, URL_PART_PATH).Replace('/', '\\');
                }
                else //相对目录形式，如：tuan/feed.php
                {
                    //如："http://moretuan.net/tuan/" + "tuan/feed.php"
                    getUrl = urlPath + file1;

                    //保存图片文件，如："tuan\" + "tuan\feed.php"
                    localFile = getURLPart(getUrl, URL_PART_PATH).Replace('/', '\\');
                }

                //替换本地元素链接
                if (chkDownWholePage.Checked)
                {
                    if (file1 != "" && file1 != "/")
                    {
                        if (localFile.StartsWith("index.html"))
                        {
                            if (!file1.EndsWith("/"))
                            {
                                content_out = content_out.Replace(file1 + "/", "");
                            }
                            content_out = content_out.Replace(file1, "");
                        }
                        else
                        {
                            content_out = content_out.Replace(file1, localFile.Replace('\\', '/'));
                        }
                    }
                }

                string savePath = localPath + localFile;
                string strGather = chkGather2.Checked ? "true" : "false";

                logStatus("下载文件：" + file1 + "\r\n");
                if (chkMulThread.Checked) // 是否开启多线程
                {
                    IAsyncResult result = SaveWebFileMethod.BeginInvoke(getUrl, savePath, new AsyncCallback(DownFinished), new string[] { getUrl, txtRegEx2.Text.Trim(), txtGrp2.Text.Trim(), strGather, threadNo.ToString(), level.ToString() });
                }
                else
                {
                    IAsyncResult result = SaveWebFileMethod.BeginInvoke(getUrl, savePath, null, new string[] { getUrl, txtRegEx2.Text.Trim(), txtGrp2.Text.Trim(), strGather, threadNo.ToString(), level.ToString() });
                    int rstCode = SaveWebFileMethod.EndInvoke(result);  //阻塞线程，实行单线程下线
                    DownFinished_End(result, rstCode);
                }
            }

            // 生成引用本地元素的新页面
            if (chkDownWholePage.Checked && content_out.Length > 0)
            {
                int pos = filename.LastIndexOf(".");
                string outfile = filename.Substring(0, pos) + "_out" + filename.Substring(pos);
                string outPath = localPath + outfile;
                StreamWriter sw = new StreamWriter(File.OpenWrite(outPath));
                sw.Write(content_out);
                sw.Close();

                logStatus("输出新页面=" + outPath + "\r\n");
            }
        }

        delegate int SaveWebFileDelegate(string url, string SavePath);
        /// <summary>        
        /// 保存远程文件
        /// </summary>        
        /// <param name="url"></param>        
        /// <param name="SavePath"></param>        
        private int SaveWebFile(string url, string SavePath)
        {
            if (File.Exists(SavePath))
            {
                //文件已存在
                return DOWNFILE_EXISTS;
            }

            byte[] buffer;
            WebClient wc = new WebClient();
            try
            {
                buffer = wc.DownloadData(url);
                // wc.DownloadFileCompleted += downloadFileCompleted;
                // wc.DownloadProgressChanged += downloadProgressChanged;
                // wc.DownloadDataAsync(new Uri(url), SavePath);
            }
            catch
            {
                //下载失败
                return DOWNFILE_ERROR;
            }

            try
            {
                CreatePath(SavePath);
                FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            catch
            {
                //文件写入失败
                return DOWNFILE_WRITERR;
            }


            //下载成功
            return DOWNFILE_SUCCESS;
        }
        /// <summary>
        /// 下载完成 回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void downloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                logStatus("下载文件：" + e.UserState.ToString() + ",下载完成\r\n");
            }
        }
        /// <summary>
        /// 下载进度 回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            logStatus("下载文件：" + e.UserState.ToString() + "," + e.ProgressPercentage + "%\r\n");
        }

        /// <summary>
        /// 取域名及文件名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="groupid">正则匹配组索引，参见常量：URL_PART_XXX</param>
        /// <returns></returns>
        private static string getURLPart(string url, int groupid)
        {
            string cont = "";
            string pattern = @"((http[s]?:)?//[^/]*?/)([^?]*)(\?.*)?";
            Match match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
            if (match != null)
            {
                if (groupid == URL_PART_EXT)
                {
                    //先取路径再分析后缀情况
                    cont = match.Groups[URL_PART_PATH].Value;
                }
                else
                {
                    cont = match.Groups[groupid].Value;
                }
            }

            if (groupid == URL_PART_DOMAIN && match.Groups[URL_PART_PROTOCOL].Value == "")
            {
                //针对 //domain.com 域名，默认补全http:协议
                cont = "http:" + cont;
            }
            else if (groupid == URL_PART_PATH)
            {
                if (cont == "")
                {
                    //如果匹配不到文件路径，则将url去掉域名，再拼接成文件名
                    //string path = Regex.Replace(url, @"(http[s]?://[^/]*?/)", "");
                    //string fileName = path.Replace("/", "_").Replace(":", "_").Replace("?", "_");

                    //如果匹配不到文件路径，则取参数部分，再拼接成文件名
                    string param = match.Groups[URL_PART_PARAM].Value;
                    if (param.Length > 0)
                    {
                        string fileName = param.Replace("/", "_").Replace(":", "_").Replace("?", "_");
                        cont = fileName + ".html";
                    }
                    else
                    {
                        cont = "index.html";
                    }
                }
                else if (isAddParamName) //追加网址参数到文件名
                {
                    //为了应对URL只有参数变化的情况，抓取不同参数网址文件，如：detail.htm?id=1，文件存储名为detail_id=1.htm
                    string param = match.Groups[URL_PART_PARAM].Value;
                    string pName = param.Replace("/", "_").Replace(":", "_").Replace("?", "_");
                    int pos = cont.LastIndexOf('.');
                    if (pos >= 0)
                    {
                        cont = cont.Substring(0, pos) + "_" + pName + cont.Substring(pos);
                    }
                    else
                    {
                        cont = cont + "_" + pName + "html";
                    }
                }
                else
                {
                }
            }
            else if (groupid == URL_PART_EXT)
            {
                if (cont.Length > 0)
                {
                    int pos = cont.LastIndexOf(".");
                    if (pos >= 0)
                    {
                        cont = cont.Substring(pos + 1);
                    }
                    else
                    {
                        cont = "";
                    }
                }
                //如果匹配不到后缀，默认补全html
                //cont = "html";
            }

            return cont.ToString();
        }

        /// <summary>
        /// 返回下载状态
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string getDownFileStatus(int code)
        {
            string result = "";
            switch (code)
            {
                case DOWNFILE_SUCCESS:
                    result = "下载成功";
                    break;
                case DOWNFILE_ERROR:
                    result = "下载失败";
                    break;
                case DOWNFILE_EXISTS:
                    result = "文件已存在";
                    break;
                case DOWNFILE_WRITERR:
                    result = "文件写入失败";
                    break;
                default:
                    result = "未知错误";
                    break;
            }

            return result;
        }

        /// <summary>
        ///  创建一个目录树
        /// </summary>
        /// <param name="path">形式如：E:\\a\\b\\c\\</param>
        private static void CreatePath(string path)
        {
            int pos = path.IndexOf('\\', 0);
            while (pos > 0)
            {
                string dir = path.Substring(0, pos + 1);
                DirectoryInfo info = Directory.CreateDirectory(dir);
                pos = path.IndexOf('\\', pos + 1);
            }
        }

        /// <summary>
        /// 在字符串前加0,是字符串达到指定的长度.
        /// </summary>
        /// <param name="str">要修改的字符串.</param>
        /// <param name="len">指定的长度.</param>
        /// <returns></returns>
        private static string AddZero(string str, int len)
        {
            if (str.Length < len)
            {
                for (int ii = str.Length; ii < len; ii++)
                {
                    str = "0" + str;
                }
            }
            return str;
        }

        //定义委托
        delegate void ShowStatusDelegate(string msg);

        /// <summary>
        /// 写状态日志信息
        /// </summary>
        /// <param name="msg"></param>
        private void logStatus(string msg)
        {
            if (chkRichBox.Checked)
            {
                if (brwStatus.InvokeRequired)
                {
                    ShowStatusDelegate d = new ShowStatusDelegate(logStatus);
                    brwStatus.Invoke(d, msg);
                }
                else
                {
                    string info = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + msg;
                    statusLogBuf.Append(info);

                    brwStatus.DocumentText = searchKeyWord(txtKeyword.Text).ToString();
                    // 滚动至底
                    //brwStatus.Document.Window.ScrollTo(0, brwStatus.Document.Body.ScrollRectangle.Height);
                }
            }
            else
            {
                if (txtStatus.InvokeRequired)
                {
                    ShowStatusDelegate d = new ShowStatusDelegate(logStatus);
                    txtStatus.Invoke(d, msg);
                }
                else
                {
                    string info = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + msg;
                    statusLogBuf.Append(info);

                    txtStatus.Clear();
                    txtStatus.AppendText(searchKeyWord(txtKeyword.Text).ToString());
                }
            }
        }

        //扁平化拷贝
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtFolder.Text.Trim() == "")
            {
                MessageBox.Show("请选择操作目录！");
                txtFolder.Focus();
                return;
            }
            if (txtDestDir.Text.Trim() == "")
            {
                MessageBox.Show("请输入目标子目录！");
                txtDestDir.Focus();
                return;
            }

            logStatus("操作开始.." + "\r\n");
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";
            string newPath = localPath + txtDestDir.Text.Trim();
            logStatus("原始目录：" + localPath + "\r\n");
            logStatus("目标目录：" + newPath + "\r\n");
            logStatus("目标文件清单如下：" + "\r\n");
            if (!Directory.Exists(newPath))
            {
                if (chkRealExecute.Checked)
                {
                    Directory.CreateDirectory(newPath);
                }
            }
            //copyFile(localPath, newPath, true);

            string filename = "", filter = "*.*";
            if (txtFileExt.Text.Trim().Length > 0)
            {
                filter = txtFileExt.Text.Trim();
            }
            string[] files = GetFiles(localPath, filter, SearchOption.AllDirectories).Where(s => s.IndexOf(newPath) < 0).ToArray();
            foreach (string file in files)
            {
                filename = file.Replace(localPath, "").Replace("\\", "_");

                if (chkOverWrite.Checked || !File.Exists(newPath + filename))
                {
                    logStatus("拷贝文件：" + filename + "\r\n");
                }
                else
                {
                    logStatus("跳过拷贝：" + filename + "\r\n");
                }

                if (chkRealExecute.Checked)
                {
                    if (chkOverWrite.Checked || !File.Exists(newPath + filename))
                    {
                        File.Copy(file, newPath + filename, true);
                    }
                }
            }

            logStatus("操作结束.." + "\r\n");
        }

        // 多个文件过滤器调用GetFiles，如filters="*.jpg|*.bmp"
        private static string[] GetFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }

        /// <summary>
        /// 获取耗费时间
        /// </summary>
        /// <param name="milliseconds">毫秒数</param>
        /// <returns></returns>
        private static string getUseTime(long milliseconds)
        {
            int hour = (int)Math.Floor(1.0 * milliseconds / 3600000);
            int minute = (int)Math.Floor(1.0 * (milliseconds - hour * 3600000) / 60000);
            int second = (int)Math.Floor(1.0 * (milliseconds - hour * 3600000 - minute * 60000) / 1000);
            int millisecond = (int)(milliseconds - hour * 3600000 - minute * 60000 - second * 1000);
            string strTime = string.Format("{0:D2}时 {1:D2}分 {2:D2}秒 {3:D1}", hour, minute, second, millisecond / 100);
            return strTime;
        }

        private void chkGather_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGather.Checked && txtRegEx.Text.Trim().Length <= 0)
            {
                MessageBox.Show("请输入第一层正规表达式！");
                txtRegEx.Focus();
            }
        }

        private void chkGather2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGather2.Checked && txtRegEx2.Text.Trim().Length <= 0)
            {
                MessageBox.Show("请输入第二层正规表达式！");
                txtRegEx2.Focus();
            }
        }

        private void txtRegEx_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtRegEx2_TextChanged(object sender, EventArgs e)
        {
            // 如果使用第二层正则，那么第一层正则不为空，且勾选抓取数据
            if (txtRegEx2.Text.Trim().Length > 0)
            {
                chkGather.Checked = true;
            }
        }

        // 窗口关闭前
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dlgRst = MessageBox.Show("是否保存当前修改过的配置？", "温馨提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (dlgRst == DialogResult.Yes)
            {
                //写入配置文件
                WriteConfig(cur_config_file);
            }
            else if (dlgRst == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        // 载入配置文件
        private void picBoxCfg_Click(object sender, EventArgs e)
        {
            openFileDlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDlg.Filter = "配置文件|*.ini|所有文件|*.*";
            DialogResult dlgRst = openFileDlg.ShowDialog();
            if (dlgRst == DialogResult.OK)
            {
                try
                {
                    //读取配置文件
                    ReadConfig(openFileDlg.FileName);
                    cur_config_file = openFileDlg.FileName;
                    toolTip1.SetToolTip(lnkLabel, cur_config_file);
                    lnkLabel.LinkColor = Color.Red; // 标记红色表示修改过
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取配置文件失败！");
                }
            }
        }

        // 点击配置文件链接
        private void lnkLabel_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = @" /select, " + cur_config_file; //选定文件
            // p.StartInfo.Arguments = @" " + cur_config_file; //直接打开文件
            p.Start();
        }

        #region 读写配置文件操作

        /// <summary>
        /// 写入配置文件
        /// </summary>
        private void WriteConfig(string iniFile = "")
        {
            if (iniFile == "")
            {
                iniFile = AppDomain.CurrentDomain.BaseDirectory + CONFIG_FILE;
            }
            IniFiles ini = new IniFiles(iniFile);

            ini.WriteString(INI_SESSION, "txtURL", txtURL.Text);
            ini.WriteString(INI_SESSION, "txtFolder", txtFolder.Text);
            ini.WriteString(INI_SESSION, "txtRegEx", txtRegEx.Text);
            ini.WriteString(INI_SESSION, "txtRegEx2", txtRegEx2.Text);
            ini.WriteString(INI_SESSION, "txtNum1", txtNum1.Text);
            ini.WriteString(INI_SESSION, "txtNum2", txtNum2.Text);
            ini.WriteString(INI_SESSION, "txtGrp1", txtGrp1.Text);
            ini.WriteString(INI_SESSION, "txtGrp2", txtGrp2.Text);
            ini.WriteString(INI_SESSION, "txtLen", txtLen.Text);
            ini.WriteString(INI_SESSION, "txtThreadNum", txtThreadNum.Text);
            ini.WriteString(INI_SESSION, "txtMaxCount", txtMaxCount.Text);
            ini.WriteString(INI_SESSION, "txtFileExt", txtFileExt.Text);
            ini.WriteString(INI_SESSION, "txtDestDir", txtDestDir.Text);

            ini.WriteBool(INI_SESSION, "chkGather", chkGather.Checked);
            ini.WriteBool(INI_SESSION, "chkGather2", chkGather2.Checked);
            ini.WriteBool(INI_SESSION, "chkOverWrite", chkOverWrite.Checked);
            ini.WriteBool(INI_SESSION, "chkRealExecute", chkRealExecute.Checked);
            ini.WriteBool(INI_SESSION, "chkMulThread", chkMulThread.Checked);
            ini.WriteBool(INI_SESSION, "chkAddParamName", chkAddParamName.Checked);
            //ini.WriteBool(INI_SESSION, "chkDownWholePage", chkDownWholePage.Checked);
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        private void ReadConfig(string iniFile = "")
        {
            if (iniFile == "")
            {
                iniFile = AppDomain.CurrentDomain.BaseDirectory + CONFIG_FILE;
            }
            IniFiles ini = new IniFiles(iniFile);

            txtURL.Text = ini.ReadString(INI_SESSION, "txtURL", "");
            txtFolder.Text = ini.ReadString(INI_SESSION, "txtFolder", "");
            txtRegEx.Text = ini.ReadString(INI_SESSION, "txtRegEx", "");
            txtRegEx2.Text = ini.ReadString(INI_SESSION, "txtRegEx2", "");
            txtNum1.Text = ini.ReadString(INI_SESSION, "txtNum1", "0");
            txtNum2.Text = ini.ReadString(INI_SESSION, "txtNum2", "0");
            txtGrp1.Text = ini.ReadString(INI_SESSION, "txtGrp1", "0");
            txtGrp2.Text = ini.ReadString(INI_SESSION, "txtGrp2", "0");
            txtLen.Text = ini.ReadString(INI_SESSION, "txtLen", "1");
            txtThreadNum.Text = ini.ReadString(INI_SESSION, "txtThreadNum", "1");
            txtMaxCount.Text = ini.ReadString(INI_SESSION, "txtMaxCount", "0");
            txtFileExt.Text = ini.ReadString(INI_SESSION, "txtFileExt", "*.jpg|*.bmp");
            txtDestDir.Text = ini.ReadString(INI_SESSION, "txtDestDir", "backup\\");

            chkGather.Checked = ini.ReadBool(INI_SESSION, "chkGather", false);
            chkGather2.Checked = ini.ReadBool(INI_SESSION, "chkGather2", false);
            chkOverWrite.Checked = ini.ReadBool(INI_SESSION, "chkOverWrite", false);
            chkRealExecute.Checked = ini.ReadBool(INI_SESSION, "chkRealExecute", false);
            chkMulThread.Checked = ini.ReadBool(INI_SESSION, "chkMulThread", false);
            chkAddParamName.Checked = ini.ReadBool(INI_SESSION, "chkAddParamName", false);
            //chkDownWholePage.Checked = ini.ReadBool(INI_SESSION, "chkDownWholePage", false);
        }

        #endregion

        #region 搜索框placehold效果

        // 输入要检索的文字
        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            if (chkRichBox.Checked)
            {
                brwStatus.DocumentText = searchKeyWord(txtKeyword.Text).ToString();
            }
            else
            {
                txtStatus.Text = searchKeyWord(txtKeyword.Text).ToString();
            }
            if (txtKeyword.Text.Length > 0)
            {
                lbPlacehold.Visible = false;
            }
            else
            {
                lbPlacehold.Visible = true;
            }
        }

        private void txtKeyword_Enter(object sender, EventArgs e)
        {
            lbPlacehold.Visible = false;
        }

        private void txtKeyword_Leave(object sender, EventArgs e)
        {
            if (txtKeyword.Text.Length > 0)
            {
                lbPlacehold.Visible = false;
            }
            else
            {
                lbPlacehold.Visible = true;
            }
        }

        private void lbPlacehold_Click(object sender, EventArgs e)
        {
            lbPlacehold.Visible = false;
            txtKeyword.Focus();
        }

        #endregion

        /// <summary>
        /// 检索文字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private StringBuilder searchKeyWord(string key)
        {
            StringBuilder sb = new StringBuilder();
            int matchLines = 0, matchCount = 0;
            if (key.Length <= 0)
            {
                lblMatchCount.Text = string.Format("匹配 {0} 行，{1} 处", matchLines, matchCount);
                //return statusLogBuf;

                if (chkRichBox.Checked)
                {
                    sb = new StringBuilder("<body style='white-space: nowrap;'>" + statusLogBuf.ToString().Replace("\r\n", "<br>") + "<body>");
                }
                else
                {
                    sb = new StringBuilder(statusLogBuf.ToString());
                }
                return sb;
            }

            if (chkRichBox.Checked)
            {
                sb.Append("<body style='white-space: nowrap;'>");
            }
            string[] words = Regex.Split(statusLogBuf.ToString(), "\r\n", RegexOptions.IgnoreCase);
            CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            foreach (string line in words)
            {
                if (line.Length > 0)
                {
                    MatchCollection matches = null;
                    try
                    {
                        matches = Regex.Matches(line, key, RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    // if (Compare.IndexOf(line, key, CompareOptions.IgnoreCase) >= 0)
                    if (matches.Count > 0)
                    {
                        if (chkRichBox.Checked)
                        {
                            string newLine = Regex.Replace(line, key, "<font color='red'>$0</font>", RegexOptions.IgnoreCase);
                            sb.Append(newLine + "<br>");
                        }
                        else
                        {
                            string newLine = Regex.Replace(line, key, "【$0】", RegexOptions.IgnoreCase);
                            sb.Append(newLine + "\r\n");
                        }
                        matchLines++;
                        matchCount += matches.Count;
                    }
                }
            }

            lblMatchCount.Text = string.Format("匹配 {0} 行，{1} 处", matchLines, matchCount);
            if (chkRichBox.Checked)
            {
                sb.Append("<body>");
            }
            return sb;
        }

        private void chkRichBox_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRichBox.Checked)
            {
                brwStatus.Show();
                txtStatus.Hide();
            }
            else
            {
                brwStatus.Hide();
                txtStatus.Show();
            }

            txtKeyword_TextChanged(txtKeyword, e);
        }

        private void chkDownWholePage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDownWholePage.Checked)
            {
                //下载页面元素及完整页面，生成引用本地元素的新页面
                DialogResult dlgRst = MessageBox.Show("将改变当前界面的配置参数（只设置第一层正则），确认操作？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dlgRst == DialogResult.Yes)
                {
                    //配置 抓取整个页面内嵌文件 的参数
                    txtRegEx.Text = @"(((href|src)[\s]*?=)|(url\())[\s]*?['""]((?!javascript:).*?)['""]";
                    txtGrp1.Text = "5";
                    chkGather.Checked = true;

                    txtRegEx2.Text = "";
                    txtGrp2.Text = "0";
                    chkGather2.Checked = false;
                }
            }
        }
    }
}
