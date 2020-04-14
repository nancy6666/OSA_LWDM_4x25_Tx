using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
    public enum Channel
    {
        CH1,
        CH2,
    }
   
    public class TC720
    {
        private List<byte> dataSend = new List<byte>();
        private byte[] dataRecv = new byte[64];
        protected SerialPort comPort;
        public double TempSpan
        {
            get;
            set;
        }
        public int StablizaitonTime
        {
            get;
            set;
        }

        public int TimeOut
        {
            get;
            set;
        }

        public bool DeInit()
        {
            if (comPort != null)
            {
                comPort.Close();
                comPort.Dispose();
            }
            return true;
        }
        public TC720(string com)
        {
            try
            {
                comPort = new System.IO.Ports.SerialPort();
                if (comPort != null)
                {
                    comPort.PortName = com;
                    comPort.BaudRate = 230400;
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
                        ReadTemperature(Channel.CH1);
                    }
                    else
                        throw new Exception("打开TC720出错！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"TC720 初始化出错，{ex.Message}");
            }
        }

        public bool WriteTemperature(Channel nCh,double temp)
        {
            lock (comPort)
            {
                Int32 value = (Int32)(temp * 100);
                dataSend.Clear();
                string strWriteCmd = nCh == Channel.CH1 ? "*1c" : "*1c";
                string valueH = string.Format("{0:X2}", ((value >> 8) & 0xFF)).ToLower();
                string valueL = string.Format("{0:X2}", ((value) & 0xFF)).ToLower();

                string strCmd = $"{strWriteCmd}{valueH}{valueL}";
                byte[] sum = CheckSum(strCmd.Substring(1, strCmd.Length - 1));

                foreach (var ch in strCmd)
                    dataSend.Add((byte)ch);
                dataSend.Add(sum[1]);
                dataSend.Add(sum[0]);
                dataSend.Add((byte)'\r');
                comPort.Write(dataSend.ToArray(), 0, dataSend.Count);
                Thread.Sleep(50);
                comPort.Read(dataRecv, 0, 64);
                string str = System.Text.Encoding.ASCII.GetString(dataRecv).Replace("*", "").Replace("^", "").Replace("\0", "");
                bool bRet= str.Substring(0, 4) == $"{valueH}{valueL}";
                return bRet;
            }
      
        }

        public double ReadTemperature(Channel nCh)
        {
            lock (comPort)
            {
                dataSend.Clear();
                string strReadCmd = nCh == Channel.CH1 ? "*01" : "*04";
                string strCmd = $"{strReadCmd}0000";
                byte[] sum = CheckSum(strCmd.Substring(1, strCmd.Length - 1));

                foreach (var ch in strCmd)
                    dataSend.Add((byte)ch);
                dataSend.Add(sum[1]);
                dataSend.Add(sum[0]);
                dataSend.Add((byte)'\r');
                comPort.Write(dataSend.ToArray(), 0, dataSend.Count);
                Thread.Sleep(50);
                comPort.Read(dataRecv, 0, 64);
                string str = System.Text.Encoding.ASCII.GetString(dataRecv).Replace("*", "").Replace("^", "").Replace("\0", "");
                Int16 value = Convert.ToInt16(str.Substring(0, 4), 16);
                return Math.Round((double)value /100.0f, 2);
            }
        }

        private byte[] CheckSum(byte[] ByteList)
        {
            int sum = 0;
            foreach (var it in ByteList)
                sum += it;
            string h = string.Format("{0:X}", sum & 0xF).ToLower();
            string l = string.Format("{0:X}", (sum >> 4) & 0xF).ToLower();
            return new Byte[2] { (byte)h[0], (byte)l[0] };

        }

        private byte[] CheckSum(string Str)
        {
            int sum = 0;
            foreach (var it in Str)
                sum += (byte)it;
            string h = string.Format("{0:X}", sum & 0xF).ToLower();
            string l = string.Format("{0:X}", (sum >> 4) & 0xF).ToLower();
            return new Byte[2] { (byte)h[0], (byte)l[0] };
        }
    }
}
