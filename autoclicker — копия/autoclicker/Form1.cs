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





        private void EnterOnSite()
        {
            Brow = new OpenQA.Selenium.Chrome.ChromeDriver();

            Brow.Manage().Window.Maximize();
            Brow.Navigate().GoToUrl("https://bitrix24.net.ua/");

            //selivanovsky.i
            //    silvergraf


            IWebElement log = Brow.FindElement(By.Name("USER_LOGIN"));
            log.SendKeys(login);
            IWebElement pas = Brow.FindElement(By.Name("USER_PASSWORD"));
            pas.SendKeys(pasword + OpenQA.Selenium.Keys.Enter);
            System.Threading.Thread.Sleep(200);

            Brow.Close();
            Brow.Dispose();

        }


        private void button1_Click(object sender, EventArgs e)
        {
           
            if ((int)DateTime.Now.DayOfWeek > 0 && (int)DateTime.Now.DayOfWeek < 6)
                EnterOnSite();
            
        }
        string login, pasword, timemorn, timeeven;
        private string PathParams = @"bt.xml";

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
                pasword = setting.Root.Element("Pass").Value;
                timemorn = setting.Root.Element("time1").Value;
                timeeven = setting.Root.Element("time2").Value;

                ld = new List<DailyTimer>();
                d = new DailyTimer();
                d1 = new DailyTimer();
                d.T1 = TimeSpan.Parse(timemorn);
                d.TB = d.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;
                d1.T1 = TimeSpan.Parse(timeeven);
                d1.TB = d1.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;

                ld.Add(d);
                ld.Add(d1);
                timeNow.Text = "time: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                label4.Text = d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today";

            }
            else
            {
                setting = new XDocument(
                    new XElement("Root",
                    new XElement("User", " "),
                    new XElement("Pass", " "),
                    new XElement("time1", "08:00 "),
                    new XElement("time2", "17:00 ")
                    
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

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeNow.Text = "time: " + DateTime.Now.Hour+":"+ DateTime.Now.Minute;

            label4.Text = d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today";
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
                   new XElement("Pass", PasText.Text),
                   new XElement("time1", maskedTextBox1.Text),
                   new XElement("time2", maskedTextBox2.Text)
               ));
            setting.Save(PathParams);
            LoadParams(PathParams);
            LoginText.Text = "";
            PasText.Text = "";
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";


        }

        List<DailyTimer> ld;
        DateTime dateLast;
        private void Form1_Load(object sender, EventArgs e)
        {           
            LoadParams(PathParams);                      
           
            timeNow.Text = "time: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            label4.Text = d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today";
            dateLast = DateTime.Now;
          
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
