using BDCSubmit.Business.CommonClass;
using BDCSubmit.Business.SubmitModel;
using NPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BDCSubmit.Business.BLL.SJ
{
    public class ResponseHandler
    {
        #region 声明
        private static ResponseHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static ResponseHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new ResponseHandler();
                        }
                    }
                }
                return mInstance;
            }
        }

        private ResponseHandler()
        { }
        #endregion

  
        string qxdm = "";
        IDatabase db = null;
        IDatabase sjdb = null;
        /// <summary>
        /// 解析响应报文
        /// </summary>
        /// <param name="qx"></param>
        /// <param name="lstResponseFiles"></param>
        public void ResponseData(QXType qx, List<string> lstResponseFiles, ExcuteTaskType et)
        {
            ExeResponseData(qx, lstResponseFiles, et);
        }

        private void ExeResponseData(QXType qx, List<string> lstResponseFiles, ExcuteTaskType et)
        {
            if (qx == QXType.None) return;
            if (lstResponseFiles == null || lstResponseFiles.Count <= 0) return;
            int istatus = (int)STATUSType.Submiting;

            switch (qx)
            {
                case QXType.QY:
                    db = new Database("qysubmit");
                    qxdm = "371423";
                    break;
                case QXType.QH:
                    db = new Database("qhsubmit");
                    qxdm = "371425";
                    break;
                case QXType.PY:

                    db = new Database("pysubmit");

                    qxdm = "371426";
                    break;
                case QXType.XJ:

                    db = new Database("xjsubmit");

                    qxdm = "371427";
                    break;
                default:
                    break;
            }
            //ShowDialogForm sdf = null;
            try
            {
                List<BIZANDREP> lstR = sjdb.Fetch<BIZANDREP>(" where qxdm='" + qxdm + "' and status=" + istatus + "").ToList();
                XElement config = XElement.Load(SystemHandler.configFilePath);
                List<City> lstCity = SystemHandler.Instance.GetGeneralCitys();
                City city = lstCity.Where(p => qxdm.Equals(p.CityCode)).FirstOrDefault();
                List<string> lstFiles = lstResponseFiles;

                // add by cfl 2019年9月 8日
                string XJGetRepMsg = city.RepMsgPath + "/XJGetRepMsg";
                string reperrorpath = city.RepMsgPath.Trim().Replace("/RepMsg", "/RepMsgError"); //D:/dzxml/371423/RepMsgError/GetRepMsg
                if (!Directory.Exists(reperrorpath))
                {
                    Directory.CreateDirectory(reperrorpath);
                }
                string XJGetRepMsgError = reperrorpath + "/XJGetRepMsgError"; // D:/dzxml/371423/RepMsgError/GetRepMsg//XJGetRepMsgError
                if (!Directory.Exists(XJGetRepMsgError))
                {
                    Directory.CreateDirectory(XJGetRepMsgError);
                }


                if (!Directory.Exists(XJGetRepMsg))
                    Directory.CreateDirectory(XJGetRepMsg);
                if (lstFiles != null && lstFiles.Count > 0)
                {
                    int totalMain = lstFiles.Count;
                    if (et == ExcuteTaskType.Show)
                    {
                        //sdf = new ShowDialogForm("提示", "正在查找响应报文文件...", "请稍候");
                        //sdf.SetProgress(totalMain);
                    }
                    foreach (var item in lstFiles)
                    {
                        try
                        {

                            if (et == ExcuteTaskType.Hide){}
                            //sdf.SetMessage("正在获取响应报文:" + item + " 数据"); 

                                string bizmsgid = Path.GetFileNameWithoutExtension(item).Replace("Rep", "");
                                BIZANDREP bar = lstR.Where(p => bizmsgid.Equals(p.BIZMSGID)).FirstOrDefault();
                                if (bar != null)
                                {
                                    BIZANDREP qxbar = db.Fetch<BIZANDREP>("where YWH ='"+ bar.YWH+"'").FirstOrDefault();//区县
                                    if (qxbar != null)
                                    {
                                        XElement root = XElement.Load(item);
                                        string repCode = root.Element("ResponseCode").Value;
                                        switch (repCode)
                                        {
                                            case "0000":
                                                qxbar.STATUS = bar.STATUS = (int)STATUSType.RepSuccess;
                                                qxbar.REPTEXT = bar.REPTEXT = root.Element("ResponseInfo").Value;
                                                break;
                                            case "2040":
                                                qxbar.STATUS = bar.STATUS = (int)STATUSType.RepFailure;
                                                qxbar.REPTEXT = bar.REPTEXT = root.Element("ResponseInfo").Value;
                                                break;
                                            case "1000":
                                                qxbar.STATUS = bar.STATUS = (int)STATUSType.RepFailure;
                                                qxbar.REPTEXT = bar.REPTEXT = root.Element("AdditionalData").Value;
                                                break;
                                            default:
                                                qxbar.STATUS = bar.STATUS = (int)STATUSType.RepFailure;
                                                qxbar.REPTEXT = bar.REPTEXT = root.Element("ResponseInfo").Value;
                                                break;
                                        }
                                    }
                                    bar.REPFILEPATH = item;

                                    //更新市级库BIZANDREP
                                    sjdb.Update(bar);
                                    if (qxbar != null)
                                    {
                                        //更新区县库BIZANDREP
                                        db.Update(qxbar);
                                    }



                                    if (item.Contains("RepMsgError"))
                                    {
                                        File.Copy(item, XJGetRepMsgError + "/" + Path.GetFileName(item), true);
                                    }
                                    else
                                    {
                                        File.Copy(item, XJGetRepMsg + "/" + Path.GetFileName(item), true);
                                    }

                                    File.Delete(item);

                                }

                            }
                        
                        catch (Exception ex)
                        {
                                throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (sdf != null)
                //{
                //    sdf.Close(); sdf.Dispose();
                //}
            }
        }
        /// <summary>
        /// 返回有上报信息的响应报文文件完整路径
        /// </summary>
        /// <param name="qx"></param>
        /// <returns></returns>
        public List<string> GetResponseFiles(QXType qx)
        {
            if (qx == QXType.None) return null;
            List<string> lstResultFiles = new List<string>();
            try
            {
                int istatus = (int)STATUSType.Submiting;
                sjdb = new Database("sjsubmit");
                //SJContext = SystemHandler.Instance.SJCommunicationContext;
                switch (qx)
                {
                    case QXType.QY:

                        qxdm = "371423";
                        break;
                    case QXType.QH:

                        qxdm = "371425";
                        break;
                    case QXType.PY:

                        qxdm = "371426";
                        break;
                    case QXType.XJ:

                        qxdm = "371427";
                        break;
                    default:
                        break;
                }
                List<BIZANDREP> lstR = sjdb.Fetch<BIZANDREP>(" where qxdm='" + qxdm + "' and status=" + istatus + "").ToList();
                XElement config = XElement.Load(SystemHandler.configFilePath);
                List<City> lstCity = SystemHandler.Instance.GetGeneralCitys();
                City city = lstCity.Where(p => qxdm.Equals(p.CityCode)).FirstOrDefault();
                List<string> lstFiles = Directory.GetFiles(city.RepMsgPath, "Rep*.xml").ToList(); //D:/dzxml/371423/RepMsg/GetRepMsg
                string XJGetRepMsg = city.RepMsgPath + "/XJGetRepMsg";

                string reperrorpath = city.RepMsgPath.Trim().Replace("/RepMsg", "/RepMsgError"); //D:/dzxml/371423/RepMsgError/GetRepMsg
                string XJGetRepMsgError = reperrorpath + "/XJGetRepMsgError";


                if (!Directory.Exists(reperrorpath))
                {
                    Directory.CreateDirectory(reperrorpath);
                }
                List<string> errorlstFiles = Directory.GetFiles(reperrorpath, "Rep*.xml").ToList();
                List<string> totallstFiles = lstFiles.Union(errorlstFiles).ToList<string>();


                if (!Directory.Exists(XJGetRepMsgError))
                {
                    Directory.CreateDirectory(XJGetRepMsgError);
                }



                if (!Directory.Exists(XJGetRepMsg))
                    Directory.CreateDirectory(XJGetRepMsg);
                List<string> lstXJGetFiles = Directory.GetFiles(XJGetRepMsg, "Rep*.xml").ToList();
                List<string> errorlstXJGetFiles = Directory.GetFiles(XJGetRepMsgError, "Rep*.xml").ToList();

                List<string> lstXJGetFileName = new List<string>();
                lstXJGetFiles.ForEach(item => { lstXJGetFileName.Add(Path.GetFileNameWithoutExtension(item)); });
                errorlstXJGetFiles.ForEach(item => { lstXJGetFileName.Add(Path.GetFileNameWithoutExtension(item)); });
                if (lstXJGetFileName.Count > 0)
                {
                    totallstFiles.ForEach(item =>
                    {
                        if (!lstXJGetFileName.Contains(Path.GetFileNameWithoutExtension(item)))
                        {
                            string bizmsgid = Path.GetFileNameWithoutExtension(item).Replace("Rep", "");
                            BIZANDREP bar = lstR.Where(p => bizmsgid.Equals(p.BIZMSGID)).FirstOrDefault();
                            if (bar != null)
                                lstResultFiles.Add(item);
                        }
                    });
                }
                else
                {
                    totallstFiles.ForEach(item =>
                    {
                        string bizmsgid = Path.GetFileNameWithoutExtension(item).Replace("Rep", "");
                        BIZANDREP bar = lstR.Where(p => bizmsgid.Equals(p.BIZMSGID)).FirstOrDefault();
                        if (bar != null)
                            lstResultFiles.Add(item);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstResultFiles;
        }
    }
}
