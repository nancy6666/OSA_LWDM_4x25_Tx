using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFTest_Tx_PAM4_50G.Instruments
{
    public class MS9740A
    {
        private GPIB MS9740;
        public enum DetectMethod { _2NDPEAK, LEFT, RIGHT }
        public double SMSR { get; set; }
        public double CWL { get; set; }
        public MS9740A(int GPIBaddr)
        {
            MS9740 = new GPIB(GPIBaddr);
        }
        public void ReadTestData()
        {
            try
            {
                //sets the envelop method and cut level
                MS9740.GPIBwr("AP DFB");
                MS9740.GPIBwr("*CLS");
                //start single sweep
                MS9740.GPIBwr("SSI");
                //waiting for completion
                MS9740.GPIBwr("*OPC?");
                MS9740.GPIBrd(200);
                //acquire result
                MS9740.GPIBwr("APR?");
                string str = MS9740.GPIBrd(200);
                string[] strs = str.Split(',');
                this.SMSR = Convert.ToDouble(strs[0]);
                this.CWL = Convert.ToDouble(strs[2]);

            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when reading Test data from MS9740A.\r\n" + e.Message);
            }
        }

        /// <summary>
        /// sets the SMSR method and detecting method
        /// </summary>
        /// <param name="detectMethod"></param>
        private void SetSMSR(DetectMethod detectMethod)
        {
            switch (detectMethod)
            {
                case DetectMethod.LEFT:
                    MS9740.GPIBwr("ANA SMSR,LEFT");
                    break;
                case DetectMethod.RIGHT:
                    MS9740.GPIBwr("ANA SMSR,RIGHT");
                    break;
                case DetectMethod._2NDPEAK:
                    MS9740.GPIBwr("ANA SMSR,2NDPEAK");
                    break;
            }

        }
    }
}
