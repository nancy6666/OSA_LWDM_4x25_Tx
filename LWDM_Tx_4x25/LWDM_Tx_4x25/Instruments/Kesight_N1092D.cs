using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using System.Windows.Forms;
using System.Threading;

namespace RFTest_Tx_PAM4_50G.Instruments
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
        public double DCA_Offset
        {
            get;
            set;
        }
        private double ooma;
        public double OOMA
        {
            get
            {
                return ooma;
            }
            set
            {
                this.ooma = value + this.DCA_Offset;
            }
        }
        public double TDEQ
        {
            get;
            set;
        }
        public double OuterER
        {
            get;
            set;
        }
        public double Linearity
        {
            get;
            set;
        }
     
        private double lanch_power;
        public double Lanch_power
        {
            get
            {
                return lanch_power;
            }
            set
            {
                this.lanch_power = value + this.DCA_Offset;
            }
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

                //set termination character to CHR(10) (i.e. "\n")
                //enable terminate reads on termination character
                myN1010A.IO.TerminationCharacter = 10;
                myN1010A.IO.TerminationCharacterEnabled = true;
            }
            catch(Exception ex)
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
                myN1010A.WriteString($":TRIGger:PLOCk  ON", true);//pattern lock on
           
                myN1010A.WriteString($":LTESt:ACQuire:STATe ON", true);//enable limits
                myN1010A.WriteString($":LTESt:ACQuire:CTYPe:WAVeforms 1", true);//set number of waveforms

                myN1010A.WriteString($":CHANnel{this.Channel}:SIRC ON", true);//enable SIRC

                //myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FRATe {this.Filter_rate}", true);
                //myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FRATe?", true);
                //var a = myN1010A.ReadString();
                myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FBANdwidth {this.Channel_bandWidth}", true);
                myN1010A.WriteString($":CHANnel{this.Channel}:SIRC:FBANdwidth?", true);
                var a = myN1010A.ReadString();
                myN1010A.WriteString($":FUNCtion1:FOPerator TEQualizer", true);//设置TEDCQ

                myN1010A.WriteString(":SPRocess1:TEQualizer:PRESets \"IEEE 802.3bs Draft 3.2\"", true);//设置TEDCQ

                myN1010A.WriteString($":SPRocess:DFEQualizer:PRESets:SELections? ", true);//设置TEDCQ
               var t=myN1010A.ReadString();
                myN1010A.WriteString($":FUNCtion1:DISPlay ON", true);//
                myN1010A.WriteString($":DISPlay:WINDow:TIME1:DMODe STILed", true);//眼图和TEDCQ分开显示

            }
            catch (Exception ex)
            {
                throw new Exception($"根据Test plan 对N1092进行设置时出错，{ex.Message}");
            }
        }
        /// <summary>
        ///Get OER,AOP,OOMA,Linearity
        /// </summary>
        public void QueryMeasurementResults()
        {
            try
            {              
                // query measurement results
                myN1010A.WriteString($":MEASure:EYE:OOMA:SOURce CHANnel{this.Channel}", true);
                myN1010A.WriteString(":MEASure:EYE:OOMA:UNITs DBM", true);
                myN1010A.WriteString(":MEASure:EYE:OOMA?", true);
                this.OOMA = Convert.ToDouble(myN1010A.ReadString());

                myN1010A.WriteString(":MEASure:EYE:OER:UNITs DECibel", true);
                myN1010A.WriteString(":MEASure:EYE:OER?", true);
                this.OuterER = Convert.ToDouble(myN1010A.ReadString());

                myN1010A.WriteString(":MEASure:EYE:PAM:LINearity?", true);
                this.Linearity = Convert.ToDouble(myN1010A.ReadString());

                myN1010A.WriteString(":MEASure:EYE:APOWer:UNITs DBM", true);
                myN1010A.WriteString(":MEASure:EYE:APOWer?", true);
                this.Lanch_power = Convert.ToDouble(myN1010A.ReadString());


                //query err information
                inst_code = -1;
                //string messages = "";
                //do
                //{
                //    myN1010A.WriteString(":SYSTem:ERRor?", true);
                //    inst_err = myN1010A.ReadString().Split(',');
                //    inst_code = Convert.ToInt32(inst_err[0].ToString());
                //    messages = messages.ToString() + inst_err[0].ToString() + ": " + inst_err[1].ToString() + "\r\n";

                //} while (inst_code != 0);

                Application.DoEvents();

            }
            catch (Exception ex)
            {
                throw new Exception($"获取测试数据出错！{ex.Message}");
            }
        }
        public void GetTEDCQ()
        {
            myN1010A.WriteString(":MEASure:EYE:TDEQ:SOURce1 FUNCtion1", true);
            myN1010A.WriteString(":MEASure:EYE:TDEQ?", true);
            this.TDEQ = Convert.ToDouble(myN1010A.ReadString());
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
                //QueryMeasurementResults();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void Run()
        {
            try
            {
                SetN1092();
                myN1010A.WriteString(":ACQuire:RUN", true);//when a limit test is turned on
                Thread.Sleep(10000);
                myN1010A.WriteString("*OPC?", true);//wait for test completion
                complete = myN1010A.ReadString();
                //QueryMeasurementResults();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveImage(string ImageName)
        {
            try
            {
          //      myN1010A.WriteString($":DISK:SIMage:FNAMe \"C:\\DDisk\\RF eye\\913000001\\1\"", true);
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
