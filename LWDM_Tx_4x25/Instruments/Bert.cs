using Stelight.TestLibrary;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
    public class Bert
    {
        public Inst_PAM4_Bert Inst_PAM4_Bert;
        public double Ppg_data_rate
        {
            get;
            set;
        }
        public int Ppg_channel
        {
            get;
            set;
        }
        public string Ppg_PRBS_pattern
        {
            get;
            set;
        }

        public double Clock
        {
            get;
            set;
        }

        public Bert(string com)
        {
            try
            {
                Inst_PAM4_Bert = new Inst_PAM4_Bert(com);
                Inst_PAM4_Bert.remoteControl();
            }
            catch (Exception ex)
            {
                throw new Exception($"Open Bert error!{ex.Message}");
            }
        }
        //Bert的初始设置
        public void SetBert()
        {
            try
            {
                for (int BertChannel = 0; BertChannel < 4; BertChannel++)
                {
                    Inst_PAM4_Bert.setTimebase(this.Ppg_data_rate);// set rate

                    Inst_PAM4_Bert.ppgOutputControl(BertChannel, true);//Enable PPG CH1
                    Inst_PAM4_Bert.setPPGSignalType(BertChannel, "NRZ");//设置 PPG 的信号为 PMA4 或者 NRZ，字符串 msg 为 NRZ 或者 PAM
                    Thread.Sleep(200);
                    Inst_PAM4_Bert.setEDSignalType(BertChannel, "NRZ");
                    Thread.Sleep(200);
                    Inst_PAM4_Bert.setPPGpatternType(BertChannel, "PBRS"); //设置patternType 为[PRBS| SSPRQ |JP03A|JP03B|SQWV|LINEAR|FIXED|CJT

                    Inst_PAM4_Bert.setPPGPRBSpattern(BertChannel, this.Ppg_PRBS_pattern);
                    //Inst_PAM4_Bert.setPpgprbsMode(BertChannel, "COMBINE");//设置信号发生器PRBS 为combine或者MSB LSB模式
                    Inst_PAM4_Bert.setTriggerFreq((trigger)this.Clock);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"对Bert进行初始设置时出错，{ex.Message}");
            }
        }
    }       
}
