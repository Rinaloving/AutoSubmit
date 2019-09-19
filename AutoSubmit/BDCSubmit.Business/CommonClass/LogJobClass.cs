using BDCSubmit.Business.SubmitModel;
using NPoco;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDCSubmit.Business.CommonClass
{
    public class LogJobClass : IJob
    {
        /// <summary>
        /// 同步4县日志数据
        /// </summary>
        /// <param name="context"></param>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
              await RunExchangeAccesslog();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task RunExchangeAccesslog()
        {
            List<City> lstCity = SystemHandler.Instance.GetConnectionCitys();
            foreach (var item in lstCity)
            {
                await ExchangeAccesslog(item);
            }
        }

        IDatabase db = null;
        IDatabase sjdb = null;
        private async Task ExchangeAccesslog(City city)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            string citycode = city.CityCode;
            try
            {
                
                switch (city.CityCode)
                {
                    case "371423":
                        db = new Database("qysubmit");
                        break;
                    case "371425":
                        db = new Database("qhsubmit");
                        break;
                    case "371426":
                        db = new Database("pysubmit");
                        break;
                    case "371427":
                        db = new Database("xjsubmit");
                        break;
                    default:
                        break;
                }
                string strToday = DateTime.Today.ToString("yyyyMMdd");
                string qryRegisterInfo = string.Format("  where areacode='{0}' and accessdate='{1}' and logtype='RegisterInfo'", city.CityCode, strToday);
                string qryAccessInfo = string.Format(" where areacode='{0}' and accessdate='{1}' and logtype='AccessInfo'", city.CityCode, strToday);
                ACCESSLOG RegisterInfo = db.Fetch<ACCESSLOG>(qryRegisterInfo).FirstOrDefault();
                sjdb = new Database("sjsubmit");
                if (RegisterInfo != null)
                {
                    RegisterInfo.PID = Guid.NewGuid().ToString("N");
                    sjdb.Insert("ACCESSLOG","PID",false,RegisterInfo);
                }
                ACCESSLOG AccessInfo = db.Fetch<ACCESSLOG>(qryAccessInfo).FirstOrDefault();
                if (AccessInfo != null)
                {
                    AccessInfo.PID = Guid.NewGuid().ToString("N");
                    sjdb.Insert("ACCESSLOG", "PID", false, AccessInfo);
                }
            }
            catch (Exception ex)
            {
                LogClass.Instance.WriteLogFile(citycode + "向市局同步日志报文错误，信息:" + ex.ToString());
            }
        }

        
    }
}
