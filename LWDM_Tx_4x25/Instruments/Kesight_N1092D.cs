using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using System.Windows.Forms;
using System.Threading;

namespace LWDM_Tx_4x25.Instruments
{
    public class Kesight_N1092D
    {
        #region Properties
        public string Channel
        {
            get;
            set;
        }
        private string filter_rate;
        public string Filter_rate
        {
            get
            {
                return filter_rate;
            }
            set
            {
                this.filter_rate = value + "E+9";
            }

        }
        private string channel_bandWidth;
        public string Channel_bandWidth
        {
            get
            {
                return channel_bandWidth;
            }
            set
            {
                this.channel_bandWidth = value + "E+9";
            }
        }
        public double AOP_Offset
        {
            get;
            set;
        }
        public double Crossing
        {
            get;set;
        }
        public double Jitter_pp
        {
            get;
            set;
        }
        public double JitterRMS
        {
            get;
            set;
        }
        public double FallTime
        {
            get;
            set;
        }

        public double RiseTime
        {
            get;
            set;
        }

        public double MaskMargin
        {
            get;
            set;
        }
        public double ER
        {
            get;
            set;
        }

        private FormattedIO488 myN1010A;
        private string addr;
        private string complete;
        private string[] inst_err;
        private int inst_code;
        #endregion

        public void Init()
        {
            try
            {
                ResourceManager mgr = new ResourceManager();
                // create the formatted io object
                myN1010A = new FormattedIO488();

                //get VISA address from textbox1
                addr = "TCPIP0::localhost::hislip0,4880::INSTR";

                // open IO driver session
                myN1010A.IO = (IMessage)mgr.Open(addr.ToString());

                //set timeout
                myN1010A.IO.Timeout = 20000;

                myN1010A.IO.TerminationCharacter = 10;
                myN1010A.IO.TerminationCharacterEnabled = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"N1092D初始化失败，{ex.Message}");
            }
        }
        /// <summary>
        /// set N1092
        /// </summary>
        public void SetN1092()
        {
            try
            {
                myN1010A.WriteString(":SYSTem:MODE EYE", true);//choose eye/mask mode

                myN1010A.WriteString($":CHANnel{this.Channel}:DISPlay ON", true);//enable channel
               // myN1010A.WriteString($":TRIGger:PLOCk  ON", true);//pattern lock on

                myN1010A.WriteString(":ACQuire:CDISplay", true);//clear the dispaly
                ////set acquisition limit
                myN1010A.WriteString($":LTESt:ACQuire:CTYPe:WAVeforms", true);//  Create test based on acquisition to waveforms.
                myN1010A.WriteString($":LTESt:ACQuire:CTYPe:WAVeforms 1000", true);//set number of waveforms
               ///????need to confirm patterns or waveforms
                // myN1010A.WriteString($":LTESt:ACQuire:CTYPe:PATTerns 1000", true);//set number of waveforms

                myN1010A.WriteString($":LTESt:ACQuire:STATe ON", true);//enable limits

                //myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FRATe {this.Filter_rate}", true);
                //myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FRATe?", true);
                //var a = myN1010A.ReadString();
                myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FBANdwidth {this.Channel_bandWidth}", true);
                myN1010A.WriteString($":CHAN{this.Channel}:ATTenuator:DECibels {this.AOP_Offset}", true);//设置AOP offset

                //Set mask margin
                myN1010A.WriteString(":MTESt1:DISPlay OFF");
                Thread.Sleep(100);
                myN1010A.WriteString($":MTESt1:LOAD:FNAMe \"% DEMO_DIR %\\Masks\\Ethernet\025.78125 - 100GBASE-LR4_Tx_Optical_D31.mskx\""); //set mask 100G LR
                myN1010A.WriteString(":MTESt1:LOAD", true);
                myN1010A.WriteString(":MTESt:MARGin:STATe ON", true);
                myN1010A.WriteString(":MTESt:MARGin:METHod AUTO", true);
                myN1010A.WriteString(":MTESt:MARGin:AUTO:METHod HRATio", true);
                myN1010A.WriteString(":MTESt:MARGin:AUTO:HRATio 5e-5", true);
            }
            catch (Exception ex)
            {
                throw new Exception($"根据Test plan 对N1092进行设置时出错，{ex.Message}");
            }
        }
        /// <summary>
        ///Get measurement results
        /// </summary>
        public void QueryMeasurementResults()
        {
            string errorMessages = "";

            try
            {
                //// query measurement results
                myN1010A.WriteString($":MEASure:EYE:JITTer:SOURce CHANnel{ this.Channel}", true);
                myN1010A.WriteString(":MEASure:EYE:JITTer:FORMat RMS", true);
                myN1010A.WriteString(":MEASure:EYE:JITTer", true);
                myN1010A.WriteString(":MEASure:EYE:JITTer?", true);
                this.JitterRMS = Convert.ToDouble(myN1010A.ReadString())*Math.Pow(10,12);
                myN1010A.WriteString(":MEASure:EYE:JITTer:FORMat PP", true);
                myN1010A.WriteString(":MEASure:EYE:JITTer", true);
                myN1010A.WriteString(":MEASure:EYE:JITTer?", true);
                this.Jitter_pp = Convert.ToDouble(myN1010A.ReadString()) * Math.Pow(10, 12);

                myN1010A.WriteString(":MEASure:EYE:ERATio", true);
                myN1010A.WriteString(":MEASure:EYE:ERATio?", true);
                this.ER = Convert.ToDouble(myN1010A.ReadString());

                myN1010A.WriteString(":MEASure:EYE:CROSsing", true);
                myN1010A.WriteString(":MEASure:EYE:CROSsing?", true);
                this.Crossing = Convert.ToDouble(myN1010A.ReadString());

                myN1010A.WriteString(":MEASure:EYE:RISetime", true);
                myN1010A.WriteString(":MEASure:EYE:RISetime?", true);
                this.RiseTime = Convert.ToDouble(myN1010A.ReadString()) * Math.Pow(10, 12);
                myN1010A.WriteString(":MEASure:EYE:FALLtime", true);
                myN1010A.WriteString(":MEASure:EYE:FALLtime?", true);
                this.FallTime = Convert.ToDouble(myN1010A.ReadString()) * Math.Pow(10, 12);

                // Mask Margin

                myN1010A.WriteString(":MEASure:MTESt1:MARgin?", true);
                double ret = 0;
                double.TryParse(myN1010A.ReadString(), out ret);
                this.MaskMargin = ret;

                //AOP 只需在眼图上显示
                myN1010A.WriteString(":MEASure:EYE:APOWer:UNITs DBM", true);
                myN1010A.WriteString(":MEASure:EYE:APOWer", true);

                //query err information
                inst_code = -1;

                myN1010A.WriteString(":SYSTem:ERRor?", true);
                inst_err = myN1010A.ReadString().Split(',');
                inst_code = Convert.ToInt32(inst_err[0].ToString());
                //if (inst_code != 0)
                //{
                //    errorMessages = errorMessages.ToString() + inst_err[0].ToString() + ": " + inst_err[1].ToString() + "\r\n";
                //    throw new Exception($"从眼图仪获取测试数据出错,{errorMessages}");
                //}
            }
            catch (Exception ex)
            {
                throw new Exception($"从眼图仪获取测试数据出错！{ex.Message}");
            }
        }
       
