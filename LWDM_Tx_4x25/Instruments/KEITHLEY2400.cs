using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LWDM_Tx_4x25.Instruments
{
  public class KEITHLEY2400
    {
        /// <summary>
        /// 分析源
        /// </summary>
        public enum MEAScategory
        {
            OFFALL, ONVOLT, ONCURR
        }
        /// <summary>
        /// 分析Range
        /// </summary>
        public enum MEASRANGE
        {
            REAL, UP, DOWN, MAX, MIN, DEFAULT, AUTO
        }

        /// <summary>
        /// 源
        /// </summary>
        public enum SOURCEMODE {VOLT, CURR, MEM}

        /// <summary>
        /// 源工作类型
        /// </summary>
        public enum SOURCEingMODE {FIX, LIST, SWP}

        /// <summary>
        /// 源RANGE
        /// </summary>
        public enum SOURCERANGE {REAL, UP, DOWN, MAX, MIN, DEFAULT, AUTO}

        /// <summary>
        /// 设置CMPL值
        /// </summary>
        public enum ComplianceLIMIT {DEFAULT, MAX, MIN, REAL}

        /// <summary>
        /// 要读取的值类型
        /// </summary>
        public enum READcategory { VOLT, CURR }

        public enum TeminalPanel { FRONT,REAR}

        /// <summary>
        /// Elements contained in the data string for commands :FETCh/:READ/:MEAS/:TRAC:DATA
        /// </summary>
        [Flags]
        public enum EnumDataStringElements
        {
            VOLT = 0x1,
            CURR = 0x2,
            RES = 0x4,
            TIME = 0x8,
            STAT = 0x10,
            ALL = VOLT | CURR | RES | TIME | STAT
        }

        public struct MeasuredData
        {
            public double Voltage;
            public double Current;
            public double Resistance;
            public double Timestamp;
            public UInt32 Status;
        }
        #region Variables

        public EnumDataStringElements DataStringElements;
        private GPIB GPIBDevice;

        public decimal I_limit;
        public decimal Vcc;

        #endregion

        public KEITHLEY2400(int GPIBaddr)
        {
            try
            {
                GPIBDevice = new GPIB(GPIBaddr);
            }
            catch(Exception ex)
            {
                throw new Exception($"Open K2400 Error!{ex.Message}");
            }
        }

        /// <summary>
        /// Set the SourceMeter to V-Source Mode
        /// </summary>
        public void SetToVoltageSource()
        {
            OUTPUT(false);
            SetMEASCategory(MEAScategory.ONCURR);
            SetSOURCEMODE(SOURCEMODE.VOLT);
            SetSOURCEingMODEofVOLT(SOURCEingMODE.FIX);
            SetMEASRangeofCURR(MEASRANGE.AUTO);

            // only return current measurement value under V-Source
            SetDataElement(EnumDataStringElements.CURR | EnumDataStringElements.STAT);
        }

        /// <summary>
        /// Set the SourceMeter to I-Source Mode
        /// </summary>
        public void SetToCurrentSource()
        {
            OUTPUT(false);
            SetMEASCategory(MEAScategory.ONVOLT);
            SetSOURCEMODE(SOURCEMODE.CURR);
            SetSOURCEingMODEofCURR(SOURCEingMODE.FIX);
            SetMEASRangeofVOLT(MEASRANGE.AUTO);

            // only return voltage measurement value under I-Source
            SetDataElement(EnumDataStringElements.VOLT | EnumDataStringElements.STAT);
        }


        /// <summary>
        /// 获取设备描述
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            GPIBDevice.GPIBwr("*IDN?");
            return  GPIBDevice.GPIBrd(100);
        }

        /// <summary>
        /// 打开关闭输出
        /// </summary>
        /// <param name="on"></param>
        public void OUTPUT(bool on)
        {
            if (on) { GPIBDevice.GPIBwr("OUTP ON"); }
            else { GPIBDevice.GPIBwr("OUTP OFF"); }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 设置前/后面板
        /// </summary>
        /// <param name="panel"></param>
        public void SetTerminalPanel(TeminalPanel panel)
        {
            switch(panel)
            {
                case TeminalPanel.FRONT:
                    GPIBDevice.GPIBwr(":ROUTe:TERMinals FRONt");
                    break;
                case TeminalPanel.REAR:
                    GPIBDevice.GPIBwr(":ROUTe:TERMinals REAR");
                    break;
            }
        }

        /// <summary>
        /// Set the elements valid while executing :Read/etc. commands
        /// </summary>
        /// <param name="Elements"></param>
        public void SetDataElement(EnumDataStringElements Elements)
        {
            List<string> elemlsit = new List<string>();

            if (Elements.HasFlag(EnumDataStringElements.VOLT))
                elemlsit.Add(EnumDataStringElements.VOLT.ToString());

            if (Elements.HasFlag(EnumDataStringElements.CURR))
                elemlsit.Add(EnumDataStringElements.CURR.ToString());

            if (Elements.HasFlag(EnumDataStringElements.RES))
                elemlsit.Add(EnumDataStringElements.RES.ToString());

            if (Elements.HasFlag(EnumDataStringElements.TIME))
                elemlsit.Add(EnumDataStringElements.TIME.ToString());

            if (Elements.HasFlag(EnumDataStringElements.STAT))
                elemlsit.Add(EnumDataStringElements.STAT.ToString());

            if (elemlsit.Count == 0)
                throw new ArgumentException (string.Format("the null elemtents passed"));
            else
            {
                string arg = String.Join(",", elemlsit.ToArray());
                GPIBDevice.GPIBwr(string.Format("FORM:ELEM {0}", arg));

                this.DataStringElements = Elements;
            }
        }
        public MeasuredData GetMeasuredData(EnumDataStringElements Elements)
        {

            MeasuredData result = new MeasuredData();
            result.Voltage = double.NaN;
            result.Current = double.NaN;
            result.Resistance = double.NaN;
            result.Timestamp = double.NaN;
            result.Status = UInt32.MinValue;

            var ret = Fetch();

            if (Elements == EnumDataStringElements.ALL)
            {
                var retArray = ret.Split(',');
                // get voltage
                if (double.TryParse(retArray[0], out double v))
                    result.Voltage = v;

                // get current
                if (double.TryParse(retArray[1], out double c))
                    result.Current = c;

                // get resistance
                if (double.TryParse(retArray[2], out double r))
                    result.Resistance = r;

                // get timestamps
                if (double.TryParse(retArray[3], out double t))
                    result.Timestamp = t;

                // get status
                if (UInt32.TryParse(retArray[4], out UInt32 s))
                    result.Status = s;
            }
            else if (Elements == EnumDataStringElements.VOLT)
            {
                var retArray = ret.Split(',');
                // get Voltage
                if (double.TryParse(retArray[0], out double v))
                    result.Voltage = v;
            }
            else if (Elements == EnumDataStringElements.CURR)
            {
                var retArray = ret.Split(',');
                // get current
                if (double.TryParse(retArray[0], out double c))
                    result.Current = c;
            }
            else if (Elements == EnumDataStringElements.RES)
            {
                // get resistance
                if (double.TryParse(ret, out double r))
                    result.Resistance = r;
            }
            else if (Elements == EnumDataStringElements.TIME)
            {
                // get timestamps
                if (double.TryParse(ret, out double t))
                    result.Timestamp = t;
            }
            else if (Elements == EnumDataStringElements.STAT)
            {
                // get status
                if (UInt32.TryParse(ret, out UInt32 s))
                    result.Status = s;
            }

            return result;

        }

        #region 测量源操作

        /// <summary>
        /// 测量源设置
        /// </summary>
        /// <param name="mc"></param>
        public void SetMEASCategory(MEAScategory mc)
        {
            switch (mc)
            {
                case MEAScategory.OFFALL:
                    GPIBDevice.GPIBwr(":SENS:FUNC:OFF:ALL");
                    break;
                case MEAScategory.ONCURR:
                    GPIBDevice.GPIBwr(":SENS:FUNC:ON \"CURR\"");
                    break;
                case MEAScategory.ONVOLT:
                    GPIBDevice.GPIBwr(":SENS:FUNC:ON \"VOLT\"");
                    break;
            }
        }

        /// <summary>
        /// 设置电流分析源的RANGE
        /// </summary>
        /// <param name="range"></param>
        /// <param name="real"></param>
        public void SetMEASRangeofCURR(MEASRANGE range, decimal real=-1)
        {
            switch (range)
            {
                case MEASRANGE.MAX:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG MAX:");
                    break;
                    
                case MEASRANGE.MIN:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG MIN:");
                    break;

                case MEASRANGE.UP:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG UP:");
                    break;

                case MEASRANGE.DOWN:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG DOWN:");
                    break;

                case MEASRANGE.DEFAULT:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG DEF:");
                    break;

                case MEASRANGE.AUTO:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 1");
                    break;

                case MEASRANGE.REAL:
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:CURR:RANG " + real.ToString());
                    break;
            }
        }

        /// <summary>
        /// 设置电压测量源的RANGE
        /// </summary>
        /// <param name="range"></param>
        /// <param name="real"></param>
        public void SetMEASRangeofVOLT(MEASRANGE range, decimal real=-1)
        {
            switch (range)
            {
                case MEASRANGE.MAX:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG MAX:");
                    break;

                case MEASRANGE.MIN:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG MIN:");
                    break;

                case MEASRANGE.UP:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG UP:");
                    break;

                case MEASRANGE.DOWN:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG DOWN:");
                    break;

                case MEASRANGE.DEFAULT:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG DEF:");
                    break;

                case MEASRANGE.AUTO:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 1");
                    break;

                case MEASRANGE.REAL:
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SENS:VOLT:RANG " + real);
                    break;
            }
        }

        /// <summary>
        /// 设置电流测量源的Compliance
        /// </summary>
        /// <param name="cmpl"></param>
        /// <param name="real"></param>
        public void SetComplianceofCURR(ComplianceLIMIT cmpl, decimal real=-1)
        {
            switch (cmpl)
            {
                case ComplianceLIMIT.DEFAULT:
                    GPIBDevice.GPIBwr(":SENS:CURR:PROT DEF");
                    break;

                case ComplianceLIMIT.MIN:
                    GPIBDevice.GPIBwr(":SENS:CURR:PROT MIN");
                    break;

                case ComplianceLIMIT.MAX:
                    GPIBDevice.GPIBwr(":SENS:CURR:PROT MAX");
                    break;

                case ComplianceLIMIT.REAL:
                    GPIBDevice.GPIBwr(":SENS:CURR:PROT " + real.ToString());
                    break;
            }
        }

        /// <summary>
        /// 设置电压测量源的Compliance
        /// </summary>
        /// <param name="cmpl"></param>
        /// <param name="real"></param>
        public void SetComplianceofVOLT(ComplianceLIMIT cmpl, decimal real=-1)
        {
            switch (cmpl)
            {
                case ComplianceLIMIT.DEFAULT:
                    GPIBDevice.GPIBwr(":SENS:VOLT:PROT DEF");
                    break;

                case ComplianceLIMIT.MIN:
                    GPIBDevice.GPIBwr(":SENS:VOLT:PROT MIN");
                    break;

                case ComplianceLIMIT.MAX:
                    GPIBDevice.GPIBwr(":SENS:VOLT:PROT MAX");
                    break;

                case ComplianceLIMIT.REAL:
                    GPIBDevice.GPIBwr(":SENS:VOLT:PROT " + real.ToString());
                    break;
            }
        }
        #endregion

        #region 源操作
        /// <summary>
        /// 设置源类型
        /// </summary>
        /// <param name="smode"></param>
        public void SetSOURCEMODE(SOURCEMODE smode)
        {
            switch (smode)
            {
                case SOURCEMODE.CURR:
                    GPIBDevice.GPIBwr(":SOUR:FUNC CURR");
                    break;

                case SOURCEMODE.VOLT :
                    GPIBDevice.GPIBwr(":SOUR:FUNC VOLT");
                    break;

                case SOURCEMODE.MEM:
                    GPIBDevice.GPIBwr(":SOUR:FUNC MEM");
                    break;
            }
        }

        /// <summary>
        /// 设置电流源工作类型
        /// </summary>
        /// <param name="singmode"></param>
        public void SetSOURCEingMODEofCURR(SOURCEingMODE singmode)
        {
            switch (singmode)
            {
                case SOURCEingMODE.FIX:
                    GPIBDevice.GPIBwr(":SOUR:CURR:MODE FIX");
                    break;

                case SOURCEingMODE.LIST:
                    GPIBDevice.GPIBwr(":SOUR:CURR:MODE LIST");
                    break;

                case SOURCEingMODE.SWP:
                    GPIBDevice.GPIBwr(":SOUR:CURR:MODE SWP");
                    break;
            }
        }

        /// <summary>
        /// 设置电压源工作类型
        /// </summary>
        /// <param name="singmode"></param>
        public void SetSOURCEingMODEofVOLT(SOURCEingMODE singmode)
        {
            switch (singmode)
            {
                case SOURCEingMODE.FIX:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:MODE FIX");
                    break;

                case SOURCEingMODE.LIST:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:MODE LIST");
                    break;

                case SOURCEingMODE.SWP:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:MODE SWP");
                    break;
            }
        }

        /// <summary>
        /// 设置电流源RANGE
        /// </summary>
        /// <param name="srange"></param>
        /// <param name="real"></param>
        public void SetSOURCERANGEofCURR(SOURCERANGE srange, decimal real)
        {
            switch (srange)
            {
                case SOURCERANGE.AUTO:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 1");
                    break;

                case SOURCERANGE.DEFAULT:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG DEF");
                    break;

                case SOURCERANGE.DOWN:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG DOWN");
                    break;

                case SOURCERANGE.UP:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG UP");
                    break;

                case SOURCERANGE.MIN:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG MIN");
                    break;

                case SOURCERANGE.MAX:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG MAX");
                    break;

                case SOURCERANGE.REAL:
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:CURR:RANG " + real.ToString());
                    break;
            }
        }

        /// <summary>
        /// 设置电压源RANGE
        /// </summary>
        /// <param name="srange"></param>
        /// <param name="real"></param>
        public void SetSOURCERANGEofVOLT(SOURCERANGE srange, decimal real)
        {
            switch (srange)
            {
                case SOURCERANGE.AUTO:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 1");
                    break;

                case SOURCERANGE.DEFAULT:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG DEF");
                    break;

                case SOURCERANGE.DOWN:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG DOWN");
                    break;

                case SOURCERANGE.UP:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG UP");
                    break;

                case SOURCERANGE.MIN:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG MIN");
                    break;

                case SOURCERANGE.MAX:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG MAX");
                    break;

                case SOURCERANGE.REAL:
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG:AUTO 0");
                    GPIBDevice.GPIBwr(":SOUR:VOLT:RANG " + real.ToString());
                    break;
            }
        }

        /// <summary>
        /// 设置电压源电压值
        /// </summary>
        /// <param name="lev"></param>
        public void SetSOURCEVOLTlevel(decimal lev)
        {
            GPIBDevice.GPIBwr(":SOUR:VOLT:LEV " + lev.ToString());
        }

        /// <summary>
        /// 设置电流源电流值
        /// </summary>
        /// <param name="lev"></param>
        public void SetSOURCECURRlevel(decimal lev)
        {
            GPIBDevice.GPIBwr(":SOUR:CURR:LEV " + lev.ToString());
        }
        #endregion

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string Fetch()
        {
            try
            {
                GPIBDevice.GPIBwr(":READ?");
                string str = GPIBDevice.GPIBrd(200);
                return str;
            }
            catch(Exception e)
            {
                throw new Exception("An error occurred when reading VBR.\r\n" + e.Message);
            }
        }
    }
}
