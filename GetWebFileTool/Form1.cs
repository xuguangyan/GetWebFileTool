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

namespace GetWebSiteTool
{
    public partial class Form1 : Form
    {
        const int DOWNFILE_SUCCESS = 0;
        const int DOWNFILE_ERROR = -1;
        const int DOWNFILE_EXISTS = -2;
        const int DOWNFILE_WRITERR = -3;

        static string localPath = "";  //本地路径
        static string domainName = ""; //网址域名
        static int taskTotal = 1; //总共任务数
        static int taskDone = 0;  //完成任务数
        static Queue<String> urls = new Queue<String>(); // 任务队列(url)
        static Object locker = new Object(); //线程锁
        static DateTime startTime;

        public Form1()
        {
            InitializeComponent();

            txtFolder.Text = AppDomain.CurrentDomain.BaseDirectory + "download\\";
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
            txtStatus.Text = "";
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

            //URL地址
            string url = txtURL.Text.Trim();

            //网址域名，如：http://moretuan.net/
            domainName = getURLPart(url, 1);

            //本地路径，如：D:\\Demo\
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";

            //任务数
            taskTotal = 1;
            //已完成
            taskDone = 0;

            startTime = DateTime.Now;
            ShowStatus("开始任务...\r\n");
            string strGather = chkGather.Checked ? "true" : "false";

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

                    string filename = getURLPart(url2, 2).Replace('/', '\\');
                    string savePath = localPath + filename;

                    ShowStatus("下载线程" + threadNo + "：" + filename + "\r\n");
                    SaveWebFileMethod.BeginInvoke(url2, savePath, new AsyncCallback(DownFinished), new string[] { url2, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, (threadNo++).ToString() });
                }
            }
            else
            {
                string filename = getURLPart(url, 2).Replace('/', '\\');
                string savePath = localPath + filename;

                ShowStatus("下载线程" + threadNo + "：" + filename + "\r\n");
                SaveWebFileMethod.BeginInvoke(url, savePath, new AsyncCallback(DownFinished), new string[] { url, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, threadNo.ToString() });
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

            //string url = result.AsyncState.ToString();
            //string regEx = txtRegEx.Text.Trim();
            string filename = getURLPart(url, 2).Replace('/', '\\');

            lock (locker)
            {
                taskDone++;
                ShowStatus("完成线程" + threadNo + "：" + filename + " (" + status + ")\r\n");
                ShowStatus("任务进度：" + taskDone + "/" + taskTotal + "\r\n");
            }

            string extFile = getURLPart(filename, 3);
            // bool isTextFile = ("txt|xml|htm|html|shtml|asp|aspx|php|jsp|js|css".IndexOf(extFile) >= 0);

            //获取文件(通过regEx分析)
            if (regEx != "" && (code == DOWNFILE_SUCCESS || code == DOWNFILE_EXISTS)/* && isTextFile*/)
            {
                string savePath = localPath + filename;
                StreamReader sr = File.OpenText(savePath);
                string strContent = sr.ReadToEnd();
                sr.Close();

                ShowStatus("开始正则分析：" + filename + " ========\r\n");
                string urlPath = url.Substring(0, url.LastIndexOf("/") + 1);
                getFileByRegex(strContent, urlPath, regEx, grpId, isGather, threadNo);
                ShowStatus("完成正则分析：" + filename + " ========\r\n");
            }

            lock (locker)
            {
                if (taskDone >= taskTotal)
                {
                    double useTime = (DateTime.Now - startTime).TotalMilliseconds;
                    string strUseTime = getUseTime(Convert.ToInt64(useTime));
                    ShowStatus("已完成，任务总数：" + taskTotal + "，耗时：" + strUseTime + "\r\n");
                }
            }

            if (urls.Count > 0)
            {
                string url2 = urls.Dequeue();
                string filename2 = getURLPart(url2, 2).Replace('/', '\\');
                string savePath2 = localPath + filename2;
                string strGather = chkGather.Checked ? "true" : "false";

                ShowStatus("下载线程" + threadNo + "：" + filename2 + "\r\n");
                SaveWebFileMethod.BeginInvoke(url2, savePath2, new AsyncCallback(DownFinished), new string[] { url2, txtRegEx.Text.Trim(), txtGrp1.Text.Trim(), strGather, threadNo.ToString() });
            }
        }

        /// <summary>
        /// 获取文件(通过正则表达式分析)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="urlPath"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="grpId">匹配组id</param>
        /// <param name="isGether">是否抓取文件中数据</param>
        /// <param name="threadNo">线程序号</param>
        private void getFileByRegex(string content, string urlPath, string pattern, int grpId, bool isGether, int threadNo)
        {
            string filePath = getURLPart(urlPath + "test.htm", 2).Replace('/', '\\');
            filePath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            int curCount = 0, maxCount = 0;
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
                    ShowStatus("匹配输出=" + file1 + "\r\n");
                    continue;
                }

