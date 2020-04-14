using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTest_Tx_PAM4_50G
{
   public class CTestData
    {
        #region Properties

       public int Common_id
        {
            get;
            set;
        }
        public string Test_start_time
        {
            set;
            get;
        }
        public string Test_end_time
        {
            set;
            get;
        }

        public double Temp_case
        {
            get;
            set;
        }
        public double Io
        {
            get;
            set;
        }
        public double EA_Voltage
        {
            get;
            set;
        }
        public double Temp_Coc
        {
            get;
            set;
        }
        public double CWL
        {
            get;
            set;
        }
        public double SMSR
        {
            get;
            set;
        }
        public double TDECQ
        {
            get;
            set;
        }
        public double OuterER
        {
            get;
            set;
        }

        public double OuterOMA
        {
            get;
            set;
        }
        public double Lanch_power
        {
            get;
            set;
        }
      
        public double Linearity
        {
            get;
            set;
        }
       
      public bool Pf
        {
            get;
            set;
        }
        #endregion
    }
}
