using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTest_Tx_PAM4_50G
{
  public  class CTestDataCommon
    {
        public int ID
        {
            get;
            set;
        }
        public int Spec_id
        {
            get;
            set;
        }
        public string SN
        {
            get;
            set;
        }
        public string Test_station
        {
            get;
            set;
        }
        public string Test_Date
        {
            get;
            set;
        }
        public string Operator
        {
            get;
            set;
        }
        public int TC_count
        {
            get;
            set;
        }
        public double Vcc1
        {
            get;
            set;
        }
        public double Vcc2
        {
            get;
            set;
        }
        public double Ppg_data_rate
        {
            get;
            set;
        }
        public int Ppg_channel
        {
            get;
            set;
        }
        public string Ppg_pattern
        {
            get;
            set;
        }
      
        public double Pre_cursor
        {
            get;
            set;
        }
        public double Main_cursor
        {
            get;
            set;
        }
        public double Post_cursor
        {
            get;
            set;
        }
        public double Inner_1
        {
            get;
            set;
        }
        public double Inner_2
        {
            get;
            set;
        }
        public UInt16 TxTEC
        {
            get;
            set;
        }
        public UInt16 RxVPD
        {
            get;
            set;
        }
        public UInt16 TxVB
        {
            get;
            set;
        }
        public UInt16 TxVEA
        {
            get;
            set;
        }
        public UInt16 TxVG
        {
            get;
            set;
        }
        public UInt16 LDBias
        {
            get;
            set;
        }
        public UInt16 RxVGC
        {
            get;
            set;
        }
        public bool Pf
        {
            get;
            set;
        }
        public List<CTestData> lstTestData = new List<CTestData>();
        
    }
}
