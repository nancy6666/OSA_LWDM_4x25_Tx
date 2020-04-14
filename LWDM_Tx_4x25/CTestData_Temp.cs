using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
{
   public class CTestData_Temp
    {
        public int Common_id;
        public double Temp_out;
        public double Temp_in;
        public double Vcc1;
        public double Vcc2;
        public double Vcc3;
        public double Icc1;
        public double Icc2;
        public double Icc3;
        public double Itec;
        public double Vtec;
        public double Ptec;
        public bool Pf;
        public List<CTestData_Channel> lstTestData_Channel = new List<CTestData_Channel>();

    }
}
