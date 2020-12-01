using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
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
        public string Test_Start_Time
        {
            get;
            set;
        }
        public string Test_Stop_Time
        {
            get;
            set;
        }
        public string Operator
        {
            get;
            set;
        }
        public string Rate
        {
            get;set;
        }
        public bool Pf = true;
        public List<CTestData_Temp> lstTestData_Temp = new List<CTestData_Temp>();
        
    }
}
