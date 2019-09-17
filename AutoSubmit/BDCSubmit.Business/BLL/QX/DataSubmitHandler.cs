using BDCSubmit.Business.BLL.Common;
using BDCSubmit.Business.CommonClass;
using BDCSubmit.Business.SubmitModel;
using NPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BDCSubmit.Business.BLL.QX
{
    public class DataSubmitHandler
    {
        #region 声明
        private static DataSubmitHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static DataSubmitHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new DataSubmitHandler();
                        }
                    }
                }
                return mInstance;
            }
        }

        private DataSubmitHandler()
        { }
        #endregion

        //IDbGUIDOperator curContext = null;
        //IBaseOperator baseHelper = null;
        //IDbProvider provider = null;

        //IDbGUIDOperator SJContext = null;
        //IBaseOperator SJbaseHelper = null;
        //IDbProvider SJprovider = null;
        string qxdm = "";
        /// <summary>
        /// 陵城区有2个区县代码，老的371421，新的371403
        /// </summary>
        string lcqxdm = "";
        IDatabase db = null;
        IDatabase sjdb = null;
        /// <summary>
        /// 数据上报，只上报BIZANDREP.status为1的数据
        /// </summary>
        /// <param name="qx"></param>
        public void DataSubmit(QXType qx, ExcuteTaskType et)
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
                sjdb = new Database("sjsubmit");
                int istatus = (int)STATUSType.DefaultValue;
                string qrySQL = @"select * from RNANDCN t where exists (select * from BIZANDREP s where s.qxdm='" + qxdm + "' and s.status=" + istatus + " and s.ywh=t.ywh)  and t.qxdm='" + qxdm + "' ";
                //List<BIZANDREP> lstBAR = curContext.GetList<BIZANDREP>(" where qxdm='" + qxdm + "' and status=" + istatus + "").ToList();
                List<RNANDCN> lstRC = sjdb.Fetch<RNANDCN>(qrySQL);// SJbaseHelper.ConvertToList<RNANDCN>(SJbaseHelper.ExecuteDataReader(SJprovider, qrySQL)).ToList();
                                                                  // SJprovider.Close();
                if (!string.IsNullOrEmpty(lcqxdm))
                {
                    if (lstRC != null)
                    {
                        string qrySQL2 = @"select * from RNANDCN t where exists (select * from BIZANDREP s where s.qxdm='" + lcqxdm + "' and s.status=" + istatus + " and s.ywh=t.ywh)  and t.qxdm='" + lcqxdm + "' ";
                        List<RNANDCN> lstRC2 = sjdb.Fetch<RNANDCN>(qrySQL);//SJbaseHelper.ConvertToList<RNANDCN>(SJbaseHelper.ExecuteDataReader(SJprovider, qrySQL2)).ToList();
                                                                           // SJprovider.Close();
                        if (lstRC2 != null)
                            lstRC.AddRange(lstRC2);
                    }
                }
                CheckBusinessTables cbt = XMLHelper.DeserializeByXmlFilePath<CheckBusinessTables>(SystemHandler.CheckBusinessTablesXMLFilePath);

                if (lstRC != null && lstRC.Count > 0)
                {
                    int totalMain = lstRC.Count;
                    if (et == ExcuteTaskType.Show)
                    {
                        //sdf = new ShowDialogForm("提示", "正在生成上报数据...", "请稍候");
                        //sdf.SetProgress(totalMain);
                    }
                    foreach (var rc in lstRC)
                    {
                        Head hd = null;
                        string xmlPath = "";
                        try
                        {

                            //
                            BusinessTypeClass btc = cbt.Business.Where(p => rc.JRYWBM.Equals(p.BusinessCode)).FirstOrDefault();
                            if (et == ExcuteTaskType.Show) { }
                                //sdf.SetMessage("正在生成业务号:" + rc.YWH + " 报文数据");
                            if (btc != null)
                            {
                                List<TableClass> lstTables = btc.检查表.Where(p => p.是否必选).ToList();
                                if (lstTables != null)
                                {

                                    hd = HeadHandler.Instance.ConstructHead(rc);
                                    List<dynamic> lstEntitys = new List<dynamic>();
                                    dynamic entity = null;
                                    Assembly pAssembly = Assembly.LoadFrom(SystemHandler.assPath);

                                    foreach (var item in lstTables)
                                    {
                                        entity = pAssembly.CreateInstance("BDCSubmit.Business.SubmitModel." + item.表名);
                                        if (et == ExcuteTaskType.Show) { }
                                            //sdf.SetContentEX("正在生成表:" + item.表名 + "数据");
                                        lstEntitys.AddRange(GetSJInstance(item.表名, rc,db));
                                    }
                                    //生成xml
                                    //1
                                    XElement ele = XMLHelper.CreateBizEX<dynamic>(hd, lstEntitys);

                                    //2
                                    XElement root = XElement.Load(SystemHandler.configFilePath);
                                    //3

                                    List<City> lstCity = SystemHandler.Instance.GetGeneralCitys();
                                    City city = lstCity.Where(p => qxdm.Equals(p.CityCode)).FirstOrDefault();

                                    if (city != null)
                                    {
                                        string BizMsgPath = city.BizMsgPath;
                                        if (!string.IsNullOrEmpty(BizMsgPath) && Directory.Exists(BizMsgPath))
                                        {
                                            xmlPath = BizMsgPath + "/" + "Biz" + hd.BizMsgID + ".xml";
                                            ele.Save(xmlPath);
                                            HeadHandler.Instance.CreateMsgWithSignature(xmlPath);  //每次修改完，这要重新保存一下
                                            //修改市级 BIZANDREP.STATUS值
                                            BIZANDREP bar = sjdb.Fetch<BIZANDREP>(" where qxdm='" + qxdm + "' and ywh='" + rc.YWH + "'").FirstOrDefault();
                                            bar.STATUS = (int)STATUSType.Submiting;
                                            bar.BIZFILEPATH = xmlPath;

                                            bar.BIZMSGID = hd.BizMsgID;
                                            sjdb.Update(bar);
                                            //修改区县 BIZANDREP.STATUS值
                                            BIZANDREP qxbar = db.Fetch<BIZANDREP>(" where qxdm='" + qxdm + "' and ywh='" + rc.YWH + "'").FirstOrDefault();
                                            if (qxbar != null)
                                            {
                                                qxbar.STATUS = (int)STATUSType.Submiting;
                                                qxbar.BIZMSGID = hd.BizMsgID;
                                                db.Update(qxbar);
                                            }
                                            if (et == ExcuteTaskType.Show) { }
                                                //sdf.SetMessageEX("生成业务号:" + rc.YWH + " 报文数据完成");
                                        }
                                    }

                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                //删除生成的报文
                                if (!string.IsNullOrEmpty(xmlPath) && File.Exists(xmlPath))
                                    File.Delete(xmlPath);

                                //
                            }
                            catch
                            {
                                throw ex;
                            }
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
        public List<dynamic> GetSJInstance(string tableName, RNANDCN rc, IDatabase db)
        {
            dynamic entity = null;
            string sql = CheckDataHandler.Instance.GetQuerySQL(tableName, rc, QueryType.all);
            Assembly pAssembly = Assembly.LoadFrom(SystemHandler.assPath);


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
        //public BizMode GetSJInstanceExtra(string tableName, RNANDCN rc)
        //{
        //    IEntity entity = null;
        //    string sql = CheckDataHandler.Instance.GetQuerySQL(tableName, rc, QueryType.all);
        //    Assembly pAssembly = Assembly.LoadFrom(SystemHandler.assPath);
        //    entity = pAssembly.CreateInstance("BDCSubmit.Business.Model." + tableName) as IEntity;
        //    IEntity qxEntity = baseHelper.ConvertToEntity(entity.GetType(), baseHelper.ExecuteDataReader(provider, sql)) as IEntity;
        //    provider.Close(); // add by cfl 2018年4月20日
        //    BizMode model = pAssembly.CreateInstance("BDCSubmit.Business.Model." + tableName + "BizModel") as BizMode;
        //    ConvertObject.CopySameField(qxEntity, model);
        //    return model;
        //}

//        public DataTable GetData(QXType qx)
//        {
//            if (qx == QXType.None) return null;
//            try
//            {
//                string qxdm = "";
//                switch (qx)
//                {
//                    case QXType.QY:
//                        qxdm = "371423";
//                        break;
//                    case QXType.QH:

//                        qxdm = "371425";
//                        break;
//                    case QXType.PY:
//                        qxdm = "371426";
//                        break;
//                    case QXType.XJ:
//                        qxdm = "371427";
//                        break;
//                    case QXType.DC:
//                        qxdm = "371402";
//                        break;
//                    case QXType.LC:
//                    case QXType.LX:
//                        qxdm = "371421"; lcqxdm = "371403";
//                        break;
//                    case QXType.NJ:
//                        qxdm = "371422";
//                        break;
//                    case QXType.LY:
//                        qxdm = "371424";
//                        break;
//                    case QXType.WC:
//                        qxdm = "371428";
//                        break;
//                    case QXType.LL:
//                        qxdm = "371481";
//                        break;
//                    case QXType.YC:
//                        qxdm = "371482";
//                        break;

//                    default:
//                        break;
//                }
//                int isfsb = (int)STATUSType.DefaultValue;
//                DataTable dtData = null;
//                string qrySQL = @"select t.realeunum,t.ywh,t.scywh,t.jrywbm,t.qxdm,t.createtime,
//case s.status when 1 then '默认值(未上报)' 
//              when 2 then '上报中,未回复'
//              when 3 then '已经回复,部里检查通过'
//              when 4 then '已经回复,部里检查未通过'
//              else '默认值(未上报)' end as sjzt
// from RNANDCN t inner join BIZANDREP s on t.ywh=s.ywh and t.qxdm=s.qxdm and t.qxdm='" + qxdm + "' and s.status=" + isfsb + "";
//                DataSet ds = SystemHandler.Instance.SJCommunicationContext.ExecuteQuery(qrySQL);
//                if (ds != null && ds.Tables.Count > 0)
//                    dtData = ds.Tables[0];
//                if (!string.IsNullOrEmpty(lcqxdm))
//                {
//                    DataTable dtData2 = null;
//                    string qrySQL2 = @"select t.realeunum,t.ywh,t.scywh,t.jrywbm,t.qxdm,t.createtime,
//case s.status when 1 then '默认值(未上报)' 
//              when 2 then '上报中,未回复'
//              when 3 then '已经回复,部里检查通过'
//              when 4 then '已经回复,部里检查未通过'
//              else '默认值(未上报)' end as sjzt
// from RNANDCN t inner join BIZANDREP s on t.ywh=s.ywh and t.qxdm=s.qxdm and t.qxdm='" + lcqxdm + "' and s.status=" + isfsb + "";
//                    DataSet ds2 = SystemHandler.Instance.SJCommunicationContext.ExecuteQuery(qrySQL2);
//                    if (ds2 != null && ds2.Tables.Count > 0)
//                        dtData2 = ds2.Tables[0];
//                    dtData2.Rows.Cast<DataRow>().ToList().ForEach(p => dtData.ImportRow(p));
//                }

//                return dtData;
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }
//        }
    }
}
