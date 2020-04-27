using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.NI4882;
using System.Threading;
namespace LWDM_Tx_4x25.Instruments
{
  public  class Keithley7001 
    {
        private GPIB GPIBDevice;

        public Keithley7001(int GPIBaddr)
        {
            try
            {
                GPIBDevice = new GPIB(GPIBaddr);
            }
            catch(Exception ex)
            {
                throw new Exception($"Open K7001 error!{ex.Message}");
            }
        }

        public  object Query(string objCmd)
        {
            try
            {
                GPIBDevice.GPIBwr(objCmd);
                string res = GPIBDevice.GPIBrd(200);

                return res; 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public void CloseRelay(int pin1,int pin2)
        {
            string strCmd = String.Format(":clos (@ {0}!{1})", pin1, pin2);
           GPIBDevice.GPIBwr(strCmd);
        }
       
        public void CloseRelay(string pin)
        {
            try
            {
                string strCmd = String.Format($":clos (@ {pin})");
                GPIBDevice.GPIBwr(strCmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"K7001关闭继电器{pin}时出错！{ex.Message}");
            }
}
        public void OpenRelay(int pin1, int pin2)
        {
            string strCmd = String.Format(":open (@ {0}!{1})", pin1, pin2);
            GPIBDevice.GPIBwr(strCmd);
        }
        public void OpenRelay(string strPin)
        {
            string strCmd = String.Format($":open (@ {strPin})");
            GPIBDevice.GPIBwr(strCmd);
        }
        public void OpenAllRelay()
        {
            try
            {
                string strCmd = String.Format("*RST;:open all");
                GPIBDevice.GPIBwr(strCmd);
            }
            catch(Exception ex)
            {
                throw new Exception($"K7001打开所有继电器时出错！{ex.Message}");

            }
        }

    }
}
