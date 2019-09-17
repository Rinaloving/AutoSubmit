using BDCSubmit.Business.CommonClass;
using BDCSubmit.Business.SubmitModel;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDCSubmit.Business.BLL.QX
{
    public class CheckDataHandler
    {
        #region 声明
        private static CheckDataHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static CheckDataHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new CheckDataHandler();
                        }
                    }
                }
                return mInstance;
            }
        }

        private CheckDataHandler()
        { }
        #endregion


        string qxdm = "";
        IDatabase db = null;
        /// <summary>
        /// 陵城区有2个区县代码，老的371421，新的371403
        /// </summary>
        string lcqxdm = "";
        /// <summary>
        /// 数据检查(只检查RNANDCN表中SFSB值为0的数据)
        /// </summary>
        /// <param name="qx">区县枚举</param>
        /// <param name="et">任务显示或隐藏枚举</param>
        public void CheckTables(QXType qx, ExcuteTaskType et)
        {
            if (qx == QXType.None)
                return;
            //ShowDialogForm sdf = null;
            try
            {
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
                int isfsb = (int)SFSBType.DefaultValue;  // 是否非上报 0为非上报
                CheckBusinessTables cbt = XMLHelper.DeserializeByXmlFilePath<CheckBusinessTables>(SystemHandler.CheckBusinessTablesXMLFilePath);
                List<RNANDCN> lstRC = db.Fetch<RNANDCN>(" where qxdm='" + qxdm + "' and sfsb=" + isfsb + "").ToList(); //读取配置文件中，设置生成报文Data必填字段
                if (!string.IsNullOrEmpty(lcqxdm))
                {
                    if (lstRC != null)
                    {
                        List<RNANDCN> lstRC2 = db.Fetch<RNANDCN>(" where qxdm='" + lcqxdm + "' and sfsb=" + isfsb + "").ToList();
                        if (lstRC2 != null)
                            lstRC.AddRange(lstRC2);
                    }
                }
                if (lstRC != null && lstRC.Count > 0)
                {
                    int totalMain = lstRC.Count;
                    if (et == ExcuteTaskType.Show)
                    {
                        //sdf = new ShowDialogForm("提示", "正在检查数据...", "请稍候");
                        //sdf.SetProgress(totalMain);
                    }
                    int countMain = 1;
                    foreach (var rc in lstRC)
                    {
                        try
                        {
                            BusinessTypeClass btc = cbt.Business.Where(p => rc.JRYWBM.Equals(p.BusinessCode)).FirstOrDefault();
                            if (et == ExcuteTaskType.Show) { }
                                //sdf.SetMessage("正在检查业务号:" + rc.YWH + "。(" + countMain + "/" + totalMain + ")");
                            if (btc != null)
                            {
                                List<TableClass> lstTables = btc.检查表.Where(p => p.是否必选).ToList();

                                if (lstTables != null)
                                {
                                    int countSub = 1;
                                    int totalSub = lstTables.Count;
                                    if (et == ExcuteTaskType.Show) { }
                                        //sdf.SetProgressSub(totalSub);

                                    foreach (var item in lstTables)
                                    {
                                        int count = 0;
                                        if (et == ExcuteTaskType.Show) { }
                                            //sdf.SetContentSub("正在检查表:" + item.表名 + "。(" + countSub + "/" + totalSub + ")");
                                        count = GetDataCounts(item.表名, rc);
                                        if (count == 0)
                                        {
                                            if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                                rc.CHECKMESSAGE = item.表名 + "无数据";
                                            else
                                                rc.CHECKMESSAGE += ";" + item.表名 + "无数据";
                                        }
                                        countSub++;
                                    }
                                    if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                    {
                                        rc.SFSB = 1;
                                    }
                                    else
                                    {
                                        rc.SFSB = 2;
                                    }
                                    db.Update(rc);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                    rc.CHECKMESSAGE = rc.JRYWBM + "不规范，请检查";
                                else
                                    rc.CHECKMESSAGE += ";" + rc.JRYWBM + "不规范，请检查";
                                if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                {
                                    rc.SFSB = 1;
                                }
                                else
                                {
                                    rc.SFSB = 2;
                                }
                                db.Update(rc);
                            }
                            countMain++;
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
                //    sdf.Close();
                //    sdf.Dispose();
                //}
            }
        }

        private int GetDataCounts(string tableName, RNANDCN rc)
        {
            int count = 0;
            string sql = "";
            sql = GetQuerySQL(tableName, rc, QueryType.count);
            if (sql == "")
                return count;
            var result = db.ExecuteAsync(sql);
            //int.TryParse(baseHelper.ExecuteScalar(provider, sql).toStringEX(), out count);
            int.TryParse(result.Id.ToString(), out count);
            return count;
        }
        /// <summary>
        /// 获取市中间库中满足条件数据数量
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        //public int SJGetDataCounts(string tableName, RNANDCN rc)
        //{
        //    int count = 0;
        //    string sql = "";
        //    sql = GetQuerySQL(tableName, rc, QueryType.count);

        //    IBaseOperator SJbaseHelper = (SystemHandler.Instance.SJCommunicationContext as OracleContext).BaseHelper;
        //    IDbProvider SJprovider = new OracleProvider(SystemHandler.Instance.SJCommunicationconnectionstring);
        //    int.TryParse(SJbaseHelper.ExecuteScalar(SJprovider, sql).toStringEX(), out count);
        //    return count;
        //}


        public string GetQuerySQL(string tableName, RNANDCN rc, QueryType qt)
        {
            string sql = "";
            string qry = " count(*) ";
            switch (qt)
            {
                case QueryType.count:
                    qry = " count(*) ";
                    break;
                case QueryType.all:
                    qry = " * ";
                    break;
                default:
                    break;
            }
            switch (tableName)
            {
                case "ZTT_GY_QLR":
                    sql = string.Format("select " + qry + " from " + tableName + " where CASENUM='{0}'and BDCDYH='{1}'", rc.YWH, rc.REALEUNUM);
                    break;
                case "QLF_QL_TDSYQ":
                case "QLF_FW_FDCQ_DZ_XM":
                case "KTT_GZW":
                case "ZH_K_105":
                case "KTT_FW_ZRZ":
                case "KTF_QT_MZDZW":
                case "KTF_QT_XZDZW":
                case "KTF_QT_DZDZW":
                case "QLF_QL_QTXGQL":
                case "ZTF_GY_QLR_GX":
                case "FW_K_103":
                case "QLT_QL_LQ":
                    sql = string.Format("select " + qry + " from " + tableName + " where bdcdyh='{0}'", rc.REALEUNUM);
                    break;
                case "KTT_ZDJBXX":
                case "KTF_ZDBHQK":
                    sql = string.Format("select " + qry + " from " + tableName + " where zddm='{0}'", rc.REALEUNUM.Substring(0, 19));
                    break;
                case "ZD_K_103":
                    sql = string.Format("select " + qry + " from ZD_K_103 where substr(bdcdyh,0,19)='{0}'", rc.REALEUNUM.Substring(0, 19));
                    break;
                case "DJT_DJ_SLSQ":
                case "DJF_DJ_SQR":
                case "QLF_QL_JSYDSYQ":
                case "QLT_FW_FDCQ_DZ":
                case "QLT_FW_FDCQ_YZ":
                case "QLF_FW_FDCQ_QFSYQ":
                case "QLF_QL_NYDSYQ":
                case "QLF_QL_HYSYQ":
                case "QLT_QL_GJZWSYQ":
                case "QLF_QL_ZXDJ":
                case "QLF_QL_YYDJ":
                case "QLF_QL_YGDJ":
                case "QLF_QL_DYAQ":
                case "QLF_QL_CFDJ":
                case "QLF_QL_DYIQ":
                case "FJ_F_100":
                case "DJF_DJ_SJ":
                case "DJF_DJ_SF":
                case "DJF_DJ_SH":
                case "DJF_DJ_SZ":
                case "DJF_DJ_FZ":
                case "DJF_DJ_GD":
                    sql = string.Format("select " + qry + " from " + tableName + " where ywh='{0}'", rc.YWH);
                    break;

                case "KTT_ZHJBXX":
                case "KTF_ZH_YHZK":
                case "KTF_ZHBHQK":
                    sql = string.Format("select " + qry + " from " + tableName + " where zhdm='{0}'", rc.REALEUNUM.Substring(0, 19));
                    break;

                case "KTF_ZH_YHYDZB":
                case "KTT_GY_JZX":
                case "KTT_GY_JZD":
                    sql = string.Format("select " + qry + " from " + tableName + " where zhhddm='{0}'", rc.REALEUNUM.Substring(0, 19));
                    break;

                case "KTT_FW_LJZ":
                    KTT_FW_ZRZ zrz3 = db.Fetch<KTT_FW_ZRZ>(string.Format(" where bdcdyh='{0}'", rc.REALEUNUM).Substring(0, 28)).FirstOrDefault();
                    if (zrz3 != null)
                    {
                        sql = string.Format("select " + qry + " from KTT_FW_LJZ where ZRZPID='{0}'", zrz3.PID);
                    }
                    break;
                case "KTT_FW_C":

                    KTT_FW_ZRZ zrz = db.Fetch<KTT_FW_ZRZ>(string.Format(" where bdcdyh='{0}'", rc.REALEUNUM).Substring(0, 28)).FirstOrDefault();
                    if (zrz != null)
                    {
                        sql = string.Format("select " + qry + " from KTT_FW_C where ZRZPID='{0}'", zrz.PID);
                    }
                    break;
                case "KTT_FW_H":

                    // if ("371425".Equals(qxdm))
                    // {
                    KTT_FW_ZRZ zrz2 = db.Fetch<KTT_FW_ZRZ>(string.Format(" where bdcdyh='{0}' order by createtime desc ", rc.REALEUNUM.Substring(0, 24) + "0000")).FirstOrDefault();
                    if (zrz2 != null)
                    {
                        KTT_FW_C fwc = db.Fetch<KTT_FW_C>(string.Format(" where ZRZH='{0}' order by createtime desc ", zrz2.ZRZH)).FirstOrDefault();
                        if (fwc != null)
                        {
                            KTT_FW_H fwh = db.Fetch<KTT_FW_H>(string.Format(" where ZRZH='{0}' and CH='{1}'and BDCDYH='{2}' order by createtime desc ", fwc.ZRZH, fwc.CH, rc.REALEUNUM.Substring(0, 28))).FirstOrDefault();
                            //这里根据
                            if (fwh != null)
                            {
                                sql = string.Format("select " + qry + " from KTT_FW_H where ZRZH='{0}' and CH='{1}'and BDCDYH='{2}'and pid ='{3}' ", fwc.ZRZH, fwc.CH, rc.REALEUNUM.Substring(0, 28), fwh.PID); // 只获取一条最新的
                            }
                            else //经过分析，现场数据库会出现一种层中最新的记录在户室表中不存在，此时我们根据单元号和户室表中的最新时间来匹配最新的一条
                            {
                                KTT_FW_H fwh2 = db.Fetch<KTT_FW_H>(string.Format(" where  BDCDYH='{0}' order by createtime desc ", rc.REALEUNUM.Substring(0, 28))).FirstOrDefault();
                                if (fwh2 != null)
                                {
                                    sql = string.Format("select " + qry + " from KTT_FW_H where  BDCDYH='{0}'and  to_char(createtime,'yyyy/mm/dd,hh24:mi:ss') =  to_char(to_date('{1}','yyyy/mm/dd,hh24:mi:ss'),'yyyy/mm/dd,hh24:mi:ss') ", rc.REALEUNUM.Substring(0, 28), fwh2.CREATETIME);
                                }

                            }

                        }

                    }

                    // }
                    //else
                    //{
                    //    KTT_FW_ZRZ zrz2 = curContext.GetList<KTT_FW_ZRZ>(string.Format(" where bdcdyh='{0}' ", rc.REALEUNUM.Substring(0,28))).FirstOrDefault();
                    //    if (zrz2 != null)
                    //    {
                    //        KTT_FW_C fwc = curContext.GetList<KTT_FW_C>(string.Format(" where ZRZPID='{0}'", zrz2.PID)).FirstOrDefault();
                    //        if (fwc != null)
                    //        {
                    //            sql = string.Format("select " + qry + " from KTT_FW_H where CPID='{0}'", fwc.PID);
                    //        }
                    //    }
                    //}

                    break;

                default:
                    break;
            }
            return sql;
        }

    }
}
