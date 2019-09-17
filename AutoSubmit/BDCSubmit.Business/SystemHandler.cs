
using BDCSubmit.Business.CommonClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BDCSubmit.Business
{
    public class SystemHandler
    {
        #region 声明
        private static SystemHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static SystemHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new SystemHandler();
                        }
                    }
                }
                return mInstance;
            }
        }

        private SystemHandler()
        { }
        #endregion

        public static readonly string assPath = System.Windows.Forms.Application.StartupPath + "/BDCSubmit.Business.dll";

        public static readonly string configFilePath = System.Windows.Forms.Application.StartupPath + "/Config.xml";
        /// <summary>
        /// 检查业务表是否必选配置文件路径
        /// </summary>
        public static readonly string CheckBusinessTablesXMLFilePath = System.Windows.Forms.Application.StartupPath + "/CheckBusinessTables.xml";




        /// <summary>
        /// 获取4县连接配置
        /// </summary>
        /// <returns></returns>
        public List<City> GetConnectionCitys()
        {
            List<City> lst = new List<City>();
            try
            {
                if (File.Exists(configFilePath))
                {
                    XElement root = XElement.Load(configFilePath);
                    List<XElement> lstEl = root.Element("ConnectionConfig").Element("Citys").Elements("City").ToList();
                    lstEl.ForEach(p =>
                    {
                        lst.Add(new City
                        {
                            CityName = p.Attribute("CityName").Value,
                            CityCode = p.Attribute("CityCode").Value,
                            DataSource = p.Attribute("DataSource").Value,
                            Catalog = p.Attribute("Catalog").Value,
                            UserName = p.Attribute("UserName").Value,
                            UserPassword = p.Attribute("UserPassword").Value
                        }
                        );
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lst;
        }



        public List<City> GetGeneralCitys()
        {
            List<City> lst = new List<City>();
            try
            {
                if (File.Exists(configFilePath))
                {
                    XElement root = XElement.Load(configFilePath);
                    List<XElement> lstEl = root.Element("GeneralConfig").Element("Citys").Elements("City").ToList();
                    lstEl.ForEach(p =>
                    {
                        lst.Add(new City
                        {
                            CityName = p.Attribute("CityName").Value,
                            CityCode = p.Attribute("CityCode").Value,
                            BizMsgPath = p.Attribute("BizMsgPath").Value,
                            RepMsgPath = p.Attribute("RepMsgPath").Value,
                            LinuxBizMsgPath = p.Attribute("LinuxBizMsgPath").Value,
                            LinuxRepMsgPath = p.Attribute("LinuxRepMsgPath").Value
                        }
                        );
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lst;
        }

        public bool SaveGeneralCity(City city)
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    XElement root = XElement.Load(configFilePath);
                    List<XElement> lstEl = root.Element("GeneralConfig").Element("Citys").Elements("City").ToList();
                    foreach (var item in lstEl)
                    {
                        if (city.CityCode.Equals(item.Attribute("CityCode").Value))
                        {
                            item.Attribute("BizMsgPath").Value = city.BizMsgPath;
                            item.Attribute("RepMsgPath").Value = city.RepMsgPath;
                            item.Attribute("LinuxBizMsgPath").Value = city.LinuxBizMsgPath;
                            item.Attribute("LinuxRepMsgPath").Value = city.LinuxRepMsgPath;
                            root.Save(configFilePath);
                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetLogConfig()
        {
            string str = "0 0 20 * * ?";
            try
            {
                if (File.Exists(configFilePath))
                {
                    XElement root = XElement.Load(configFilePath);

                    XElement LogInterval = root.Element("GeneralConfig").Element("LogInterval");
                    if (LogInterval == null)
                    {
                        XElement logxel = new XElement("LogInterval", str);
                        root.Element("GeneralConfig").Add(logxel);
                        root.Save(configFilePath);
                        LogInterval = root.Element("GeneralConfig").Element("LogInterval");
                        //throw new Exception("配置文件:" + configFilePath + "节点GeneralConfig下未找到LogInterval"); ;
                    }
                    str = LogInterval.Value;
                }
                else
                {
                    throw new Exception("未找到文件配置文件:" + configFilePath + "无法读取配置");
                }
                return str;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SaveLogConfig(string str)
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    XElement root = XElement.Load(configFilePath);

                    XElement LogInterval = root.Element("GeneralConfig").Element("LogInterval");
                    if (LogInterval == null)
                    {
                        throw new Exception("配置文件:" + configFilePath + "节点GeneralConfig下未找到LogInterval"); ;
                    }
                    LogInterval.Value = str;
                    root.Save(configFilePath);
                }
                else
                {
                    throw new Exception("未找到文件配置文件:" + configFilePath + "无法读取配置");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
