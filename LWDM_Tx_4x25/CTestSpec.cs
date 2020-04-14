using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
{
    public class CTestSpec
    {
        #region Properties
        public int ID
        {
            get;
            set;
        }
        public string PN
        {
            get;
            set;
        }
        public int version;
       
        public double Power_min;
        public double Power_max;
        public double Impd_min;
        public double Impd_max;
        public double Idark_min;
        public double Idark_max;
        public double Cwl_min;
        public double Cwl_max;
        public double SMSR_min;
        public double SMSR_max;
        public double Jitter_pp_min;
        public double Jitter_pp_max;
        public double Jitter_rms_min;
        public double Jitter_rms_max;
        public double Crossing_min;
        public double Crossing_max;
        public double Fall_time_min;
        public double Fall_time_max;
        public double Rise_time_min;
        public double Rise_time_max;
        public double Er_min;
        public double Er_max;
        public double Mask_margin_min;
        public double Mask_margin_max;
        public double Itec_min;
        public double Itec_max;
        public double Vtec_min;
        public double Vtec_max;
        public double Ptec_min;
        public double Ptec_max;
        #endregion
    }
}
