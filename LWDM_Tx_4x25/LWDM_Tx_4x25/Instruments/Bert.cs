using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFTest_Tx_PAM4_50G.Instruments
{
  public  class Bert
    {
        protected SerialPort comPort;
        public bool Init(string com)
        {
            try
            {
                bool ret = false;
                comPort = new System.IO.Ports.SerialPort();
                if (comPort != null)
                {
                    comPort.PortName = com;
                    comPort.BaudRate = 115200;
                    comPort.Parity = Parity.None;
                    comPort.StopBits = StopBits.One;
                    comPort.DataBits = 8;
                    comPort.ReadTimeout = 1000;
                    comPort.WriteTimeout = 1000;
                    if (comPort.IsOpen)
                        comPort.Close();
                    comPort.Open();
                    if (comPort.IsOpen)
                    {
                      //  ReadTemperature(Channel.CH1);
                        ret = true;
                    }
                    else
                        ret = false;
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception($"Bert 初始化出错，{ex.Message}");
            }
        }
    }
}
