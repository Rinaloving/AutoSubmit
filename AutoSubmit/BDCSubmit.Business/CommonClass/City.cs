using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDCSubmit.Business.CommonClass
{
    public class City
    {
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public string DataSource { get; set; }
        public string Catalog { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }



        public string BizMsgPath { get; set; }
        public string RepMsgPath { get; set; }
        public string LinuxBizMsgPath { get; set; }
        public string LinuxRepMsgPath { get; set; }
    }
}
