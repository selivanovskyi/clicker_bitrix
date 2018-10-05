using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace autoclicker
{

    public struct alarms
    {

    }
         
    public partial class Form1 : Form
    {
        IWebDriver Brow;
        

        

        public Form1()
        {
            InitializeComponent();
        }



        string login, pasword, timemorn, timeeven;
        private string PathParams = @"bt.xml";
        int selectBrowser = 0;

        private void EnterOnSite()
        {
            if (selectBrowser == 0)
            {
                var serv = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService();
                serv.HideCommandPromptWindow = true;
                var options = new OpenQA.Selenium.Chrome.ChromeOptions();
                if (!visBr.Checked)
                {
                    options.AddArgument("--window-position=-32000,-32000");
                }



                //  Brow.SwitchTo
                Brow = new OpenQA.Selenium.Chrome.ChromeDriver(serv, options); 
            }
            else
            {
                var serv = OpenQA.Selenium.Firefox.FirefoxDriverService.CreateDefaultService();// Chrome.ChromeDriverService.CreateDefaultService();
                serv.HideCommandPromptWindow = true;
                var options = new OpenQA.Selenium.Firefox.FirefoxOptions();
                if (!visBr.Checked)
                {
                    options.AddArgument("--window-position=-32000,-32000");
                }


                var pos = !visBr.Checked? new Point(32000, 32000): new Point(0, 0);

                Brow = new OpenQA.Selenium.Firefox.FirefoxDriver(serv,options, TimeSpan.FromSeconds(30));
                Brow.Manage().Window.Position = pos;

            }

            //Brow.Manage().Timeouts().ImplicitlyWait( ();
            //Brow.Manage().Window.Size.Heig
            WebDriverWait wait = new WebDriverWait (Brow,TimeSpan.FromSeconds(10));
            //WebDriverWait wait = new WebDriverWait(driver, 10);
            Brow.Navigate().GoToUrl("https://bitrix24.net.ua/");

            //selivanovsky.i
            //    silvergraf


            IWebElement log = wait.Until(ExpectedConditions.ElementIsVisible( By.Name("USER_LOGIN")));
            log.SendKeys(login);
            IWebElement pas = Brow.FindElement(By.Name("USER_PASSWORD"));
            pas.SendKeys(pasword + OpenQA.Selenium.Keys.Enter);
            System.Threading.Thread.Sleep(selectBrowser==0?700:5000);

            Brow.Close();
            Brow.Dispose();
            Loger.SetLog(login);

        }


        private void button1_Click(object sender, EventArgs e)
        {

            if ((int)DateTime.Now.DayOfWeek > 0 && (int)DateTime.Now.DayOfWeek < 6)
            {
                //Thread workerThread = new Thread(EnterOnSite);
                //workerThread.Start();
                EnterOnSite();
            }
        }
        

        private void LoginText_TextChanged(object sender, EventArgs e)
        {

        }


        void LoadParams(string XMLFile)
        {
            XDocument setting;
            if (File.Exists(XMLFile))
            {
                setting = XDocument.Load(XMLFile);
                login = setting.Root.Element("User").Value;
                pasword = Crypt.Decrypt(setting.Root.Element("Pass").Value);
                timemorn = setting.Root.Element("time1").Value;
                timeeven = setting.Root.Element("time2").Value;
                selectBrowser = int.Parse(setting.Root.Element("brows").Value);

                ld = new List<DailyTimer>();
                d = new DailyTimer();
                d1 = new DailyTimer();
                try
                {
                    d.T1 = TimeSpan.Parse(timemorn);
                    d.TB = d.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;
                    d1.T1 = TimeSpan.Parse(timeeven);
                    d1.TB = d1.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;
                }
                catch { }
                ld.Add(d);
                ld.Add(d1);
                timeNow.Text = "time: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                label4.Text = "time next: " + (d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today");

            }
            else
            {
                setting = new XDocument(
                    new XElement("Root",
                    new XElement("User", " "),
                    new XElement("Pass", " "),
                    new XElement("time1", "08:00 "),
                    new XElement("time2", "17:00 "),
                    new XElement("brows", "0")

                ));
                timemorn = "08:00 ";
                timeeven = "17:00 ";

                setting.Save(XMLFile);
                MessageBox.Show("не заполнен файл bt.xml");
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }

        }
       

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeNow.Text = "time: " + DateTime.Now.Hour+":"+ DateTime.Now.Minute;

            label4.Text = "time next: " + (d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today");
            check();
        }

        DailyTimer d;
        DailyTimer d1;

        private void button2_Click(object sender, EventArgs e)
        {
            XDocument setting;
            setting = new XDocument(
                   new XElement("Root",
                   new XElement("User", LoginText.Text),
                   new XElement("Pass", Crypt.Encrypt(PasText.Text)),
                   new XElement("time1", maskedTextBox1.Text),
                   new XElement("time2", maskedTextBox2.Text),
                   new XElement("brows", comboBox1.SelectedIndex)
               ));
            setting.Save(PathParams);
            LoadParams(PathParams);
            LoginText.Text = "";
            PasText.Text = "";
            maskedTextBox1.Text = "00:00";
            maskedTextBox2.Text = "00:00";


        }

        List<DailyTimer> ld;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                PasText.PasswordChar = '*';
            }
            else
            {
                PasText.PasswordChar = '\0';
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           

        
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectBrowser = comboBox1.SelectedIndex ;
        }

        DateTime dateLast;
        private void Form1_Load(object sender, EventArgs e)
        {
           
            LoadParams(PathParams);
            comboBox1.SelectedIndex = selectBrowser;

            timeNow.Text = "time: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            try
            {
                label4.Text = "time next: "  + (d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today");
            }
            catch { }
            dateLast = DateTime.Now;
            Loger.Path_logs = Directory.GetCurrentDirectory();
            
          
        }
        public void check()
        {          
            foreach (var td in ld)
            {
              if (dateLast.Date != DateTime.Now.Date)
              {
                        td.TB = true;
              }
              if (td.TB)
                {
                    if (td.T1.Hours ==DateTime.Now.Hour && td.T1.Minutes == DateTime.Now.Minute)
                    {
                        EnterOnSite();
                        td.TB = false;
                        Loger.SetLog(login);
                    }
                }
            }
            dateLast = DateTime.Now;


        }
    }
}
