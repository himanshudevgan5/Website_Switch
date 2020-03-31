using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BrowserSwitch
{
    public partial class BrowserForm : Form
    {
        private Timer myTimer;
        private int counter;
        private WebBrowser tabBrowser;
        private WebsiteList result;
        public List<Tab> tabs = new List<Tab>();
        private Tab tempTab;
        private DB database;
        private int waitTime;
        private Boolean dbWrite;
        private int counterEnd;
        private int LabelFontSize;

       [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;


        public BrowserForm()
        {
            InitializeComponent();
            Cursor.Hide();
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);

            //read in Config
            try
            {
                result = getWebsiteList("websites.xml");

                waitTime = int.Parse(ConfigurationManager.AppSettings["UpdateIntervall"]);
                dbWrite = Boolean.Parse(ConfigurationManager.AppSettings["WriteToDB"]);
                counterEnd = int.Parse(ConfigurationManager.AppSettings["DBCounter"]);
                LabelFontSize = int.Parse(ConfigurationManager.AppSettings["LabelFontSize"]);

                if (dbWrite)
                {
                    database = new DB(ConfigurationManager.AppSettings["mysqlUsername"], ConfigurationManager.AppSettings["mysqlPassword"], ConfigurationManager.AppSettings["mysqlHost"], ConfigurationManager.AppSettings["mysqlDatabase"]);
                }

            }
            catch (Exception e)
            {
                ShowWindow(hwnd, SW_SHOW);
                Cursor.Show();
                throw new System.Exception(e.Message);
            }

            int i = 0;


            foreach (Website website in result.Websites)
            {
                try
                {
                    Tab tab = new Tab(website);

                    tabBrowser = new WebBrowser();
                    tabBrowser.Name = i.ToString();
                    tabBrowser.ScriptErrorsSuppressed = true;
                    tabBrowser.Dock = DockStyle.Fill;
                    tabBrowser.TabIndex = 1;
                    tabBrowser.ScrollBarsEnabled = false;
                    tabBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);

                    if (tab.URLType.Equals("local"))
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "localSite\\" + website.URL;

                        if (File.Exists(file))
                        {
                            tab.URL = "file://" + file;
                        }
                        else
                        {
                            ShowWindow(hwnd, SW_SHOW);
                            throw new System.ArgumentException("URL location not found. (" + file + ", " + tab.PageTitle + ")");
                        }
                    }

                    if (tab.ErrorURLType.Equals("local"))
                    {
                        string file = AppDomain.CurrentDomain.BaseDirectory + "localSite\\" + website.ErrorURL;

                        if (File.Exists(file))
                        {
                            tab.ErrorURL = "file://" + file;
                        }
                        else
                        {
                            ShowWindow(hwnd, SW_SHOW);
                            throw new System.ArgumentException("URL location not found. (" + file + ", " + tab.PageTitle);
                        }
                    }

                    Uri websiteUri = new Uri(tab.URL);
                    tabBrowser.Url = websiteUri;

                    tab.webobject = tabBrowser;

                    TabPage WebsiteTabPage = new TabPage();

                    Font labelFont = new Font(FontFamily.GenericSansSerif, LabelFontSize);

                    //define the seconds label

                    if (result.Websites.Count > 1)
                    {
                         Label secondsLabel = new Label();
                        secondsLabel.AutoSize = true;
                        secondsLabel.TabIndex = 2;
                        secondsLabel.Font = labelFont;
                        secondsLabel.Location = new System.Drawing.Point(1122, 3);
                        secondsLabel.Parent = tabBrowser;
                        secondsLabel.Dock = DockStyle.Right;
                        secondsLabel.BackColor = Color.Red;

                        tab.labelSeconds = secondsLabel;

                    }
                    Label websiteCountLabel = new Label();
                    websiteCountLabel.AutoSize = true;
                    websiteCountLabel.Text = (tabs.Count + 1).ToString() + "/" + result.Websites.Count;
                    websiteCountLabel.TabIndex = 2;
                    websiteCountLabel.Font = labelFont;
                    websiteCountLabel.BackColor = Color.Red;
                    websiteCountLabel.Location = new System.Drawing.Point(1122, 3);
                    websiteCountLabel.Parent = tabBrowser;
                    websiteCountLabel.Dock = DockStyle.Right;

                    tab.labelCount = websiteCountLabel;

                    WebsiteTabPage.AutoScroll = false;
                    WebsiteTabPage.Controls.Add(tabBrowser);
                    tabControl.TabPages.Add(WebsiteTabPage);

                    tab.tabobject = WebsiteTabPage;

                    tabs.Add(tab);
                    i++;
                }
                catch (Exception e)
                {
                    Cursor.Show();
                    throw new System.Exception(e.Message + " (" + website.ID + "; " + website.PageTitle + ")");
                }

            }




        }

        /// <summary>
        /// Actualizes the time passed on the webpage/tag
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            tempTab.labelSeconds.Text = counter++.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static WebsiteList getWebsiteList(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WebsiteList));
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            {
                return (WebsiteList)serializer.Deserialize(fileStream);

            }
        }

        public async Task actualizePage()
        {

            if (dbWrite)
            {
                database.writeStatusToDB();
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

            int i = 0;
            int counterWriteDB = 0;
            tempTab = tabs[i];

            while (true)
            {

                if (i >= tabs.Count)
                {
                    i = 0;
                    counterWriteDB++;

                    if (dbWrite && counterWriteDB == counterEnd)
                    {
                        database.writeStatusToDB();
                        counterWriteDB = 0;
                    }
                }

                tempTab.labelSeconds.Text = "";
                tabControl.SelectedTab = tabs[i].tabobject;
                tempTab = tabs[i];

                    if (tabs[i].webobject.DocumentTitle.ToLower().Contains("error"))
                {
                    Uri errorURI = new Uri(tempTab.ErrorURL);
                    tempTab.webobject.Navigate(errorURI);

                    if (tempTab.checkBrowser == null)
                    {
                        tempTab.checkBrowser = new WebBrowser();
                        tempTab.checkBrowser.ScriptErrorsSuppressed = true;
                        tempTab.checkBrowser.Visible = false;
                        tempTab.checkBrowser.TabIndex = 1;
                        tempTab.checkBrowser.Url = new Uri(tempTab.URL);
                    }

                }
                else if (tempTab.checkBrowser != null && !tempTab.checkBrowser.DocumentTitle.ToLower().Contains("error"))
                {
                    tempTab.webobject.Navigate(new Uri(tempTab.URL));
                    tempTab.checkBrowser = null;
                }
                else if (tempTab.checkBrowser == null && !tabs[i].webobject.DocumentTitle.ToLower().Contains(tabs[i].PageTitle.ToLower()))
                {
                    tempTab.webobject.Refresh();
                }

                counter = 1;
                myTimer = new Timer();
                if (tabs.Count > 1)
                {
                    myTimer.Tick += new EventHandler(TimerEventProcessor);
                }
                myTimer.Interval = 1000;
                myTimer.Start();
                await Task.Delay(TimeSpan.FromSeconds(waitTime));
                myTimer.Stop();
                myTimer.Enabled = false;

                i++;
            }


        }

        private void DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser web = ((WebBrowser)sender);

            int i = Int32.Parse(web.Name);

            if (tabs[i].checkBrowser == null)
            {
                web.Document.Body.Style = "zoom:" + tabs[i].ZoomLevel.ToString() + "%;";
            }
            else
            {
                web.Document.Body.Style = "zoom:" + tabs[i].ZoomLevelErrorURL.ToString() + "%;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserForm_Shown(object sender, EventArgs e)
        {
            actualizePage();
        }

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
        }

    }
}