        public void AutoScale()
        {
            try
            {
                SetN1092();
                myN1010A.WriteString(":SYSTem:AUToscale", true);
                Thread.Sleep(5000);
                myN1010A.WriteString("*OPC?");
                complete = myN1010A.ReadString();
            }
            catch(Exception ex)
            {
                throw new Exception($"AutoScale的时候出错，{ ex.Message}");
            }
        }
        public void Run()
        {
            try
            {
                myN1010A.WriteString("ACQuire:SINGle", true);// Stop waveform acquisition.

                SetN1092();

                myN1010A.WriteString(":ACQuire:RUN", true);//when a limit test is turned on
                myN1010A.WriteString(":SYSTem:AUToscale", true);
                Thread.Sleep(10000);
                myN1010A.WriteString("*OPC?", true);//wait for test completion for 1000 waveforms
                complete = myN1010A.ReadString();
                myN1010A.WriteString(":LTESt:ACQuire:STATe OFF");//Turn limit test off.
            }
            catch (Exception ex)
            {
                throw new Exception($"Run的时候出错，{ ex.Message}");
            }
        }

        public void SaveImage(string ImageName)
        {
            try
            {
          //    myN1010A.WriteString($":DISK:SIMage:FNAMe \"C:\\DDisk\\RF eye\\913000001\\1\"", true);
                myN1010A.WriteString($":DISK:SIMage:FNAMe \"{ImageName}\"", true);
                myN1010A.WriteString(":DISK:SIMage:SAVE", true);
                myN1010A.WriteString("*OPC?", true);
                myN1010A.ReadString();
                myN1010A.WriteString(":DISK:SIMage:FNAMe:AUPDate", true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// close instrument session
        /// </summary>
        public void Close()
        {
            myN1010A.IO.Close();
        }
    }
}
