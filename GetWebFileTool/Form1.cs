﻿using System;
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

        ArrayList list = new ArrayList();

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

            }


            if (txtFolder.Text.Trim() == "")
            {
                MessageBox.Show("请选择操作目录！");
                txtFolder.Focus();
                return;
            }

            //URL地址
            string url = txtURL.Text.Trim();

            //网址域名，如：http://moretuan.net/
            domainName = getURLPart(url, 1);

            //本地路径，如：D:\\Demo\
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";

            ShowStatus("开始下载页面...\r\n");
            if (url.IndexOf("(*)") > 0)
            {
                int len = int.Parse(txtLen.Text);
                int num1 = int.Parse(txtNum1.Text);
                int num2 = int.Parse(txtNum2.Text);
                for (int i = num1; i <= num2; i++)
                {
                    string num = AddZero(i.ToString(), len);
                    string url2 = url.Replace("(*)", num);

                    string filename = getURLPart(url2, 2).Replace('/', '\\');
                    string savePath = localPath + filename;

                    SaveWebFileMethod.BeginInvoke(url2, savePath, new AsyncCallback(DownFinished), new string[] { url2, txtRegEx.Text.Trim(), txtGrp1.Text.Trim()});
                }
            }
            else
            {
                string filename = getURLPart(url, 2).Replace('/', '\\');
                string savePath = localPath + filename;

                SaveWebFileMethod.BeginInvoke(url, savePath, new AsyncCallback(DownFinished), new string[] { url, txtRegEx.Text.Trim(), txtGrp1.Text.Trim() });
            }
        }

        //完成下载
        private void DownFinished(IAsyncResult result)
        {
            int code = SaveWebFileMethod.EndInvoke(result);
            string status = getDownFileStatus(code);

            string[] strArry = result.AsyncState as string[];
            string url = strArry[0];
            string regEx = strArry[1]; //正则表达式
            int grpId = int.Parse(strArry[2]); //匹配组序号

            //string url = result.AsyncState.ToString();
            //string regEx = txtRegEx.Text.Trim();
            string filename = getURLPart(url, 2).Replace('/', '\\');

            ShowStatus(filename + " (" + status + ")\r\n");

            string extFile = getURLPart(filename, 3);
            bool isTextFile = ("txt|xml|htm|html|shtml|asp|aspx|php|jsp|js|css".IndexOf(extFile) >= 0);

            //获取文件(通过regEx分析)
            if (regEx != "" && code == DOWNFILE_SUCCESS && isTextFile)
            {
                string savePath = localPath + filename;
                StreamReader sr = File.OpenText(savePath);
                string strContent = sr.ReadToEnd();
                sr.Close();

                string urlPath = url.Substring(0, url.LastIndexOf("/") + 1);
                getFileByRegex(strContent, urlPath, regEx, grpId);
            }
        }

        /// <summary>
        /// 获取文件(通过正则表达式分析)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="urlPath"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="grpId">匹配组id</param>
        private void getFileByRegex(string content, string urlPath, string pattern, int grpId)
        {
            string filePath = getURLPart(urlPath + "test.htm", 2).Replace('/', '\\');
            filePath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                string getUrl = "", localFile = "";
                string file1 = match.Groups[1].Value;
                if (file1 == null || file1.Trim().Length <= 0)
                    continue;
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

                SaveWebFileMethod.BeginInvoke(getUrl, savePath, new AsyncCallback(DownFinished), new string[] { getUrl, txtRegEx2.Text.Trim(), txtGrp2.Text.Trim() });
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

            byte[] buffer ;
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
            }else if (groupid == 3 && cont == "")
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
            switch(code)
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
                string dir = path.Substring(0, pos+1);
                DirectoryInfo info = Directory.CreateDirectory(dir);
                pos=path.IndexOf('\\',pos+1);
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
                txtStatus.AppendText(msg);
            }
        }



        //遍历拷贝
        private void btnCopy_Click(object sender, EventArgs e)
        {
            localPath = txtFolder.Text.Trim().TrimEnd('\\') + "\\";
            string newPath = localPath + "backup\\";
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            copyFile(localPath, newPath);
        }

        private void copyFile(string path, string newPath)
        {
            string filename = "";
            string[] files = Directory.GetFiles(path, "*.jpg");
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                filename = file.Replace(localPath, "").Replace("\\", "_");
                File.Copy(file, newPath + filename);
                txtStatus.AppendText(filename + "\r\n");
            }
            foreach (string dir in dirs)
            {
                copyFile(dir, newPath);
            }
        }
    }
}