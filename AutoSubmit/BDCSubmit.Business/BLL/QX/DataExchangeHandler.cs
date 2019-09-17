using BDCSubmit.Business.CommonClass;
using BDCSubmit.Business.SubmitModel;
using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BDCSubmit.Business.BLL.QX
{
    public class DataExchangeHandler
    {
        #region 声明
        private static DataExchangeHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static DataExchangeHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new DataExchangeHandler();
                        }
                    }
                }
                return mInstance;
            }
        }



        private DataExchangeHandler()
        { }
        #endregion

        

        string qxdm = "";
        /// <summary>
        /// 陵城区有2个区县代码，老的371421，新的371403
        /// </summary>
        string lcqxdm = "";
        IDatabase db = null;
        IDatabase sjdb = null;
        /// <summary>
        /// 数据同步(只同步RNANDCN表中SFSB值为1(数据检查通过)的数据)
        /// </summary>
        /// <param name="qx"></param>
        public void DataExchange(QXType qx, ExcuteTaskType et)
        {
            if (qx == QXType.None) return;
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
                int isfsb = (int)SFSBType.CheckSuccess;
                CheckBusinessTables cbt = XMLHelper.DeserializeByXmlFilePath<CheckBusinessTables>(SystemHandler.CheckBusinessTablesXMLFilePath);
                //查询通过检查的数据，才进行同步
                List<RNANDCN> lstRC = db.Fetch<RNANDCN>(" where qxdm='" + qxdm + "' and sfsb=" + isfsb + "").ToList();
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
                    int count = 1, total = lstRC.Count;
                    if (et == ExcuteTaskType.Show)
                    {
                        //sdf = new ShowDialogForm("提示", "正在同步数据...", "请稍候");
                        //sdf.SetProgress(total);
                    }
                    foreach (var rc in lstRC)
                    {
                        BusinessTypeClass btc = cbt.Business.Where(p => rc.JRYWBM.Equals(p.BusinessCode)).FirstOrDefault();
                        List<TableClass> lstTables = btc.检查表.Where(p => p.是否必选).ToList();
                        string strExchangeErr = "";

                        try
                        {
                            sjdb = new Database("sjsubmit");

                            RNANDCN newRC = new RNANDCN();
                            //CopySameField(rc, newRC);
                            newRC = rc;
                            newRC.SFSB = (int)SFSBType.ExchangeSuccess;
                            //市级库新增RNANDCN
                            int count1 = sjdb.Fetch<RNANDCN>(" where YWH='" + newRC.YWH + "' and QXDM='" + newRC.QXDM + "'").Count;
                            if (count1 == 0)
                                sjdb.Insert("RNANDCN", "PID", false, newRC);
                            else
                            {
                                RNANDCN sjRC = sjdb.Fetch<RNANDCN>(" where YWH='" + newRC.YWH + "' and QXDM='" + newRC.QXDM + "'").FirstOrDefault();
                                if (sjRC != null)
                                {
                                    //CopySameField(sjRC, newRC);
                                    sjRC = newRC;
                                    sjRC.SFSB = newRC.SFSB;
                                    sjdb.Update(sjRC);
                                }
                            }
                            //市级库生成BizAndRep 
                            BIZANDREP bar = new BIZANDREP();
                            bar.PID = Guid.NewGuid().ToString("N");
                            bar.YWH = rc.YWH;
                            bar.STATUS = (int)STATUSType.DefaultValue;
                            bar.QXDM = rc.QXDM;
                            bar.CREATETIME = DateTime.Now;
                            int count2 = sjdb.Fetch<BIZANDREP>(" where YWH='" + bar.YWH + "' and QXDM='" + bar.QXDM + "'").Count;
                            if (count2 == 0)
                                sjdb.Insert("BIZANDREP", "PID", false, bar);
                            else
                            {
                                BIZANDREP sjBar = sjdb.Fetch<BIZANDREP>(" where YWH='" + bar.YWH + "' and QXDM='" + bar.QXDM + "'").FirstOrDefault();
                                if (sjBar != null)
                                {
                                    sjBar.YWH = rc.YWH;
                                    sjBar.STATUS = bar.STATUS;
                                    sjdb.Update(sjBar);
                                }
                            }
                            //县级库生成BizAndRep
                            //curContext.BeginTransaction();
                            int count3 = db.Fetch<BIZANDREP>(" where YWH='" + bar.YWH + "'").Count;
                            if (count3 == 0)
                                db.Insert("BIZANDREP", "PID", false, bar);
                            else
                            {
                                BIZANDREP xjBar = db.Fetch<BIZANDREP>(" where YWH='" + bar.YWH + "'").FirstOrDefault();
                                if (xjBar != null)
                                {
                                    xjBar.STATUS = bar.STATUS;
                                    db.Update(xjBar);
                                }
                            }
                            bool isError = false;
                            foreach (var item in lstTables)
                            {

                                List<dynamic> lstobj = GetNewInstance(item.表名, rc,db);



                                if (lstobj != null && lstobj.Count > 0)
                                {
                                    foreach (var sjobj in lstobj)
                                    {
                                        //if (true)
                                        //sjdb.Update(sjobj);
                                        sjdb.Insert(item.表名, "PID", false, sjobj);
                                        //else
                                        //{
                                        //    //UpdateSJobjInTransaction(sjobj);
                                        //    sjdb.Update(sjobj);
                                        //}
                                    }
                                }
                                //如果正常按程序执行，执行数据同步的数据肯定是通过数据检查的
                                //但万一有数据取不到的，写入CHECKMESSAGE
                                else
                                {
                                    if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                        rc.CHECKMESSAGE = "业务号:" + rc.YWH + "向市局同步失败,原因:" + item.表名 + "未取到数据";
                                    else
                                        rc.CHECKMESSAGE += ";" + rc.YWH + "向市局同步失败,原因:" + item.表名 + "未取到数据";
                                    rc.SFSB = (int)SFSBType.CheckFailure;
                                    db.Update(rc);      // modify by cfl 2018年4月19日                              

                                    isError = true;
                                    break;
                                }
                            }
                            if (isError) continue;
                            //更新区县库RNANDCN状态
                            rc.SFSB = (int)SFSBType.ExchangeSuccess;
                            db.Update(rc);


                            if (et == ExcuteTaskType.Show) { }
                                //sdf.SetMessage("业务号:" + rc.YWH + "相关数据同步成功。(" + count + "/" + total + ")");

                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                strExchangeErr = ex.Message;

                                rc.SFSB = (int)SFSBType.ExchangeFailure;
                                if (string.IsNullOrEmpty(rc.CHECKMESSAGE))
                                    rc.CHECKMESSAGE = "业务号:" + rc.YWH + "向市局同步失败,原因:" + strExchangeErr;
                                else
                                    rc.CHECKMESSAGE += ";" + rc.YWH + "向市局同步失败,原因:" + strExchangeErr;
                                db.Update(rc);
                                if (et == ExcuteTaskType.Show) { }
                                    //sdf.SetMessage("业务号:" + rc.YWH + "相关数据向市局同步失败。(" + count + "/" + total + ")");
                            }
                            catch
                            {
                                throw ex;
                            }
                        }

                        count++;
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

        private List<dynamic> GetQXInstance(string tableName, RNANDCN rc, IDatabase db)
        {
            dynamic entity = null;
            string sql = CheckDataHandler.Instance.GetQuerySQL(tableName, rc, QueryType.all);

            //通过反射来创建实例
            Assembly pAssembly = Assembly.LoadFrom(SystemHandler.assPath);


            try
            {

                List<dynamic> lst = db.Query<dynamic>(sql).ToList();
                List<dynamic> newList = new List<dynamic>();


                foreach (var item in lst)
                {
                    entity = pAssembly.CreateInstance("BDCSubmit.Business.SubmitModel." + tableName);
                    if (entity == null) return null;
                    PropertyInfo[] fields = entity.GetType().GetProperties();//获取指定对象的所有公共属性
                    List<object> vs = new List<object>();
                    var values = item.Values;
                    //var keys = item.Keys;

                    //foreach (var val in keys)
                    //{
                    //    ks.Add(val);
                    //}
                    foreach (var val in values)
                    {
                        vs.Add(val);
                    }

                    for (int i = 0; i < vs.Count; i++)
                    {

                        //entity = vs[i];
                        object result = null;
                        if (vs[i] != null)
                        {
                            if (fields[i].PropertyType.FullName == "System.Int16")
                            {
                                result = Convert.ToDecimal(vs[i]);
                            }
                            else if (fields[i].PropertyType.FullName == "System.Double")
                            {
                                result = Convert.ToDecimal(vs[i]);
                            }
                            else if (fields[i].PropertyType.FullName == "System.Byte")
                            {
                                result = Convert.ToByte(vs[i]);
                            }
                            else if (fields[i].PropertyType.FullName == "System.Decimal")
                            {
                                result = Convert.ToDecimal(vs[i]);
                            }
                            else
                            {
                                result = vs[i];
                            }
                        }



                        fields[i].SetValue(entity, result); // 给属性赋值
                    }



                    newList.Add(entity);

                }


                return newList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable DicToTable(Dictionary<string, object> dicDep)
        {
            DataTable dt = new DataTable();
            foreach (var colName in dicDep.Keys)
            {
                dt.Columns.Add(colName, typeof(string));
            }
            DataRow dr = dt.NewRow();
            foreach (KeyValuePair<string, object> item in dicDep)
            {
                dr[item.Key] = item.Value;
            }
            dt.Rows.Add(dr);
            return dt;
        }


        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        private List<dynamic> GetNewInstance(string tableName, RNANDCN rc, IDatabase db)
        {
            try
            {
                List<dynamic> lstqxEntity = GetQXInstance(tableName, rc,db);
                List<dynamic> lstsjEntity = new List<dynamic>();
                if (lstqxEntity == null) return null;
                Assembly pAssembly = Assembly.LoadFrom(SystemHandler.assPath);

                foreach (var item in lstqxEntity)
                {
                    dynamic SJEntity = pAssembly.CreateInstance("BDCSubmit.Business.SubmitModel." + tableName);
                    // Object SJEntity = tableName as Object;
                    if (SJEntity == null) return null;
                    // CopySameField(item, SJEntity);
                    SJEntity = item;
                    lstsjEntity.Add(SJEntity);

                }
                return lstsjEntity;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //private void CopySameField(object source, object target)
        //{
        //    if (source == null || target == null) return;
        //    PropertyInfo[] pi = target.GetType().GetProperties();
        //    object tValue = null;
        //    foreach (var item in pi)
        //    {
        //        SmartMap.DataClient.Model.ColumnAttribute ca = item.GetCustomAttributes(typeof(SmartMap.DataClient.Model.ColumnAttribute), false).FirstOrDefault() as SmartMap.DataClient.Model.ColumnAttribute;
        //        if (ca == null) continue;
        //        if ("CREATETIME".Equals(item.Name))
        //        {
        //            item.SetValue(target, DateTime.Now, null); continue;
        //        }
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
        //                        item.SetValue(target, tValue.toStringEX(), null);
        //                        break;
        //                }
        //            }
        //        }
        //    }
        }

        //public List<RNANDCN> GetData(QXType qx)
        //{
        //    int isfsb = (int)SFSBType.CheckSuccess;
        //    List<RNANDCN> lstData = null;
        //    switch (qx)
        //    {
        //        case QXType.QH:
        //            lstData = SystemHandler.Instance.QHCommunicationContext.GetList<RNANDCN>(" where qxdm='371425' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.PY:
        //            lstData = SystemHandler.Instance.PYCommunicationContext.GetList<RNANDCN>(" where qxdm='371426' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.XJ:
        //            lstData = SystemHandler.Instance.XJCommunicationContext.GetList<RNANDCN>(" where qxdm='371427' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.QY:
        //            lstData = SystemHandler.Instance.QYCommunicationContext.GetList<RNANDCN>(" where qxdm='371423' and sfsb=" + isfsb + "").ToList();
        //            break;

        //        case QXType.DC:
        //            lstData = SystemHandler.Instance.DCCommunicationContext.GetList<RNANDCN>(" where qxdm='371402' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.LC:
        //        case QXType.LX:
        //            lstData = SystemHandler.Instance.LCCommunicationContext.GetList<RNANDCN>(" where qxdm='371421' and sfsb=" + isfsb + "").ToList();
        //            lstData.AddRange(SystemHandler.Instance.LCCommunicationContext.GetList<RNANDCN>(" where qxdm='371403' and sfsb=" + isfsb + "").ToList());
        //            break;
        //        case QXType.NJ:
        //            lstData = SystemHandler.Instance.NJCommunicationContext.GetList<RNANDCN>(" where qxdm='371422' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.LY:
        //            lstData = SystemHandler.Instance.LYCommunicationContext.GetList<RNANDCN>(" where qxdm='371424' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.WC:
        //            lstData = SystemHandler.Instance.WCCommunicationContext.GetList<RNANDCN>(" where qxdm='371428' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.LL:
        //            lstData = SystemHandler.Instance.LLCommunicationContext.GetList<RNANDCN>(" where qxdm='371481' and sfsb=" + isfsb + "").ToList();
        //            break;
        //        case QXType.YC:
        //            lstData = SystemHandler.Instance.YCCommunicationContext.GetList<RNANDCN>(" where qxdm='371482' and sfsb=" + isfsb + "").ToList();
        //            break;



        //        default:
        //            break;
        //    }
        //    return lstData;
        //}

        //public bool IsExistsSJobj(object entity)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        int count = 0;
        //        //string tableName = entity.GetType().Name;
        //        string tableName = entity.ToString();
        //        string pid = "";
        //        switch (tableName)
        //        {
        //            case "ZTT_GY_QLR":
        //                pid = (entity as ZTT_GY_QLR).PID;
        //                break;
        //            case "QLF_QL_TDSYQ":
        //                pid = (entity as QLF_QL_TDSYQ).PID;
        //                break;
        //            case "QLF_FW_FDCQ_DZ_XM":
        //                pid = (entity as QLF_FW_FDCQ_DZ_XM).PID;
        //                break;
        //            case "KTT_GZW":
        //                pid = (entity as KTT_GZW).PID;
        //                break;
        //            case "ZH_K_105":
        //                pid = (entity as ZH_K_105).PID;
        //                break;
        //            case "KTT_FW_ZRZ":
        //                pid = (entity as KTT_FW_ZRZ).PID;
        //                break;
        //            case "KTT_ZDJBXX":
        //                pid = (entity as KTT_ZDJBXX).PID;
        //                break;
        //            case "KTF_ZDBHQK":
        //                pid = (entity as KTF_ZDBHQK).PID;
        //                break;
        //            case "ZD_K_103":
        //                pid = (entity as ZD_K_103).PID;
        //                break;
        //            case "DJT_DJ_SLSQ":
        //                pid = (entity as DJT_DJ_SLSQ).PID;
        //                break;
        //            case "DJF_DJ_SQR":
        //                pid = (entity as DJF_DJ_SQR).PID;
        //                break;
        //            case "QLF_QL_JSYDSYQ":
        //                pid = (entity as QLF_QL_JSYDSYQ).PID;
        //                break;
        //            case "QLT_FW_FDCQ_DZ":
        //                pid = (entity as QLT_FW_FDCQ_DZ).PID;
        //                break;
        //            case "QLT_FW_FDCQ_YZ":
        //                pid = (entity as QLT_FW_FDCQ_YZ).PID;
        //                break;
        //            case "QLF_FW_FDCQ_QFSYQ":
        //                pid = (entity as QLF_FW_FDCQ_QFSYQ).PID;
        //                break;
        //            case "QLF_QL_NYDSYQ":
        //                pid = (entity as QLF_QL_NYDSYQ).PID;
        //                break;
        //            case "QLF_QL_HYSYQ":
        //                pid = (entity as QLF_QL_HYSYQ).PID;
        //                break;
        //            case "QLT_QL_GJZWSYQ":
        //                pid = (entity as QLT_QL_GJZWSYQ).PID;
        //                break;
        //            case "QLF_QL_ZXDJ":
        //                pid = (entity as QLF_QL_ZXDJ).PID;
        //                break;
        //            case "QLF_QL_YYDJ":
        //                pid = (entity as QLF_QL_YYDJ).PID;
        //                break;
        //            case "QLF_QL_YGDJ":
        //                pid = (entity as QLF_QL_YGDJ).PID;
        //                break;
        //            case "QLF_QL_DYAQ":
        //                pid = (entity as QLF_QL_DYAQ).PID;
        //                break;
        //            case "QLF_QL_CFDJ":
        //                pid = (entity as QLF_QL_CFDJ).PID;
        //                break;
        //            case "QLF_QL_DYIQ":
        //                pid = (entity as QLF_QL_DYIQ).PID;
        //                break;
        //            case "FJ_F_100":
        //                pid = (entity as FJ_F_100).PID;
        //                break;
        //            case "DJF_DJ_SJ":
        //                pid = (entity as DJF_DJ_SJ).PID;
        //                break;
        //            case "DJF_DJ_SF":
        //                pid = (entity as DJF_DJ_SF).PID;
        //                break;
        //            case "DJF_DJ_SH":
        //                pid = (entity as DJF_DJ_SH).PID;
        //                break;
        //            case "DJF_DJ_SZ":
        //                pid = (entity as DJF_DJ_SZ).PID;
        //                break;
        //            case "DJF_DJ_FZ":
        //                pid = (entity as DJF_DJ_FZ).PID;
        //                break;
        //            case "DJF_DJ_GD":
        //                pid = (entity as DJF_DJ_GD).PID;
        //                break;
        //            case "KTT_ZHJBXX":
        //                pid = (entity as KTT_ZHJBXX).PID;
        //                break;
        //            case "KTF_ZH_YHZK":
        //                pid = (entity as KTF_ZH_YHZK).PID;
        //                break;
        //            case "KTF_ZHBHQK":
        //                pid = (entity as KTF_ZHBHQK).PID;
        //                break;
        //            case "KTF_ZH_YHYDZB":
        //                pid = (entity as KTF_ZH_YHYDZB).PID;
        //                break;
        //            case "KTT_GY_JZX":
        //                pid = (entity as KTT_GY_JZX).PID;
        //                break;
        //            case "KTT_GY_JZD":
        //                pid = (entity as KTT_GY_JZD).PID;
        //                break;
        //            case "KTT_FW_LJZ":
        //                pid = (entity as KTT_FW_LJZ).PID;
        //                break;
        //            case "KTT_FW_C":
        //                pid = (entity as KTT_FW_C).PID;
        //                break;
        //            case "KTT_FW_H":
        //                pid = (entity as KTT_FW_H).PID;
        //                break;
        //            default:
        //                break;
        //        }


        //        count = Convert.ToInt32((SystemHandler.Instance.SJCommunicationContext as OracleContext).BaseHelper.ExecuteScalar(_SjProvider, "select count(*) from " + tableName + " where PID='" + pid + "'")); // modify by cfl 2018年4月19日
        //        if (count > 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool UpdateSJobjInTransaction(IEntity entity)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string tableName = entity.GetType().Name;
        //        string pid = "";
        //        switch (tableName)
        //        {
        //            case "ZTT_GY_QLR":
        //                {
        //                    pid = (entity as ZTT_GY_QLR).PID;
        //                    ZTT_GY_QLR tmpZTT_GY_QLR = sjdb.Fetch<ZTT_GY_QLR>("PID", pid).FirstOrDefault();
        //                    if (tmpZTT_GY_QLR != null)
        //                    {
        //                        //CopySameField(entity, tmpZTT_GY_QLR);
        //                        sjdb.Update(tmpZTT_GY_QLR);
        //                    }
        //                    break;
        //                }
        //            case "QLF_QL_TDSYQ":
        //                pid = (entity as QLF_QL_TDSYQ).PID;
        //                QLF_QL_TDSYQ tmpQLF_QL_TDSYQ = sjdb.Fetch<QLF_QL_TDSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_TDSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_TDSYQ);
        //                    sjdb.Update(tmpQLF_QL_TDSYQ);
        //                }
        //                break;
        //            case "QLF_FW_FDCQ_DZ_XM":
        //                pid = (entity as QLF_FW_FDCQ_DZ_XM).PID;
        //                QLF_FW_FDCQ_DZ_XM tmpQLF_FW_FDCQ_DZ_XM = sjdb.Fetch<QLF_FW_FDCQ_DZ_XM>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_FW_FDCQ_DZ_XM != null)
        //                {
        //                    CopySameField(entity, tmpQLF_FW_FDCQ_DZ_XM);
        //                    sjdb.Update(tmpQLF_FW_FDCQ_DZ_XM);
        //                }
        //                break;
        //            case "KTT_GZW":
        //                pid = (entity as KTT_GZW).PID;
        //                KTT_GZW tmpKTT_GZW = sjdb.Fetch<KTT_GZW>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_GZW != null)
        //                {
        //                    CopySameField(entity, tmpKTT_GZW);
        //                    sjdb.Update(tmpKTT_GZW);
        //                }
        //                break;
        //            case "ZH_K_105":
        //                pid = (entity as ZH_K_105).PID;
        //                ZH_K_105 tmpZH_K_105 = sjdb.Fetch<ZH_K_105>("PID", pid).FirstOrDefault();
        //                if (tmpZH_K_105 != null)
        //                {
        //                    CopySameField(entity, tmpZH_K_105);
        //                    sjdb.Update(tmpZH_K_105);
        //                }
        //                break;
        //            case "KTT_FW_ZRZ":
        //                pid = (entity as KTT_FW_ZRZ).PID;
        //                KTT_FW_ZRZ tmpKTT_FW_ZRZ = sjdb.Fetch<KTT_FW_ZRZ>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_FW_ZRZ != null)
        //                {
        //                    CopySameField(entity, tmpKTT_FW_ZRZ);
        //                    sjdb.Update(tmpKTT_FW_ZRZ);
        //                }
        //                break;
        //            case "KTT_ZDJBXX":
        //                pid = (entity as KTT_ZDJBXX).PID;
        //                KTT_ZDJBXX tmpKTT_ZDJBXX = sjdb.Fetch<KTT_ZDJBXX>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_ZDJBXX != null)
        //                {
        //                    CopySameField(entity, tmpKTT_ZDJBXX);
        //                    sjdb.Update(tmpKTT_ZDJBXX);
        //                }
        //                break;
        //            case "KTF_ZDBHQK":
        //                pid = (entity as KTF_ZDBHQK).PID;
        //                KTF_ZDBHQK tmpKTF_ZDBHQK = sjdb.Fetch<KTF_ZDBHQK>("PID", pid).FirstOrDefault();
        //                if (tmpKTF_ZDBHQK != null)
        //                {
        //                    CopySameField(entity, tmpKTF_ZDBHQK);
        //                    sjdb.Update(tmpKTF_ZDBHQK);
        //                }
        //                break;
        //            case "ZD_K_103":
        //                pid = (entity as ZD_K_103).PID;
        //                ZD_K_103 tmpZD_K_103 = sjdb.Fetch<ZD_K_103>("PID", pid).FirstOrDefault();
        //                if (tmpZD_K_103 != null)
        //                {
        //                    CopySameField(entity, tmpZD_K_103);
        //                    sjdb.Update(tmpZD_K_103);
        //                }
        //                break;
        //            case "DJT_DJ_SLSQ":
        //                pid = (entity as DJT_DJ_SLSQ).PID;
        //                DJT_DJ_SLSQ tmpDJT_DJ_SLSQ = sjdb.Fetch<DJT_DJ_SLSQ>("PID", pid).FirstOrDefault();
        //                if (tmpDJT_DJ_SLSQ != null)
        //                {
        //                    CopySameField(entity, tmpDJT_DJ_SLSQ);
        //                    sjdb.Update(tmpDJT_DJ_SLSQ);
        //                }
        //                break;
        //            case "DJF_DJ_SQR":
        //                pid = (entity as DJF_DJ_SQR).PID;
        //                DJF_DJ_SQR tmpDJF_DJ_SQR = sjdb.Fetch<DJF_DJ_SQR>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_SQR != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_SQR);
        //                    sjdb.Update(tmpDJF_DJ_SQR);
        //                }
        //                break;
        //            case "QLF_QL_JSYDSYQ":
        //                pid = (entity as QLF_QL_JSYDSYQ).PID;
        //                QLF_QL_JSYDSYQ tmpQLF_QL_JSYDSYQ = sjdb.Fetch<QLF_QL_JSYDSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_JSYDSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_JSYDSYQ);
        //                    sjdb.Update(tmpQLF_QL_JSYDSYQ);
        //                }
        //                break;
        //            case "QLT_FW_FDCQ_DZ":
        //                pid = (entity as QLT_FW_FDCQ_DZ).PID;
        //                QLT_FW_FDCQ_DZ tmpQLT_FW_FDCQ_DZ = sjdb.Fetch<QLT_FW_FDCQ_DZ>("PID", pid).FirstOrDefault();
        //                if (tmpQLT_FW_FDCQ_DZ != null)
        //                {
        //                    CopySameField(entity, tmpQLT_FW_FDCQ_DZ);
        //                    sjdb.Update(tmpQLT_FW_FDCQ_DZ);
        //                }
        //                break;
        //            case "QLT_FW_FDCQ_YZ":
        //                pid = (entity as QLT_FW_FDCQ_YZ).PID;
        //                QLT_FW_FDCQ_YZ tmpQLT_FW_FDCQ_YZ = sjdb.Fetch<QLT_FW_FDCQ_YZ>("PID", pid).FirstOrDefault();
        //                if (tmpQLT_FW_FDCQ_YZ != null)
        //                {
        //                    CopySameField(entity, tmpQLT_FW_FDCQ_YZ);
        //                    sjdb.Update(tmpQLT_FW_FDCQ_YZ);
        //                }
        //                break;
        //            case "QLF_FW_FDCQ_QFSYQ":
        //                pid = (entity as QLF_FW_FDCQ_QFSYQ).PID;
        //                QLF_FW_FDCQ_QFSYQ tmpQLF_FW_FDCQ_QFSYQ = sjdb.Fetch<QLF_FW_FDCQ_QFSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_FW_FDCQ_QFSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_FW_FDCQ_QFSYQ);
        //                    sjdb.Update(tmpQLF_FW_FDCQ_QFSYQ);
        //                }
        //                break;
        //            case "QLF_QL_NYDSYQ":
        //                pid = (entity as QLF_QL_NYDSYQ).PID;
        //                QLF_QL_NYDSYQ tmpQLF_QL_NYDSYQ = sjdb.Fetch<QLF_QL_NYDSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_NYDSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_NYDSYQ);
        //                    sjdb.Update(tmpQLF_QL_NYDSYQ);
        //                }
        //                break;
        //            case "QLF_QL_HYSYQ":
        //                pid = (entity as QLF_QL_HYSYQ).PID;
        //                QLF_QL_HYSYQ tmpQLF_QL_HYSYQ = sjdb.Fetch<QLF_QL_HYSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_HYSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_HYSYQ);
        //                    sjdb.Update(tmpQLF_QL_HYSYQ);
        //                }
        //                break;
        //            case "QLT_QL_GJZWSYQ":
        //                pid = (entity as QLT_QL_GJZWSYQ).PID;
        //                QLT_QL_GJZWSYQ tmpQLT_QL_GJZWSYQ = sjdb.Fetch<QLT_QL_GJZWSYQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLT_QL_GJZWSYQ != null)
        //                {
        //                    CopySameField(entity, tmpQLT_QL_GJZWSYQ);
        //                    sjdb.Update(tmpQLT_QL_GJZWSYQ);
        //                }
        //                break;
        //            case "QLF_QL_ZXDJ":
        //                pid = (entity as QLF_QL_ZXDJ).PID;
        //                QLF_QL_ZXDJ tmpQLF_QL_ZXDJ = sjdb.Fetch<QLF_QL_ZXDJ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_ZXDJ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_ZXDJ);
        //                    sjdb.Update(tmpQLF_QL_ZXDJ);
        //                }
        //                break;
        //            case "QLF_QL_YYDJ":
        //                pid = (entity as QLF_QL_YYDJ).PID;
        //                QLF_QL_YYDJ tmpQLF_QL_YYDJ = sjdb.Fetch<QLF_QL_YYDJ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_YYDJ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_YYDJ);
        //                    sjdb.Update(tmpQLF_QL_YYDJ);
        //                }
        //                break;
        //            case "QLF_QL_YGDJ":
        //                pid = (entity as QLF_QL_YGDJ).PID;
        //                QLF_QL_YGDJ tmpQLF_QL_YGDJ = sjdb.Fetch<QLF_QL_YGDJ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_YGDJ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_YGDJ);
        //                    sjdb.Update(tmpQLF_QL_YGDJ);
        //                }
        //                break;
        //            case "QLF_QL_DYAQ":
        //                pid = (entity as QLF_QL_DYAQ).PID;
        //                QLF_QL_DYAQ tmpQLF_QL_DYAQ = sjdb.Fetch<QLF_QL_DYAQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_DYAQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_DYAQ);
        //                    sjdb.Update(tmpQLF_QL_DYAQ);
        //                }
        //                break;
        //            case "QLF_QL_CFDJ":
        //                pid = (entity as QLF_QL_CFDJ).PID;
        //                QLF_QL_CFDJ tmpQLF_QL_CFDJ = sjdb.Fetch<QLF_QL_CFDJ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_CFDJ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_CFDJ);
        //                    sjdb.Update(tmpQLF_QL_CFDJ);
        //                }
        //                break;
        //            case "QLF_QL_DYIQ":
        //                pid = (entity as QLF_QL_DYIQ).PID;
        //                QLF_QL_DYIQ tmpQLF_QL_DYIQ = sjdb.Fetch<QLF_QL_DYIQ>("PID", pid).FirstOrDefault();
        //                if (tmpQLF_QL_DYIQ != null)
        //                {
        //                    CopySameField(entity, tmpQLF_QL_DYIQ);
        //                    sjdb.Update(tmpQLF_QL_DYIQ);
        //                }
        //                break;
        //            case "FJ_F_100":
        //                pid = (entity as FJ_F_100).PID;
        //                FJ_F_100 tmpFJ_F_100 = sjdb.Fetch<FJ_F_100>("PID", pid).FirstOrDefault();
        //                if (tmpFJ_F_100 != null)
        //                {
        //                    CopySameField(entity, tmpFJ_F_100);
        //                    sjdb.Update(tmpFJ_F_100);
        //                }
        //                break;
        //            case "DJF_DJ_SJ":
        //                pid = (entity as DJF_DJ_SJ).PID;
        //                DJF_DJ_SJ tmpDJF_DJ_SJ = sjdb.Fetch<DJF_DJ_SJ>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_SJ != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_SJ);
        //                    sjdb.Update(tmpDJF_DJ_SJ);
        //                }
        //                break;
        //            case "DJF_DJ_SF":
        //                pid = (entity as DJF_DJ_SF).PID;
        //                DJF_DJ_SF tmpDJF_DJ_SF = sjdb.Fetch<DJF_DJ_SF>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_SF != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_SF);
        //                    sjdb.Update(tmpDJF_DJ_SF);
        //                }
        //                break;
        //            case "DJF_DJ_SH":
        //                pid = (entity as DJF_DJ_SH).PID;
        //                DJF_DJ_SH tmpDJF_DJ_SH = sjdb.Fetch<DJF_DJ_SH>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_SH != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_SH);
        //                    sjdb.Update(tmpDJF_DJ_SH);
        //                }
        //                break;
        //            case "DJF_DJ_SZ":
        //                pid = (entity as DJF_DJ_SZ).PID;
        //                DJF_DJ_SZ tmpDJF_DJ_SZ = sjdb.Fetch<DJF_DJ_SZ>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_SZ != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_SZ);
        //                    sjdb.Update(tmpDJF_DJ_SZ);
        //                }
        //                break;
        //            case "DJF_DJ_FZ":
        //                pid = (entity as DJF_DJ_FZ).PID;
        //                DJF_DJ_FZ tmpDJF_DJ_FZ = sjdb.Fetch<DJF_DJ_FZ>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_FZ != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_FZ);
        //                    sjdb.Update(tmpDJF_DJ_FZ);
        //                }
        //                break;
        //            case "DJF_DJ_GD":
        //                pid = (entity as DJF_DJ_GD).PID;
        //                DJF_DJ_GD tmpDJF_DJ_GD = sjdb.Fetch<DJF_DJ_GD>("PID", pid).FirstOrDefault();
        //                if (tmpDJF_DJ_GD != null)
        //                {
        //                    CopySameField(entity, tmpDJF_DJ_GD);
        //                    sjdb.Update(tmpDJF_DJ_GD);
        //                }
        //                break;
        //            case "KTT_ZHJBXX":
        //                pid = (entity as KTT_ZHJBXX).PID;
        //                KTT_ZHJBXX tmpKTT_ZHJBXX = sjdb.Fetch<KTT_ZHJBXX>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_ZHJBXX != null)
        //                {
        //                    CopySameField(entity, tmpKTT_ZHJBXX);
        //                    sjdb.Update(tmpKTT_ZHJBXX);
        //                }
        //                break;
        //            case "KTF_ZH_YHZK":
        //                pid = (entity as KTF_ZH_YHZK).PID;
        //                KTF_ZH_YHZK tmpKTF_ZH_YHZK = sjdb.Fetch<KTF_ZH_YHZK>("PID", pid).FirstOrDefault();
        //                if (tmpKTF_ZH_YHZK != null)
        //                {
        //                    CopySameField(entity, tmpKTF_ZH_YHZK);
        //                    sjdb.Update(tmpKTF_ZH_YHZK);
        //                }
        //                break;
        //            case "KTF_ZHBHQK":
        //                pid = (entity as KTF_ZHBHQK).PID;
        //                KTF_ZHBHQK tmpKTF_ZHBHQK = sjdb.Fetch<KTF_ZHBHQK>("PID", pid).FirstOrDefault();
        //                if (tmpKTF_ZHBHQK != null)
        //                {
        //                    CopySameField(entity, tmpKTF_ZHBHQK);
        //                    sjdb.Update(tmpKTF_ZHBHQK);
        //                }
        //                break;
        //            case "KTF_ZH_YHYDZB":
        //                pid = (entity as KTF_ZH_YHYDZB).PID;
        //                KTF_ZH_YHYDZB tmpKTF_ZH_YHYDZB = sjdb.Fetch<KTF_ZH_YHYDZB>("PID", pid).FirstOrDefault();
        //                if (tmpKTF_ZH_YHYDZB != null)
        //                {
        //                    CopySameField(entity, tmpKTF_ZH_YHYDZB);
        //                    sjdb.Update(tmpKTF_ZH_YHYDZB);
        //                }
        //                break;
        //            case "KTT_GY_JZX":
        //                pid = (entity as KTT_GY_JZX).PID;
        //                KTT_GY_JZX tmpKTT_GY_JZX = sjdb.Fetch<KTT_GY_JZX>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_GY_JZX != null)
        //                {
        //                    CopySameField(entity, tmpKTT_GY_JZX);
        //                    sjdb.Update(tmpKTT_GY_JZX);
        //                }
        //                break;
        //            case "KTT_GY_JZD":
        //                pid = (entity as KTT_GY_JZD).PID;
        //                KTT_GY_JZD tmpKTT_GY_JZD = sjdb.Fetch<KTT_GY_JZD>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_GY_JZD != null)
        //                {
        //                    CopySameField(entity, tmpKTT_GY_JZD);
        //                    sjdb.Update(tmpKTT_GY_JZD);
        //                }
        //                break;
        //            case "KTT_FW_LJZ":
        //                pid = (entity as KTT_FW_LJZ).PID;
        //                KTT_FW_LJZ tmpKTT_FW_LJZ = sjdb.Fetch<KTT_FW_LJZ>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_FW_LJZ != null)
        //                {
        //                    CopySameField(entity, tmpKTT_FW_LJZ);
        //                    sjdb.Update(tmpKTT_FW_LJZ);
        //                }
        //                break;
        //            case "KTT_FW_C":
        //                pid = (entity as KTT_FW_C).PID;
        //                KTT_FW_C tmpKTT_FW_C = sjdb.Fetch<KTT_FW_C>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_FW_C != null)
        //                {
        //                    CopySameField(entity, tmpKTT_FW_C);
        //                    sjdb.Update(tmpKTT_FW_C);
        //                }
        //                break;
        //            case "KTT_FW_H":
        //                pid = (entity as KTT_FW_H).PID;
        //                KTT_FW_H tmpKTT_FW_H = sjdb.Fetch<KTT_FW_H>("PID", pid).FirstOrDefault();
        //                if (tmpKTT_FW_H != null)
        //                {
        //                    CopySameField(entity, tmpKTT_FW_H);
        //                    sjdb.Update(tmpKTT_FW_H);
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        LogClass.Instance.WriteLogFile(ex.ToString()); // add by cfl 2018年4月19日
        //        throw ex;
        //    }
        //    //return result;
        //}
    //}
}
