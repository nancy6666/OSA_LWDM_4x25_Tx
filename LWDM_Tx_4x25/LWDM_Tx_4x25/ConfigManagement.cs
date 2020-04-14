using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RFTest_Tx_PAM4_50G
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
        public string TC720Com
        {
            get
            {
                return cfg.AppSettings.Settings["TC720"].Value;
            }
        }
        public string DP832ACom
        {
            get
            {
                return cfg.AppSettings.Settings["DP832A"].Value;
            }
        }

        public string BertCom
        {
            get
            {
                return cfg.AppSettings.Settings["Bert"].Value;
            }
        }

        public int MS9740AGPIB
        {
            get
            {
                return Convert.ToInt32(cfg.AppSettings.Settings["MS9740A"].Value);
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
