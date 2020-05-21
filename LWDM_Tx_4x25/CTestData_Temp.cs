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
        private double _icc1;
        public double Icc1
        {
            get
            {
                return _icc1;
            }
            set
            {
                _icc1 = Math.Round(value, 2);
            }
        }
        private double _icc2;
        public double Icc2
        {
            get
            {
                return _icc2;
            }
            set
            {
                _icc2 = Math.Round(value, 2);
            }
        }
        private double _icc3;
        public double Icc3
        {
            get
            {
                return _icc3;
            }
            set
            {
                _icc3 = Math.Round(value, 2);
            }
        }
        private double _itec;
        public double Itec
        {
            get
            {
                return _itec;
            }
            set
            {
                _itec = Math.Round(value, 2);
            }
        }
        private double _vtec;
        public double Vtec
        {
            get
            {
                return _vtec;
            }
            set
            {
                _vtec = Math.Round(value, 2);
            }
        }
        private double _ptec;
        public double Ptec
        {
            get
            {
                return _ptec;
            }
            set
            {
                _ptec = Math.Round(value, 2);
            }
        }
        public bool Pf;
        public List<CTestData_Channel> lstTestData_Channel = new List<CTestData_Channel>();

    }
}
