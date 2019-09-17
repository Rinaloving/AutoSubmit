
using BDCSubmit.Business;
using BDCSubmit.Business.BLL.QX;
using BDCSubmit.Business.BLL.SJ;
using BDCSubmit.Business.CommonClass;
using CCWin;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AutoSubmit
{
    public partial class AutoTask : CCSkinMain
    {
        private bool CanToClose = false;
        private bool IsRunning = false;

        public AutoTask()
        {
            InitializeComponent();
            //读取配置文件定时的时间间隔，配置文件的时间单位为 分钟，要转移为 毫秒
            try
            {



                XElement root = XElement.Load(SystemHandler.configFilePath);
                XElement interval = root.Element("GeneralConfig").Element("Interval");
                this.timer1.Interval = int.Parse(interval.Value) * 60 * 1000;
            }
            catch
            {
                this.timer1.Interval = 3600000;
            }
        }
        public void RefreshRoles()
        {
            this.tmiStart.Enabled = !this.IsRunning;
            this.tmiStop.Enabled = this.IsRunning;

        }
        private void AutoTask_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanToClose)
            {
                e.Cancel = true;
            }
            this.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!IsRunning) return;
            timer1.Tick -= timer1_Tick;
            timer1.Enabled = false;

            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                backgroundWorker1.ReportProgress(0, "本次任务开始时间:" + DateTime.Now.ToString());
                List<City> lstCitys = SystemHandler.Instance.GetConnectionCitys();
                foreach (var item in lstCitys)
                {
                    QXType qx = EnumHelper.GetByQXDM(item.CityCode);
                    if (qx == QXType.None)
                        continue;
                    backgroundWorker1.ReportProgress(0, " 执行:" + item.CityName + " 任务。" + DateTime.Now.ToString());
                    //if (qx.ToString() == "QY" || qx.ToString() == "XJ" || qx.ToString() == "PY") continue;
                    //1,数据检查
                    try
                    {

                        backgroundWorker1.ReportProgress(0, "   执行数据检查");
                        CheckDataHandler.Instance.CheckTables(qx, ExcuteTaskType.Hide);
                    }
                    catch (Exception ex)
                    {
                        backgroundWorker1.ReportProgress(0, ex.ToString());
                        LogClass.Instance.WriteLogFile(ex.ToString());
                    }
                    //2,数据同步
                    try
                    {
                        backgroundWorker1.ReportProgress(0, "   执行数据同步");

                        DataExchangeHandler.Instance.DataExchange(qx, ExcuteTaskType.Hide);

                    }
                    catch (Exception ex)
                    {
                        backgroundWorker1.ReportProgress(0, ex.ToString());
                        LogClass.Instance.WriteLogFile(ex.ToString());
                    }

                    //3,数据上报
                    try
                    {
                        backgroundWorker1.ReportProgress(0, "   执行数据上报");
                        DataSubmitHandler.Instance.DataSubmit(qx, ExcuteTaskType.Hide);
                    }
                    catch (Exception ex)
                    {
                        backgroundWorker1.ReportProgress(0, ex.ToString());
                        LogClass.Instance.WriteLogFile(ex.ToString());
                    }
                    //4,响应报文解析
                    try
                    {
                        backgroundWorker1.ReportProgress(0, "   执行响应报文解析");
                        ResponseHandler.Instance.ResponseData(qx, ResponseHandler.Instance.GetResponseFiles(qx), ExcuteTaskType.Hide);
                    }
                    catch (Exception ex)
                    {
                        backgroundWorker1.ReportProgress(0, ex.ToString());
                        LogClass.Instance.WriteLogFile(ex.ToString());
                    }

                }
            }

            catch (Exception ex)
            {
                backgroundWorker1.ReportProgress(0, ex.ToString());
                LogClass.Instance.WriteLogFile(ex.ToString());
            }
            finally
            {
                backgroundWorker1.ReportProgress(0, "本次任务结束时间:" + DateTime.Now.ToString());
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            richTextBox1.AppendText(e.UserState.ToString() + "\n");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.DoWork -= backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.ProgressChanged -= backgroundWorker1_ProgressChanged;

            timer1.Tick += timer1_Tick;
            timer1.Enabled = true;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void AutoTask_Load(object sender, EventArgs e)
        {
            try
            {
                //XElement root = XElement.Load(BDCSubmit.Business.SystemHandler.configFilePath);
                //timer1.Interval = int.Parse(root.Element("GeneralConfig").Element("Interval").Value) * 60 * 1000;

                //注释掉开机自启动功能
                //string path = Application.ExecutablePath;

                //RegistryKey rk = Registry.LocalMachine;

                //RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                //rk2.SetValue("AutoTask", path);

                //rk2.Close();
                //rk.Close();
            }
            catch
            {
                timer1.Interval = 3600000;
            }

        }

        private void tmiClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (scheduler != null)
                {
                    if (scheduler.IsStarted)
                        scheduler.Shutdown(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("上报日志任务异常:" + ex.ToString());
            }
            CanToClose = true;
            this.Close();
        }
        IScheduler scheduler;
        /// <summary>
        /// 任务是否暂停
        /// </summary>
        bool IsPauseAll = false;
        private void tmiStart_Click(object sender, EventArgs e)
        {

            this.IsRunning = true;
            RefreshRoles();
            //启动上报日志任务
            try
            {
                if (scheduler == null)
                {
                    string strtime = SystemHandler.Instance.GetLogConfig();
                    ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                    scheduler = schedulerFactory.GetScheduler() as Quartz.IScheduler;

                    IJobDetail job = null;

                    job = JobBuilder.Create<LogJobClass>()
                      .WithIdentity("LogJobClassJob", "LogJobClassJobGroup")
                      .Build();

                    if (strtime != null)
                    {
                        ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().WithCronSchedule(strtime).Build();
                        scheduler.ScheduleJob(job, trigger);
                        scheduler.Start();
                    }
                }
                else
                {
                    if (IsPauseAll)
                    {
                        scheduler.ResumeAll(); IsPauseAll = false;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            richTextBox1.AppendText("开启任务:" + DateTime.Now.ToString() + "\n");
            timer1_Tick(null, null);

        }

        private void tmiStop_Click(object sender, EventArgs e)
        {
            this.IsRunning = false;
            RefreshRoles();
            timer1.Stop();
            richTextBox1.AppendText("任务停止:" + DateTime.Now.ToString() + "\n");
            try
            {
                //暂停 上报日志任务
                if (scheduler != null)
                {
                    scheduler.PauseAll();
                    IsPauseAll = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("上报日志任务异常:" + ex.ToString());
            }
        }

        private void AutoTask_Shown(object sender, EventArgs e)
        {
            this.tmiStart.PerformClick();
        }


    }
}
