using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using static AutoUpdate.Form1;
using Timer = System.Windows.Forms.Timer;

namespace AutoUpdate
{
    public partial class Form1 : Form
    {
        private string connectURL1 = @"https://www.smartmancs.com.tw/download/";

        private System.Windows.Forms.Timer timer;

        private int dotCount = 1;

        private bool isConnect = false;

        private string ID = "";

        private string ConfigFileName = @"AutoUpdate.ini";

        private string releaseFileName = "release.json";

        private string localtemppath = "\\AutoUpdate";

        static int URLConnectTimeOutSec = 10;

        private List<UpdateFile> updateUrlFilesList;

        private List<UpdateFile> updateFilesList = new List<UpdateFile>();

        public class UpdateFile
        {
            public string filename { get; set; }
            public string version { get; set; }
            public string urlpath { get; set; }
            public string smartmanpath { get; set; }
        }


        public class ReleaseInfo
        {
            public List<UpdateFile> Files { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            // 禁止調整視窗大小
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //增加關閉視窗事件
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            go();
        }

        //關閉視窗
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //10.刪除temp資料夾
            if (Directory.Exists(System.Environment.CurrentDirectory + localtemppath))
            {
                Directory.Delete(System.Environment.CurrentDirectory + localtemppath, true);
            }
        }

        public void go()
        {
            //1.讀取ini檔
            if (Read_ini())
            {
                //2.初始化
                Init();
                textBoxStatus.Text = "正在檢查更新";
                //3.連線測試
                if (connectURL())
                {
                    textBoxStatus.Text = "已連到主機";
                    //4.取得檔案名單存至 updateUrlFilesList
                    if (GetFileLists())
                    {
                        //5.檢查檔案存不存在url上
                        if (checkFileExsit())
                        {
                            textBoxStatus.Text = "取得檔案名單";
                            //6.下載檔案
                            downloadfile();
                            //7.檢查檔案日期,要更新的檔案名單存至 updateFilesList
                            checkFileDate();
                            if(updateFilesList.Count > 0)
                            {
                                //8.用 updateFilesList 檢查.exe檔案有沒有在使用中(如果沒有使用中會更改檔案名稱)
                                if (checkFileIsExeing())
                                {
                                    textBoxStatus.Text = "檔案更新中";
                                    //9.解壓縮檔案
                                    if (UnZipFile())
                                    {
                                        textBoxStatus.Text = "檔案更新完成";
                                    }
                                    else
                                    {
                                        textBoxStatus.Text = "檔案更新失敗";
                                    }
                                }
                            }
                            else
                            {
                                textBoxStatus.Text = "不需要更新";
                            }
                        }
                    }
                }
            }
        }

        public bool UnZipFile()
        {
            bool result = true;
            foreach (UpdateFile file in updateFilesList)
            {
                string urlpath = file.urlpath;
                string updatefilename = file.filename;
                //urlpath最右邊的一個/後面的字串
                string filename = urlpath.Substring(urlpath.LastIndexOf("/") + 1);
                string smartmanpath = file.smartmanpath;
                string path = System.Environment.CurrentDirectory + localtemppath + "\\" + filename;
                string extractPath = System.Environment.CurrentDirectory + smartmanpath;
                try
                {
                    //解壓縮檔案
                    ZipFile.ExtractToDirectory(path, extractPath);
                }
                catch (Exception ex)
                {
                    textBoxStatus.Text = "解壓縮檔案失敗 " + filename;
                    result = false;
                }

            }
            return result;
        }

