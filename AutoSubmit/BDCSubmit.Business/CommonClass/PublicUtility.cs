using BDCSubmit.Business.SubmitModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BDCSubmit.Business.CommonClass
{
    public class BizMode { }

    public static class SubmitUtility
    {
        public static string Getyymmddhhmmss()
        {
            DateTime dt = DateTime.Now;
            return dt.Year.ToString().Substring(2) + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        }
    }

    /// <summary>
    /// 常数定义类
    /// </summary>
    public static class ConstClass
    {
        /// <summary>
        /// 权利类型字典表CodeNo
        /// </summary>
        public const string QLLXCodeNo = "001";
        /// <summary>
        /// 面积单位字典表CodeNo
        /// </summary>
        public const string MJDWCodeNo = "002";
        /// <summary>
        /// 权利性质字典表CodeNo
        /// </summary>
        public const string QLXZCodeNo = "003";
        /// <summary>
        /// 房屋用途字典表CodeNo
        /// </summary>
        public const string FWYTCodeNo = "017";
        /// <summary>
        /// 房屋类型字典表CodeNo
        /// </summary>
        public const string FWLXCodeNo = "018";
        /// <summary>
        /// 房屋性质字典表CodeNo
        /// </summary>
        public const string FWXZCodeNo = "019";
        /// <summary>
        /// 登记类型字典表CodeNo
        /// </summary>
        public const string DJLXCodeNo = "021";
        /// <summary>
        /// 权利人类型字典表CodeNo
        /// </summary>
        public const string QLRLXCodeNo = "036";


    }
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class ExtendClass
    {
        /// <summary>
        /// 传入string类型对象，如果对象为null,返回空字符串，否则返回原值.Trim()
        /// </summary>
        /// <param name="str">string类型对象</param>
        /// <returns></returns>
        public static string toStringEX(this string str)
        {
            if (str == null)
                return "";
            else
                return str.Trim();

        }
        /// <summary>
        ///  传入object类型对象，如果对象为null,返回空字符串，否则返回原值.toString().Trim()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string toStringEX(this object obj)
        {
            if (obj == null)
                return "";
            else
                return obj.ToString().Trim();
        }

        public static DescriptionAttribute ToDescription<T>(this T myEnum)
        {
            DescriptionAttribute da = null;
            Type type = typeof(T);
            FieldInfo info = type.GetField(myEnum.ToString());
            DescriptionAttribute descriptionAttribute = info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
            if (descriptionAttribute != null)
                da = descriptionAttribute;
            return da;

        }
    }

    /// <summary>
    /// 对象转换类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ConvertObject
    {
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        public static T ConvertObject2Target<T>(object source) where T : class
        {
            Type typeT = typeof(T);
            T objTarget = typeT.InvokeMember(null, BindingFlags.DeclaredOnly | BindingFlags.Public |
                       BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null) as T;
            try
            {
                PropertyInfo[] pTarget = typeT.GetProperties();
                object tValue = null;
                PropertyInfo tmpP = null;
                foreach (var item in pTarget)
                {
                    try
                    {
                        string typename = item.PropertyType.ToString();
                        tmpP = source.GetType().GetProperty(item.Name);
                        if (tmpP != null)
                        {
                            tValue = tmpP.GetValue(source, null);
                            //if (tValue != null)
                            //{
                            //    item.SetValue(objTarget, tValue, null);
                            //}
                            if (string.IsNullOrEmpty(tValue.toStringEX()))
                            {
                                //switch (typename)
                                //{
                                //    case "System.Int16":
                                //    case "System.Int32":
                                //    case "System.Int64":

                                //        item.SetValue(objTarget, 0, null);
                                //        break;
                                //    case "System.Decimal":
                                //        item.SetValue(objTarget, 0.0M, null);
                                //        break;
                                //    case "System.DateTime":
                                //        item.SetValue(objTarget, DateTime.MinValue, null);
                                //        break;
                                //    case "System.String":
                                //        item.SetValue(objTarget, "", null);
                                //        break;
                                //    case "System.Byte[]":
                                //        item.SetValue(objTarget, null, null);
                                //        break;
                                //    default:
                                //        item.SetValue(objTarget, tValue.toStringEX(), null);
                                //        break;
                                //}
                            }
                            else
                            {

                                switch (typename)
                                {
                                    case "System.Int32":

                                        item.SetValue(objTarget, Convert.ToInt32(tValue.toStringEX()), null);
                                        break;
                                    case "System.Int64":
                                        item.SetValue(objTarget, Convert.ToInt64(tValue.toStringEX()), null);
                                        break;
                                    case "System.Decimal":
                                    case "System.Nullable`1[System.Decimal]":
                                        item.SetValue(objTarget, Convert.ToDecimal(tValue.toStringEX()), null);
                                        break;
                                    case "System.DateTime":
                                        item.SetValue(objTarget, Convert.ToDateTime(tValue.toStringEX()), null);
                                        break;
                                    case "System.String":
                                        item.SetValue(objTarget, tValue.toStringEX(), null);
                                        break;
                                    default:
                                        item.SetValue(objTarget, tValue.toStringEX(), null);
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception)
            {


            }
            return objTarget;
        }
        /// <summary>
        /// DataTable转换成指定对象类 型
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable2Target<T>(DataTable dt) where T : class
        {
            List<T> lst = new List<T>();
            if (dt == null) return lst;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lst.Add(ConvertDataRow2Target<T>(dt.Rows[i]));
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lst;
        }
        /// <summary>
        /// DataRow转换成指定对象类型
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertDataRow2Target<T>(DataRow dr) where T : class
        {
            if (dr == null) return default(T);
            Type typeT = typeof(T);
            T objTarget = typeT.InvokeMember(null, BindingFlags.DeclaredOnly | BindingFlags.Public |
                       BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null) as T;
            try
            {
                PropertyInfo[] pTarget = typeT.GetProperties();
                object tValue = null;

                foreach (var item in pTarget)
                {
                    try
                    {

                        string typename = item.PropertyType.ToString();
                        if (dr.Table.Columns.Contains(item.Name))
                        {
                            tValue = dr[item.Name];
                            //if (tValue != null)
                            //{
                            //    item.SetValue(objTarget, tValue, null);
                            //}

                            if (string.IsNullOrEmpty(tValue.toStringEX()))
                            {
                                //switch (typename)
                                //{
                                //    case "System.Int16":
                                //    case "System.Int32":
                                //    case "System.Int64":
                                //        item.SetValue(objTarget, 0, null);
                                //        break;
                                //    case "System.Decimal":
                                //        item.SetValue(objTarget, 0.0M, null);
                                //        break;
                                //    case "System.DateTime":
                                //        item.SetValue(objTarget, DateTime.MinValue, null);
                                //        break;
                                //    case "System.String":
                                //        item.SetValue(objTarget, "", null);
                                //        break;
                                //    case "System.Byte[]":
                                //        item.SetValue(objTarget, null, null);
                                //        break;
                                //    default:
                                //        item.SetValue(objTarget, tValue.toStringEX(), null);
                                //        break;
                                //}
                            }
                            else
                            {

                                switch (typename)
                                {
                                    case "System.Int32":
                                        item.SetValue(objTarget, Convert.ToInt32(tValue.toStringEX()), null);
                                        break;
                                    case "System.Int64":
                                        item.SetValue(objTarget, Convert.ToInt64(tValue.toStringEX()), null);
                                        break;
                                    case "System.Decimal":
                                    case "System.Nullable`1[System.Decimal]":
                                        item.SetValue(objTarget, Convert.ToDecimal(tValue.toStringEX()), null);
                                        break;
                                    case "System.DateTime":
                                        item.SetValue(objTarget, Convert.ToDateTime(tValue.toStringEX()), null);
                                        break;
                                    case "System.String":
                                        item.SetValue(objTarget, tValue.toStringEX(), null);
                                        break;
                                    default:
                                        item.SetValue(objTarget, tValue.toStringEX(), null);
                                        break;
                                }
                            }


                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception)
            {


            }
            return objTarget;
        }

        /// <summary>
        /// 复制相同名称字段值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        //public static void CopySameField(object source, object target)
        //{
        //    if (source == null || target == null) return;
        //    PropertyInfo[] pi = target.GetType().GetProperties();
        //    object tValue = null;
        //    foreach (var item in pi)
        //    {
        //        SmartMap.DataClient.Model.ColumnAttribute ca = item.GetCustomAttributes(typeof(SmartMap.DataClient.Model.ColumnAttribute), false).FirstOrDefault() as SmartMap.DataClient.Model.ColumnAttribute;
        //        if (ca == null) continue;

        //        string typename = item.PropertyType.ToString();
        //        PropertyInfo tmpP = source.GetType().GetProperty(item.Name);
        //        if (tmpP != null)
        //        {
        //            tValue = tmpP.GetValue(source, null);

        //            if (string.IsNullOrEmpty(tValue.toStringEX()))
        //            {

        //            }
        //            else
        //            {

        //                switch (typename)
        //                {
        //                    case "System.Int32":

        //                        item.SetValue(target, Convert.ToInt32(tValue.ToString()), null);
        //                        break;
        //                    case "System.Int64":
        //                        item.SetValue(target, Convert.ToInt64(tValue.ToString()), null);
        //                        break;
        //                    case "System.Decimal":
        //                    case "System.Nullable`1[System.Decimal]":
        //                        item.SetValue(target, Convert.ToDecimal(tValue.ToString()), null);
        //                        break;
        //                    case "System.DateTime":
        //                        item.SetValue(target, Convert.ToDateTime(tValue.ToString()), null);
        //                        break;
        //                    case "System.String":
        //                        item.SetValue(target, tValue.toStringEX(), null);
        //                        break;
        //                    default:
        //                        item.SetValue(target, tValue, null);
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //}
    }
    /// <summary>
    /// 日志文件类
    /// </summary>
    public class LogClass
    {


        private static LogClass mInstance = null;

        private static readonly object lockAssistant = new object();

        public static LogClass Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new LogClass();
                        }
                    }
                }
                return mInstance;
            }
        }

        private LogClass()
        { }

        public readonly string logPath = System.Windows.Forms.Application.StartupPath;
        /// <summary>
        /// 写日志(追加到上次日志下一行)
        /// </summary>
        /// <param name="logText">日志内容</param>
        public void WriteLogFile(string logText)
        {

            string fname = logPath + "\\LogFile.log";


            FileInfo finfo = new FileInfo(fname);

            if (!finfo.Exists)
            {
                FileStream fs;
                fs = File.Create(fname);
                fs.Close();
                finfo = new FileInfo(fname);
            }


            if (finfo.Length > 1024 * 1024 * 10)
            {
                DateTime dtnow = DateTime.Now;
                string newPath = System.Windows.Forms.Application.StartupPath + "\\" + dtnow.ToString("yyyyMMddHHmmss") + "之前LogFile";
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                File.Move(fname, newPath + "\\LogFile.log");

            }


            FileStream fs2 = finfo.OpenWrite();
            {

                StreamWriter w = new StreamWriter(fs2);

                w.BaseStream.Seek(0, SeekOrigin.End);

                w.WriteLine("{0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());

                w.WriteLine(logText);
                w.WriteLine("------------------------------------");
                w.Flush();
                w.Close();
            }
            fs2.Close(); fs2.Dispose(); fs2 = null;

        }
    }

    /// <summary>
    /// 应用程序配置文件操作函数。可操作App.config。
    /// </summary>
    public static class AppConfigHelper
    {

        /// <summary>
        /// 取连接字符串的口令。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string GetPassword(string connectionstring)
        {
            return RegexValidHelper.MatchPassword(connectionstring);
            //Regex r = new Regex(@"Password=(?<passwsd>\w+);?");
            //return r.Match(connectionstring).Result("${passwsd}");
        }

        public static string GetSQLServer2005Password(string connectionstring)
        {
            return RegexValidHelper.MatchSQLServer2005Password(connectionstring);
        }
        public static string GetSQLServer2005UserId(string connectionstring)
        {
            return RegexValidHelper.MatchSQLServer2005UserId(connectionstring);
        }

        /// <summary>
        /// 取连接字符串的用户名。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string GetUserId(string connectionstring)
        {
            //Regex r = new Regex(@"User Id=(?<userid>\w+);*");
            //return r.Match(connectionstring).Result("${userid}");
            return RegexValidHelper.MatchUserId(connectionstring);
        }


        /// <summary>
        /// 取连接字符串的Data Source。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string GetDataSource(string connectionstring)
        {
            //Regex r = new Regex(@"^Data Source=(?<datasource>;*\w+)");
            //return r.Match(connectionstring).Result("${datasource}");
            return RegexValidHelper.MatchDataSource(connectionstring);
        }

        public static string GetInitialCatalog(string connectionstring)
        {
            return RegexValidHelper.MatchInitialCatalog(connectionstring);
        }

        /// <summary>
        /// 取当前配置文件的连接字符串
        /// </summary>
        /// <param name="key">[connectionStrings]的key</param>
        /// <returns></returns>
        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// 取当前配置文件设置键值。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 在App.config的appSettings中添加新键值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool AddSettings(string key, string value)
        {
            if (key == "") return false;
            // Open App.Config of executable
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            config.AppSettings.Settings.Add(key, value);

            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }

        /// <summary>
        /// 更新设置值后保存app.config
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool SaveSettings(string key, string value)
        {
            if (key == "") return false;
            // Open App.Config of executable
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            bool found = false;
            for (int i = 0; i < config.AppSettings.Settings.AllKeys.Length; i++)
            {
                if (config.AppSettings.Settings.AllKeys[i] == key)
                {
                    found = true;
                }
            }
            if (!found) return false;
            config.AppSettings.Settings[key].Value = value;
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }

        /// <summary>
        /// 保存App.config中connectionStrings的连接字符串
        /// </summary>
        /// <param name="connectkey">关键字</param>
        /// <param name="connectionstrings">连接字符串</param>
        /// <returns></returns>
        public static bool SaveConnectionString(string connectkey, string connectionstrings)
        {
            // Open App.Config of executable
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            bool found = false;
            for (int i = 0; i < config.ConnectionStrings.ConnectionStrings.Count; i++)
            {
                if (config.ConnectionStrings.ConnectionStrings[i].Name == connectkey)
                {
                    found = true;
                }
            }
            if (!found) return false;
            config.ConnectionStrings.ConnectionStrings[connectkey].ConnectionString = connectionstrings;
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("connectionStrings");
            return true;
        }

        /// <summary>
        /// System.Data.OracleClient，Oracle9i连接字符串示例
        /// Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;
        /// </summary>
        public static string SampleOracle9iConnectionStrings
        {
            get { return "Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;"; }
        }


        /// <summary>
        /// System.Data.SqlClient，SQLServer 2000 连接字符串示例
        /// Data Source=172.18.7.71;Initial Catalog=Map090320;User ID=sa;Password=
        /// </summary>
        public static string SampleSQLServer2000ConnectionStrings
        {
            get { return "Data Source=172.18.7.71;Initial Catalog=Map090320;User ID=sa;Password="; }
        }


        public static bool IsWindowsSSPI(string connectionstring)
        {
            return RegexValidHelper.IsWindowsSSPI(connectionstring);
        }
    }

    /// <summary>
    /// 常用正则表达式验证、匹配、替换操作
    /// </summary>
    public static class RegexValidHelper
    {
        /// <summary>
        /// 匹配正浮点数
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidPositiveFloatNumber(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
        }

        public static bool IsValidNumber(string strIn)
        {
            return Regex.IsMatch(strIn, @"^-?\d+$");
        }
        /// <summary>
        /// 验证电子邮件地址格式是否正确。源自msdn。
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 匹配：从 URL 提取协议和端口号。例如，“http://www.contoso.com:8080/letters/readme.html”将返回“http:8080”。
        /// 源自：msdn。
        /// </summary>
        /// <param name="url">url,如,http://www.google.com:5051</param>
        /// <returns>如http:8080</returns>
        public static String MatchProtocolPort(String url)
        {
            Regex r = new Regex(@"^(?<proto>\w+)://[^/]+?(?<port>:\d+)?/", RegexOptions.Compiled);
            Match m = r.Match(url);
            if (m.Success) return m.Result("${proto}${port}");
            else return string.Empty;
        }

        /// <summary>
        /// 验证日期格式是否正确。按年月日。
        /// 可以验证的日期格式：[2004-2-29], [2004-02-29 10:29:39 pm], [2004/12/31] 
        /// </summary>
        /// <param name="datestring">日期字符串</param>
        /// <returns></returns>
        public static bool IsValidDate(string datestring)
        {
            string pattern = @"^((\d{2}(([02468][048])|([13579][26]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|([1-2][0-9])))))|(\d{2}(([02468][1235679])|([13579][01345789]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|(1[0-9])|(2[0-8]))))))(\s(((0?[1-9])|(1[0-2]))\:([0-5][0-9])((\s)|(\:([0-5][0-9])\s))([AM|PM|am|pm]{2,2})))?$";
            return Regex.IsMatch(datestring, pattern);
        }

        /// <summary>
        /// 匹配国内电话号码：\d{3}-\d{8}|\d{4}-\d{7}。匹配形式如 0511-4405222 或 021-87888822
        /// </summary>
        /// <param name="phoneno"></param>
        /// <returns></returns>
        public static bool IsValidPhoneNo(string phoneno)
        {
            return Regex.IsMatch(phoneno, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }

        /// <summary>
        /// 匹配IPV4的IP地址。提取ip地址时有用
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsValidIP(string ip)
        {
            return Regex.IsMatch(ip, @"(?<![\d\.])((25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)\.){3}(25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)(?![\d\.])");
        }

        /// <summary>
        /// 15位身份证的验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidIdentifyCard15(string input)
        {
            return Regex.IsMatch(input, @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$");
        }


        /// <summary>
        /// 18位身份证的验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidIdentifyCard18(string input)
        {
            return Regex.IsMatch(input, @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$");
        }

        /// <summary>
        /// 手机号码验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidMobile(string input)
        {
            return Regex.IsMatch(input, @"^((\(\d{2,3}\))|(\d{3}\-))?13\d{9}$");
        }

        /// <summary>
        /// 取连接字符串的口令。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string MatchPassword(string connectionstring)
        {
            Regex r = new Regex(@"Password=(?<passwsd>\w+);?");
            return r.Match(connectionstring).Result("${passwsd}");
        }


        /// <summary>
        /// 取连接字符串的用户名。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string MatchUserId(string connectionstring)
        {
            Regex r = new Regex(@"User Id=(?<userid>\w+);*");
            return r.Match(connectionstring).Result("${userid}");
        }


        /// <summary>
        /// 取连接字符串的Data Source。适用于System.Data.OracleClient标准连接字符串。
        /// </summary>
        /// <param name="connectionstring">Example:Data Source=yichun;User Id=gtdj;Password=123;Integrated Security=no;</param>
        /// <returns>string</returns>
        public static string MatchDataSource(string connectionstring)
        {
            Regex r = new Regex(@"^Data Source=(?<datasource>;*\w+)");
            return r.Match(connectionstring).Result("${datasource}");
        }

        /// <summary>
        /// 匹配是否是安全连接
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public static bool IsWindowsSSPI(string connectionstring)
        {
            Regex r = new Regex(@"Integrated Security=(?<sspi>\w+);*");
            if (r.Match(connectionstring).Result("${sspi}") == "SSPI")
                return true;
            else
                return false;
        }
        /// <summary>
        /// 匹配连接字符串的Catalog值
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public static string MatchInitialCatalog(string connectionstring)
        {
            Regex r = new Regex(@"Initial Catalog=(?<catalog>\w+);*");
            return r.Match(connectionstring).Result("${catalog}");
        }

        /// <summary>
        /// 匹配连接字符串的Password值
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        internal static string MatchSQLServer2005Password(string connectionstring)
        {
            Regex r = new Regex(@"Password=(?<passwsd>\w+);?");
            return r.Match(connectionstring).Result("${passwsd}");
        }
        /// <summary>
        /// 匹配连接字符串的UserId值
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        internal static string MatchSQLServer2005UserId(string connectionstring)
        {
            Regex r = new Regex(@"User ID=(?<userid>\w+);*");
            return r.Match(connectionstring).Result("${userid}");
        }
    }

    public class XMLHelper
    {
        public static XElement CreateBiz<T>(Head head, List<T> lstData) where T : IEntity
        {
            XElement element = new XElement("Message");

            element.Add(CreateHead(head));
            element.Add(CreateData(lstData));
            return element;
        }

        public static XElement CreateBizEX<dynamic>(Head head, List<dynamic> lstData)
        {
            XElement element = new XElement("Message");

            element.Add(CreateHead(head));
            element.Add(CreateDataEX(lstData));
            return element;
        }
        /// <summary>
        /// 创建报文头元素Head
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public static XElement CreateHead(Head head)
        {
            XElement element = null;
            Type typeT = typeof(Head);
            PropertyInfo[] pTarget = typeT.GetProperties();
            element = new XElement(typeT.Name);
            foreach (var item in pTarget)
            {
                if ("System.DateTime".Equals(item.PropertyType.ToString()))
                {
                    XElement el = new XElement(item.Name);
                    DateTime dt = (DateTime)item.GetValue(head, null);
                    if (dt != DateTime.MinValue)
                    {
                        el.SetValue(GetXmlDateString(dt));
                        element.Add(el);
                    }
                }
                else
                {
                    XElement el = new XElement(item.Name, item.GetValue(head, null));
                    element.Add(el);
                }

            }
            return element;
        }
        private static string GetXmlDateString(DateTime dt)
        {
            string s = dt.ToShortDateString() + "T" + dt.ToLongTimeString();
            try
            {
                s = dt.Year.ToString() + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0') + "T" + dt.Hour.ToString().PadLeft(2, '0') + ":" + dt.Minute.ToString().PadLeft(2, '0') + ":" + dt.Second.ToString().PadLeft(2, '0');
            }
            catch
            {

            }
            return s;
        }
        /// <summary>
        /// 创建报文主体数据元素Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public static XElement CreateData<T>(List<T> lstData) where T : IEntity
        {
            XElement elData = new XElement("Data");
            List<XElement> lstElement = CreateElements(lstData);
            if (lstElement != null)
            {
                foreach (XElement item in lstElement)
                {
                    elData.Add(item);
                }
            }
            return elData;
        }

        /// <summary>
        /// 创建报文主体数据元素Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public static XElement CreateDataEX<dynamic>(List<dynamic> lstData)
        {
            XElement elData = new XElement("Data");
            List<XElement> lstElement = CreateElementsEX(lstData);
            if (lstElement != null)
            {
                foreach (XElement item in lstElement)
                {
                    elData.Add(item);
                }
            }
            return elData;
        }
        /// <summary>
        /// 根据对象创建xml元素,Element名：对象类型名称，Attribute名：对象属性名称， Attribute值：对象属性值
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="lst">泛型数据集合</param>
        /// <returns></returns>
        public static List<XElement> CreateElements<T>(List<T> lst) where T : IEntity
        {
            List<XElement> lstResult = new List<XElement>();

            foreach (T item in lst)
            {
                lstResult.Add(CreateElement<T>(item));
            }

            return lstResult;
        }

        /// <summary>
        /// 根据对象创建xml元素,Element名：对象类型名称，Attribute名：对象属性名称， Attribute值：对象属性值
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="lst">泛型数据集合</param>
        /// <returns></returns>
        public static List<XElement> CreateElementsEX<dynamic>(List<dynamic> lst)
        {
            List<XElement> lstResult = new List<XElement>();

            foreach (dynamic item in lst)
            {
                lstResult.Add(CreateElementEX<dynamic>(item));
            }

            return lstResult;
        }
        /// <summary>
        /// 自定义的字段，不生成值
        /// </summary>
        public static List<string> lstProperty = new List<string>() { "PID", "CREATETIME", "CASENUM" };
        /// <summary>
        /// 根据对象创建xml元素,Element名：对象类型名称，Attribute名：对象属性名称， Attribute值：对象属性值
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="t">泛型对象</param>
        /// <returns></returns>
        public static XElement CreateElement<T>(T t) where T : IEntity
        {
            XElement element = null;
            Type typeT = t.GetType();
            PropertyInfo[] pTarget = typeT.GetProperties();
            element = new XElement(typeT.Name);

            foreach (var item in pTarget)
            {
                if (lstProperty.Contains(item.Name))
                    continue;
                object obj = item.GetValue(t, null);

                if (!string.IsNullOrEmpty(obj.toStringEX()))
                {
                    if ("System.DateTime".Equals(item.PropertyType.ToString()))
                    {
                        DateTime dt = (DateTime)obj;
                        if (dt != DateTime.MinValue)
                        {
                            XAttribute att = new XAttribute(item.Name, GetXmlDateString(dt));
                            element.Add(att);
                        }
                    }
                    else
                    {
                        if (t is DJF_DJ_SQR && "QLFRDH" == item.Name)//字段名错误，不修改数据库中字段名称，改由程序生成xml时纠正名称
                        {
                            XAttribute att = new XAttribute("QLRFRDH", obj);
                            element.Add(att);
                        }
                        else
                        {
                            XAttribute att = new XAttribute(item.Name, obj);
                            element.Add(att);
                        }
                    }
                }

            }

            return element;
        }

        public static XElement CreateElementEX<dynamic>(dynamic t)
        {
            XElement element = null;
            Type typeT = t.GetType();
            PropertyInfo[] pTarget = typeT.GetProperties();
            element = new XElement(typeT.Name);

            foreach (var item in pTarget)
            {
                if (lstProperty.Contains(item.Name))
                    continue;
                object obj = item.GetValue(t, null);

                if (!string.IsNullOrEmpty(obj.toStringEX())) // 等于空也添加 string.IsNullOrEmpty(obj.toStringEX())
                {
                    if ("System.DateTime".Equals(item.PropertyType.ToString()))
                    {
                        DateTime dt = (DateTime)obj;
                        if (dt != DateTime.MinValue)
                        {
                            XAttribute att = new XAttribute(item.Name, GetXmlDateString(dt));
                            element.Add(att);
                        }
                    }
                    else
                    {
                        if (t is DJF_DJ_SQR && "QLFRDH" == item.Name)//字段名错误，不修改数据库中字段名称，改由程序生成xml时纠正名称
                        {
                            XAttribute att = new XAttribute("QLRFRDH", obj);
                            element.Add(att);
                        }
                        else if (t is KTT_FW_H && "FWFHT" == item.Name)
                        {
                            XAttribute att = new XAttribute("FCFHT", obj); // 校验规则里面是这个字段
                            element.Add(att);
                        }else if (t is KTT_FW_H && "CPID" == item.Name)
                        {
                            // 不加这个属性
                        }
                        else
                        {
                            XAttribute att = new XAttribute(item.Name, obj);
                            element.Add(att);
                        }
                    }
                }
                else
                {
                    if (t is QLF_QL_DYAQ)
                    {
                        if ("ZXDYYY" == item.Name|| "ZJJZWZL"==item.Name|| "ZJJZWDYFW"==item.Name|| "ZGZQQDSS"==item.Name
                            )
                        {
                            XAttribute att = new XAttribute(item.Name, ""); 
                            element.Add(att);
                        }
                        else if ("ZXSJ" == item.Name || "ZWLXJSSJ" == item.Name || "ZWLXQSSJ" == item.Name)
                        {
                            XAttribute att = new XAttribute(item.Name, Convert.ToDateTime("0001-01-01T00:00:00"));
                            element.Add(att);
                        }

                    }
                    else if (t is QLF_QL_YGDJ)
                    {
                        if ("YWRZJZL"==item.Name|| "FWXZ"==item.Name)
                        {
                            XAttribute att = new XAttribute(item.Name, "");
                            element.Add(att);
                        }
                        else if ("DJSJ" == item.Name)
                        {
                            XAttribute att = new XAttribute(item.Name, Convert.ToDateTime("0001-01-01T00:00:00"));
                            element.Add(att);
                            
                        }
                    }
                    
                }
     

            }

            return element;
        }

        public static void Serialize<T>(T t, string xmlFilePath)
        {
            using (XmlWriter xw = XmlWriter.Create(xmlFilePath))
            {
                XmlSerializer xz = new XmlSerializer(t.GetType());
                xz.Serialize(xw, t);
            }
        }

        public static T DeserializeByXmlFilePath<T>(string xmlFilePath) where T : class
        {
            try
            {
                using (XmlReader xr = XmlReader.Create(xmlFilePath))
                {
                    XmlSerializer xz = new XmlSerializer(typeof(T));
                    return xz.Deserialize(xr) as T;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 区县操作枚举
    /// </summary>
    public enum QXOperationType
    {
        None,
        /// <summary>
        /// 数据检查
        /// </summary>
        CheckQXDataCommand,
        /// <summary>
        /// 数据同步
        /// </summary>
        DataExchangeCommand,
        /// <summary>
        /// 数据上报
        /// </summary>
        DataSubmitCommand,
        /// <summary>
        /// 报文回复
        /// </summary>
        ResponseCommand,
        /// <summary>
        /// 报文管理
        /// </summary>
        BizRepManager

    }
    /// <summary>
    /// 区县枚举
    /// </summary>
    public enum QXType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description(PropertyDes = "未定义")]
        None = 0,
        /// <summary>
        /// 庆云
        /// </summary>
        [Description(PropertyDes = "庆云")]
        QY = 371423,
        /// <summary>
        /// 齐河
        /// </summary>
        [Description(PropertyDes = "齐河")]
        QH = 371425,
        /// <summary>
        /// 平原
        /// </summary>
        [Description(PropertyDes = "平原")]
        PY = 371426,
        /// <summary>
        /// 夏津
        /// </summary>
        [Description(PropertyDes = "夏津")]
        XJ = 371427,
        /// <summary>
        /// 德城区
        /// </summary>
        [Description(PropertyDes = "德城区")]
        DC = 371402,
        /// <summary>
        /// 陵城区
        /// </summary>
        [Description(PropertyDes = "陵城区")]
        LC = 371403,
        /// <summary>
        /// 陵县
        /// </summary>
        [Description(PropertyDes = "陵县")]
        LX = 371421,
        /// <summary>
        /// 宁津
        /// </summary>
        [Description(PropertyDes = "宁津")]
        NJ = 371422,
        /// <summary>
        /// 临邑
        /// </summary>
        [Description(PropertyDes = "临邑")]
        LY = 371424,
        /// <summary>
        /// 武城
        /// </summary>
        [Description(PropertyDes = "武城")]
        WC = 371428,
        /// <summary>
        /// 乐陵
        /// </summary>
        [Description(PropertyDes = "乐陵")]
        LL = 371481,
        /// <summary>
        /// 禹城
        /// </summary>
        [Description(PropertyDes = "禹城")]
        YC = 371482
    }
    /// <summary>
    /// 数据库表中列数据类型枚举
    /// </summary>
    public enum ColumnType
    {
        VARCHAR2,
        DATE,
        NUMBER,
        BLOB
    }
    public enum QueryType
    {
        /// <summary>
        /// 只查询数量count(*)
        /// </summary>
        count,
        /// <summary>
        /// 查询所有字段*
        /// </summary>
        all
    }
    /// <summary>
    /// RNANDCN表SFSB字段枚举值
    /// </summary>

    public enum SFSBType
    {
        /// <summary>
        /// 0默认值(未做过任何操作)
        /// </summary>
        [Description(PropertyDes = "默认值(未做过任何操作)")]
        DefaultValue = 0,
        /// <summary>
        /// 1数据检查通过
        /// </summary>
        [Description(PropertyDes = "数据检查通过")]
        CheckSuccess = 1,
        /// <summary>
        /// 2数据检查未通过
        /// </summary>
        [Description(PropertyDes = "数据检查未通过")]
        CheckFailure = 2,
        /// <summary>
        /// 3向市局同步数据成功
        /// </summary>
        [Description(PropertyDes = "向市局同步数据成功")]
        ExchangeSuccess = 3,
        /// <summary>
        /// 4向市局同步数据失败
        /// </summary>
        [Description(PropertyDes = "向市局同步数据失败")]
        ExchangeFailure = 4
    }
    /// <summary>
    /// BIZANDREP表STATUS字段枚举值
    /// </summary>
    public enum STATUSType
    {
        /// <summary>
        /// 1默认值(未上报)
        /// </summary>
        [Description(PropertyDes = "默认值(未上报)")]
        DefaultValue = 1,
        /// <summary>
        /// 2上报中,未回复
        /// </summary>
        [Description(PropertyDes = "上报中,未回复")]
        Submiting = 2,
        /// <summary>
        /// 3已经回复,部里检查通过
        /// </summary>
        [Description(PropertyDes = "已经回复,部里检查通过")]
        RepSuccess = 3,
        /// <summary>
        /// 4已经回复,部里检查未通过
        /// </summary>
        [Description(PropertyDes = "已经回复,部里检查未通过")]
        RepFailure = 4
    }
    /// <summary>
    /// 任务显示或隐藏枚举
    /// </summary>
    public enum ExcuteTaskType
    {
        /// <summary>
        /// 显示任务执行进度
        /// </summary>
        Show,
        /// <summary>
        /// 隐藏任务执行进度
        /// </summary>
        Hide
    }

    public class EnumHelper
    {
        /// <summary>
        /// 根据行政区划代码，返回对应的QXType枚举类型
        /// </summary>
        /// <param name="qxdm"></param>
        /// <returns></returns>
        public static QXType GetByQXDM(string qxdm)
        {
            QXType q = QXType.None;
            foreach (QXType item in Enum.GetValues(typeof(QXType)))
            {
                if (qxdm.Equals(((int)item).ToString()))
                {
                    q = item; break;
                }
            }
            return q;
        }
    }
    public class DescriptionAttribute : Attribute
    {
        public string PropertyDes { get; set; }
    }
}
