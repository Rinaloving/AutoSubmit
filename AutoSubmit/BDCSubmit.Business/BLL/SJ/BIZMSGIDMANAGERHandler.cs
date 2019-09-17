using BDCSubmit.Business.SubmitModel;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDCSubmit.Business.BLL.SJ
{

    public class BIZMSGIDMANAGERHandler
    {
        #region 声明
        private static BIZMSGIDMANAGERHandler mInstance = null;

        private static readonly object lockAssistant = new object();

        public static BIZMSGIDMANAGERHandler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                        {
                            mInstance = new BIZMSGIDMANAGERHandler();
                        }
                    }
                }
                return mInstance;
            }
        }

        private BIZMSGIDMANAGERHandler()
        { }
        #endregion
        IDatabase sjdb = null;
        public string GetBizMsgID(string qxdm, string yymmdd)
        {
            sjdb = new Database("sjsubmit");
            string BizMsgID = "";
            try
            {
                BIZMSGIDMANAGER idObj = sjdb.Fetch<BIZMSGIDMANAGER>(" where QXDM='" + qxdm + "' and YYMMDD='" + yymmdd + "'").FirstOrDefault();
                if (idObj == null)
                {
                    //新增
                    idObj = new BIZMSGIDMANAGER();
                    idObj.PID = Guid.NewGuid().ToString("N");
                    idObj.QXDM = qxdm;
                    idObj.YYMMDD = yymmdd;
                    idObj.FLOWID = 1;
                    sjdb.Insert<BIZMSGIDMANAGER>("BIZMSGIDMANAGER", "PID", false, idObj);
                }
                else
                {
                    //更新最大号
                    idObj.FLOWID++;
                    sjdb.Update(idObj);
                }
                BizMsgID = qxdm.ToString() + yymmdd.ToString() + idObj.FLOWID.ToString().PadLeft(6, '0');
                return BizMsgID;
            }
            catch (Exception ex)
            {
                return BizMsgID;
            }
        }


    }
}
