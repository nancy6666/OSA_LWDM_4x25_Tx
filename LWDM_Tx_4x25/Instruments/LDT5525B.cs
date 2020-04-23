using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
   public class LDT5525B
    {
        protected SerialPort comPort;
        public int StablizationTime;
        public int TimeOut;
        public double TempSpan;
        public double TempOffset;
        enum gainValue
        {
            g1 = 1,
            g3 = 3,
            g10 = 10,
            g30 = 30,
            g100 = 100,
            g300 = 300
        }
     public   enum EnumTECMode
        {
            Current,
            Temperature,
            Resistance
        }

        public LDT5525B(string com)
        {
            try
            {
                comPort = new SerialPort(com, 115200, Parity.None, 8, StopBits.One);
                //comPort.ReadTimeout = 1000;
                //comPort.WriteTimeout = 1000;
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
                throw new Exception($"Open LDT5525B error,{ex.Message}");
            }
        }

        public double ReadTemperature()
        {
            double temp = 0;
            lock (comPort)
            {
                string cmd = "TEC:T?";
                var ret = Query(cmd);
                while (!double.TryParse(ret, out temp))
                {
                    Thread.Sleep(100);
                    ret = Query(cmd);
                    //temp = 0.00f;
                    // throw new InvalidCastException("the Product temperature read from LDT5525B is invalid");
                }
            }
            return temp;
        }

        public void SetTemperature(double temp)
        {
            lock (comPort)
            {
                string cmd = $"TEC:T {temp}";
                Excute(cmd);
            }
            SetOutputStatus(true);
        }

        /// <summary>
        /// Set the status of the TEC output.
        /// </summary>
        /// <param name="on"></param>
        public void SetOutputStatus(bool on)
        {
            string cmd = $"TEC:OUT {Convert.ToInt32(on)}";
            Excute(cmd);
        }
        /// <summary>
        /// set Temperature limit
        /// </summary>
        /// <param name="limiT"></param>
        public void SetLIMI_T(double limiT)
        {
            lock (comPort)
            {
                string cmd = $"TEC:LIM:THI {limiT}";
                Excute(cmd);
            }
        }
        /// <summary>
        /// set current limit
        /// </summary>
        /// <param name="limiI"></param>
        public void SetLIMI_I(double limiI)
        {
            lock (comPort)
            {
                string cmd = $"TEC:LIM:ITE {limiI}";
               Excute(cmd);
            }
        }
        public double ReadCurrent()
        {
            double current = 0;
            lock (comPort)
            {
                string cmd = "TEC:ITE?";
                if (!double.TryParse(Query(cmd), out current))
                   {
                    current = 0.00f;
                    throw new InvalidCastException("LDT5525B current is invalid");
                }
            }
            return current;
        }

        #region Private Methods

        /// <summary>
        /// Sets TEC mode.
        /// </summary>
        /// <param name="mode">three mode type </param>
        public void SetMode(EnumTECMode mode)
        {
            string cmd = "";
            lock (comPort)
            {
                switch (mode)
                {
                    case EnumTECMode.Current:
                     cmd = "TEC:MODE:ITE";
                        break;
                    case EnumTECMode.Resistance:
                        cmd = "TEC:MODE:R";
                        break;
                    case EnumTECMode.Temperature:
                        cmd = "TEC:MODE:T";
                        break;

                }
                Excute(cmd);
            }
        }

        private void SetGain(gainValue gain)
        {
            lock (comPort)
            {
                string cmd = $"TEC:GAIN {gain}";
                Excute(cmd);
            }
        }

        private string Query(object objCmd)
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

        private void Excute(object objCmd)
        {
            try
            {
                lock (comPort)
                {
                    comPort.WriteLine(objCmd.ToString());
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
