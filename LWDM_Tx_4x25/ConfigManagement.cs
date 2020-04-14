using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LWDM_Tx_4x25
{
    public class ConfigManagement
    {
        Configuration cfg;
        public ConfigManagement()
        {
            cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
        public string DBConnectString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
            }
        }
        
        public string DCAN1092Path
        {
            get
            {
                return cfg.AppSettings.Settings["DCAN1092Path"].Value;
            }
        }
       
    }
}
