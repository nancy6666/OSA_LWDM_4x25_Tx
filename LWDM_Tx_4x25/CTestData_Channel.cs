using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
{
    public class CTestData_Channel
    {
        #region Properties

        public int Temp_id;
        public int Channel;
        public double Vcpa;
        public double Veq;
        public double Vmod;
        public double Isink;
        public double Ldd;
        public double Power;
        public double Impd;
        public double Idark;
        public double Cwl;
        public double SMSR;
        public double Jitter_pp;
        public double Jitter_rms;
        public double Crossing;
        public double Fall_time;
        public double Rise_time;
        public double Er;
        public double Mask_Margin;

        public bool Pf
        {
            get;
            set;
        }
        #endregion
    }
}