        public bool checkFileIsExeing()
        {
            bool result = true;
            //取得系統日期    
            string dt = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();

            //檢查檔案有沒有在使用中            
            foreach (UpdateFile file in updateFilesList)
            {
                string filename = file.filename;
                string smartmanpath = file.smartmanpath;
                string path = System.Environment.CurrentDirectory + smartmanpath + filename;
                if (filename.ToUpper().Contains(".EXE"))
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Move(path, path + ".bak" + dt);
                        }
                        catch (Exception ex)
                        {
                            textBoxStatus.Text = "檔案使用中 " + filename;
                            return false;
                        }
                    }
                }
                else
                {
                    if (File.Exists(path))//如果檔案存在改名字
                    {
                        File.Move(path, path + ".bak" + dt);
                    }
                }

            }
            return result;
        }


        public void checkFileDate()
        {
            foreach (UpdateFile file in updateUrlFilesList)
            {
                string filename = file.filename;
                string smartmanpath = file.smartmanpath;
                //將version轉成日期
                DateTime versionDate = DateTime.Parse(file.version);

                string filepath = System.Environment.CurrentDirectory + smartmanpath + filename;

                if (File.Exists(filepath))
                {
                    DateTime localFileDate = File.GetLastWriteTime(filepath);

                    if (localFileDate < versionDate)
                    {
                        updateFilesList.Add(file);
                    }
                }
                else
                {
                    updateFilesList.Add(file);
                }

                   

            }
        }

        public bool checkFileExsit()
        {
            bool isExist = true;
            foreach (UpdateFile file in updateUrlFilesList)
            {
                string urlpath = file.urlpath;
                //urlpath最右邊的一個/後面的字串
                string filename = urlpath.Substring(urlpath.LastIndexOf("/") + 1);
                if (!TestConnectToUrl(urlpath))
                {
                    textBoxStatus.Text = "無法取得檔案 " + filename;
                    isExist = false;
                    break;
                }
            }
            return isExist;
        }

        public void CreateLocalTempPath()
        {
            //建立 temp 資料夾
            if (!Directory.Exists(System.Environment.CurrentDirectory + localtemppath))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + localtemppath);
            }
            else
            {
                //刪除 temp 資料夾   
                DirectoryInfo di = new DirectoryInfo(System.Environment.CurrentDirectory + localtemppath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        public void downloadfile()
        {
            //建立 temp 資料夾或刪除 temp 資料夾裡的檔案
            CreateLocalTempPath();

            // 下載檔案 List<Files>.urlpath 到 Files.smartmanpath 存到本地 temp 資料夾

            foreach (UpdateFile file in updateUrlFilesList)
            {
                string urlpath = file.urlpath;
                string filename = urlpath.Substring(urlpath.LastIndexOf("/") + 1);
                string smartmanpath = file.smartmanpath;
                string path = System.Environment.CurrentDirectory + localtemppath;
                if (!File.Exists(path))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(urlpath, path +"\\" + filename);
                    }
                }
            }

        }

        public bool GetFileLists()
        {
            string url = connectURL1 + "/" + releaseFileName;
            updateUrlFilesList = GetUpdateFilesAsync(url);
            if (updateUrlFilesList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<UpdateFile> GetUpdateFilesAsync(string url)
        {
            using var httpClient = new HttpClient();
            try
            {
                // 使用 Result 属性来获取同步结果
                var json = httpClient.GetStringAsync(url).Result;
                var releaseInfo = JsonConvert.DeserializeObject<ReleaseInfo>(json);

                List<UpdateFile> updateFiles = releaseInfo.Files;
                return updateFiles;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
                textBoxStatus.Text = "release 發生錯誤";
                return new List<UpdateFile>(); // 返回一个空的 List<UpdateFile>
            }
        }


        public bool connectURL()
        {
            if (TestConnectToUrl(connectURL1))
            {
                isConnect = true;
                return true;
            }
            else
            {
                textBoxStatus.Text = "無法連到主機";
                isConnect = false;
                return false;
            }
        }

        public void Init()
        {
            // 初始化Timer
            timer = new Timer();
            timer.Interval = 1000; // 每1秒觸發一次
            timer.Tick += new EventHandler(OnTimerTick);
            timer.Start();
        }

        public bool Read_ini()
        {
            // 讀取ini檔
            string path = ConfigFileName;
            //取得目前執行檔的路徑
            string currentPath = System.Environment.CurrentDirectory + "\\" + ConfigFileName;

            // 檢查檔案是否存在
            if (!File.Exists(currentPath))
            {
                textBoxStatus.Text = "無設定檔";
                return false;
            }
            else
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("ID"))
                    {
                        ID = line.Replace(" ", "").ToUpper().Split("ID=")[1];
                        connectURL1 = connectURL1 + ID;
                    }
                }
                return true;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // 計算點的數量，最多3個，然後重置
            dotCount = (dotCount + 1) % 4;

            // 更新TextBox的文本，每次增加2個點
            textBoxStatus.Text = textBoxStatus.Text + new string('.', dotCount);

            if (dotCount >= 3)
            {
                textBoxStatus.Text = textBoxStatus.Text.Replace("..", "");
            }
        }


        public bool TestConnectToUrl(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置请求超时时间
                    client.Timeout = TimeSpan.FromSeconds(URLConnectTimeOutSec);

                    // 发送 GET 请求
                    //HttpResponseMessage response = await client.GetAsync(url);
                    // 发送 GET 请求并等待结果
                    HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();

                    // 检查响应状态码是否为 200 OK
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                // 捕获任何异常
                return false;
            }
        }

    }
}
