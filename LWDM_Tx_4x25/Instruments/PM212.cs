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

        private int[] WaveArray =new int[]
        {
            850,1300,1310,1490,1550,1625
        };
       public enum EnumWave
        {
            w850=850,
            w1300=1300,
            w1310=1310,
            w1490=1490,
            w1550=1550,
            w1625=1625
        }
        public List<double> lstPower_Offset = new List<double>();
        public PM212(string com)
        {
            try
            {
                comPort = new SerialPort(com, 19200, Parity.None, 8, StopBits.One);
                comPort.ReadTimeout = 3000;
                comPort.WriteTimeout = 3000;
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
           
        }

        /// <summary>
        /// 读功率
        /// </summary>
        /// <returns>功率值，单位dBm</returns>
        public double ReadPower()
        {
            var res = Query("v");
            if(res==""||res.Contains("L")||res.Contains("NaN"))
            {
                return double.NaN;
            }
            else
            {
                return double.Parse(res);
            }
        }

        /// <summary>
        /// 设置功率计波长
        /// </summary>
        /// <param name="waveMeter">单位为nm</param>
        public bool SetWavelength(EnumWave waveMeter)
        {
            if(!WaveArray.Contains((int)waveMeter))
            {
                return false;
            }
            var res = Query("l");//note:这个是L
            //需要设置的wave的index和当前wave的index的差值
            var differenceIndex = Array.IndexOf(WaveArray, (int)waveMeter) - Array.IndexOf(WaveArray, int.Parse(res));
            //波长换挡次数
            var setTimes = differenceIndex < 0 ? WaveArray.Length + differenceIndex:differenceIndex;
            for(int i=0;i<setTimes;i++)
            {
                Excute("1");//这是数字1
                Thread.Sleep(400);
            }
            res = Query("l");
            var returnRes = (Enum.Parse(typeof(EnumWave), res)).Equals(waveMeter);
            return returnRes;
        }

        private void Excute(object objCmd)
        {
            try
            {
                lock (comPort)
                {
                    comPort.WriteLine(objCmd.ToString());
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Write PM212 Error,{ex.Message}");
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
                throw new Exception($"Read PM212 Error,{ex.Message}");
            }
        }

        private double ConvertWdBm(double pw)
        {
            return 10 * Math.Log10(pw * 1000);
        }
    }
}