                if (maxCount > 0 && curCount > maxCount)
                {
                    ShowStatus("超出嵌套上限（" + maxCount + "），预计舍弃数量（" + (matches.Count - maxCount) + "）\r\n");
                    break;
                }

                if (file1.StartsWith("http://") || file1.StartsWith("https://"))//如：http://www.moretuan.net/tuan/feed.php
                {
                    //if (file1.IndexOf(domainName) < 0)
                    //{
                    //    ShowStatus("发现外部资源：" + file1 + "\r\n");
                    //    continue;   //外部资源不下载
                    //}
                    getUrl = file1; //如：http://www.moretuan.net/tuan/feed.php?ename=GZ
                    localFile = getURLPart(file1, 2).Replace('/', '\\');
                }
                else if (file1.StartsWith("/"))      //如：/tuan/feed.php
                {
                    //如："http://moretuan.net/" + "tuan/feed.php"
                    getUrl = domainName + file1.TrimStart('/');

                    //保存图片文件，如："tuan\feed.php"
                    localFile = file1.TrimStart('/').Replace('/', '\\');
                }
                else
                {
                    //如："http://moretuan.net/tuan/" + "tuan/feed.php"
                    getUrl = urlPath + file1;

                    //保存图片文件，如："tuan\" + "tuan\feed.php"
                    localFile = filePath + file1.Replace('/', '\\');
                }
                string savePath = localPath + localFile;
                string strGather = chkGather2.Checked ? "true" : "false";

                ShowStatus("下载文件：" + file1 + "\r\n");
                // IAsyncResult result = SaveWebFileMethod.BeginInvoke(getUrl, savePath, new AsyncCallback(DownFinished), new string[] { getUrl, txtRegEx2.Text.Trim(), txtGrp2.Text.Trim(), strGather, threadNo.ToString() });
                IAsyncResult result = SaveWebFileMethod.BeginInvoke(getUrl, savePath, null, new string[] { getUrl, txtRegEx2.Text.Trim(), txtGrp2.Text.Trim(), strGather, threadNo.ToString() });
                int rstCode = SaveWebFileMethod.EndInvoke(result);  //阻塞线程，实行单线程下线
                DownFinished_End(result, rstCode);
            }
        }

        delegate int SaveWebFileDelegate(string url, string SavePath);
        SaveWebFileDelegate SaveWebFileMethod = new SaveWebFileDelegate(SaveWebFile);
        /// <summary>        
        /// 保存远程文件
        /// </summary>        
        /// <param name="url"></param>        
        /// <param name="SavePath"></param>        
        private static int SaveWebFile(string url, string SavePath)
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
        /// 取域名及文件名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        private static string getURLPart(string url, int groupid)
        {
            string cont = "";
            string pattern = @"(http[s]?://[^/]*?/)([^\.]+\.([\w]{1,4}))?[\s\S]*";
            Match match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
            if (match != null)
            {
                cont = match.Groups[groupid].Value;
            }

            if (groupid == 2 && cont == "")
            {
                string fileName = url.Replace("/", "").Replace(":", "").Replace("?", "");
                cont = fileName + ".html";
            }
            else if (groupid == 3 && cont == "")
            {
                cont = "html";
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
        /// 显示状态信息
        /// </summary>
        /// <param name="msg"></param>
        private void ShowStatus(string msg)
        {
            if (txtStatus.InvokeRequired)
            {
                ShowStatusDelegate d = new ShowStatusDelegate(ShowStatus);
                txtStatus.Invoke(d, msg);
            }
            else
            {
                txtStatus.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + msg);
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

            ShowStatus("操作开始.." + "\r\n");
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";
            string newPath = localPath + "backup\\";
            ShowStatus("原始目录：" + localPath + "\r\n");
            ShowStatus("目标目录：" + newPath + "\r\n");
            ShowStatus("目标文件清单如下：" + "\r\n");
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
                    txtStatus.AppendText("拷贝文件：" + filename + "\r\n");
                }
                else
                {
                    txtStatus.AppendText("跳过拷贝：" + filename + "\r\n");
                }

                if (chkRealExecute.Checked)
                {
                    if (chkOverWrite.Checked || !File.Exists(newPath + filename))
                    {
                        File.Copy(file, newPath + filename, true);
                    }
                }
            }

            ShowStatus("操作结束.." + "\r\n");
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
    }
}
