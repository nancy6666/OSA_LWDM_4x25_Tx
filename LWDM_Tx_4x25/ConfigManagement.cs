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
        public string BertCom
        {
            get
            {
                return cfg.AppSettings.Settings["BertCom"].Value;
            }
        }
        public string T720Com
        {
            get
            {
                return cfg.AppSettings.Settings["T720Com"].Value;
            }
        }
        public string LDT5525BCom
        {
            get
            {
                return cfg.AppSettings.Settings["LDT5525BCom"].Value;
            }
        }
        public string PM212Com
        {
            get
            {
                return cfg.AppSettings.Settings["PM212Com"].Value;
            }
        }
        public string JW8402Com
        {
            get
            {
                return cfg.AppSettings.Settings["JW8402Com"].Value;
            }
        }
        public string K2400_1_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K2400_1_GPIB"].Value;
            }
        }
        public string K2400_2_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K2400_2_GPIB"].Value;
            }
        }
        public string K2400_3_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K2400_3_GPIB"].Value;
            }
        }
        public string K2000_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K2000_GPIB"].Value;
            }
        }
        public string K7001_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K7001_GPIB"].Value;
            }
        }
        public string AQ6370_GPIB
        {
            get
            {
                return cfg.AppSettings.Settings["AQ6370_GPIB"].Value;
            }
        }
    }
}
