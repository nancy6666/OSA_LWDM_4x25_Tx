using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
  public class PM212
    {
        protected SerialPort comPort;

        public double Power_Offset;
        public PM212(string com)
        {
            try
            {
                comPort = new SerialPort(com, 115200, Parity.None, 8, StopBits.One);
                try
                {
                    if (comPort.IsOpen)
                        comPort.Close();
                    comPort.Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"PM212 初始化出错，{ex.Message}");
            }
            SetWavelength(1.31E-6);
        }

        public double ReadPower()
        {
            double power = Convert.ToDouble(Query(":POWER?"));
            return ConvertWdBm(power);
        }

        /// <summary>
        /// 设置功率计波长
        /// </summary>
        /// <param name="waveMeter">单位为m</param>
        private void SetWavelength(double waveMeter)
        {
            string strCmd = "";
            strCmd = $"WAVELENGTH {waveMeter}";
            if( (bool)Excute(strCmd))
            {
                throw new Exception("设置功率计波长出错！");
            }
        }

        private object Excute(object objCmd)
        {
            try
            {
                lock (comPort)
                {
                    // comPort.Write("*IDN?\r\n");
                    // var test = comPort.ReadLine();
                    comPort.WriteLine(objCmd.ToString());
                    Thread.Sleep(50);
                    comPort.WriteLine(":SYST:ERR?");
                    string strErr = comPort.ReadLine();
                    Thread.Sleep(50);
                    return strErr != "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private object Query(object objCmd)
        {
            try
            {
                lock (comPort)
                {
                    Thread.Sleep(50);
                    comPort.WriteLine(objCmd.ToString());
                    Thread.Sleep(50);
                    return comPort.ReadLine().Replace("\r", "").Replace("\n", "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double ConvertWdBm(double pw)
        {
            return 10 * Math.Log10(pw * 1000);
        }
    }
}
