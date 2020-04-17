using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
   //确认设备地址是什么 01
   public class JW8402
    {
        protected SerialPort comPort;

        public  JW8402(string com)
        {
            try
            {
                comPort = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
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
                throw new Exception($"JW8402 初始化出错，{ex.Message}");
            }
        }

        /// <summary>
        /// 设置通道（小于 100 路时）
        /// </summary>
        /// <param name="nChannel">00～99（表示切换到第几通道）1表示通道1</param>
        /// <param name="devAddr">00～99（表示设备地址）</param>
        /// <returns>true:成功</returns>
        public bool SetChannel(int nChannel,int devAddr=01)
        {
            string strCmd = "";
            strCmd = string.Format("<AD{1}_S_{0}>", nChannel.ToString(), devAddr.ToString());
            string rt=Excute(strCmd);
            if(rt.Contains("OK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region private Methods

        private string Excute(object objCmd)
        {
            try
            {
                lock (comPort)
                {
                    comPort.WriteLine(objCmd.ToString());
                    Thread.Sleep(50);
                    string strReturn = comPort.ReadLine();
                    return strReturn;
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
