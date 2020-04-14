using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTest_Tx_PAM4_50G
{
  public  class CTestSpec
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
        public double CWL_Min
        {
            get;
            set;
        }
        public double CWL_Max
        {
            get;
            set;
        }
        public double SMSR_Min
        {
            get;
            set;
        }
        public double SMSR_Max
        {
            get;
            set;
        }
        public double TDECQ_Min
        {
            get;
            set;
        }
        public double TDECQ_Max
        {
            get;
            set;
        }
        public double Outer_OMA_Min
        {
            get;
            set;
        }
        public double Outer_OMA_Max
        {
            get;
            set;
        }
        public double Lanch_Power_Min
        {
            get;
            set;
        }
        public double Lanch_Power_Max
        {
            get;
            set;
        }
        public double Outer_ER_Min
        {
            get;
            set;
        }
        public double Outer_ER_Max
        {
            get;
            set;
        }
        public double Linearity_Min
        {
            get;
            set;
        }
        public double Linearity_Max
        {
            get;
            set;
        }
       
        #endregion
    }
}
