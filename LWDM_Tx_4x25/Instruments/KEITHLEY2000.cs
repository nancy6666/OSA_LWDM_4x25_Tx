using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using NationalInstruments.NI4882;

namespace LWDM_Tx_4x25.Instruments
{
   public class Keithley2000 
    {
        private GPIB K2000;

        public Keithley2000(int GPIBaddr)
        {
            try
            {
                K2000 = new GPIB(GPIBaddr);
            }
            catch(Exception ex)
            {
                throw new Exception($"Open K2000 error!{ex.Message}");
            }
        }
        public enum EnumMeasFunc
        {
            OFFALL,
            ONVOLT,
            ONCURR,
            ONRES
        }
        public enum EnumComplianceLIMIT{DEFAULT, MAX, MIN, REAL}
        

        public enum EnumVoltUnit { V, DB, DBM }
        public enum EnumDACType { DC, AC }
      
        #region private

      
        private byte[] btRecv=new byte[1024];
       
        public double[] MeasureValue=new double[2] { 0.0f,0.0f};
        #endregion
     /// <summary>
     /// 
     /// </summary>
     /// <param name="o"></param>
     /// <returns>单位是A</returns>
        public  double Fetch(object o=null)
        {
            SetContinuous(false);
            Thread.Sleep(50);
            K2000.GPIBwr(":READ?");
            string str = K2000.GPIBrd(200);
            string[] meas_ret = str.Split('\n');
            if (meas_ret.Length !=0)
            {
                Double.TryParse(meas_ret[0], out MeasureValue[0]);   //读出来的电流是A            
            }
            return MeasureValue[0];
        }

        public void SetContinuous(bool bEnable)
        {
            string strCmd = string.Format(":INITiate:CONTinuous {0}", bEnable ? "ON" : "OFF");
            K2000.GPIBwr(strCmd);
        }
        public void SetVoltUnit(EnumDACType voltType, EnumVoltUnit unit)
        {
            string strCmd = "";
            string strUnit = "";
            switch (unit)
            {
                case EnumVoltUnit.V:
                    strUnit = "V";
                    break;
                case EnumVoltUnit.DB:
                    strUnit = "DB";
                    break;
                default:
                    strUnit = "DBM";
                    break;
            }
            strCmd = string.Format(":UNIT:VOLTage:{0} {1}", voltType== EnumDACType.AC? "AC":"DC",  strUnit);
            K2000.GPIBwr(strCmd);
        }
        //public EnumVoltUnit GetVoltUnit(EnumDACType voltType)
        //{
        //    string strCmd = "";
        //    strCmd = string.Format(":UNIT:VOLTage:{0}?", voltType == EnumDACType.AC ? "AC" : "DC");
        //    string strRet = Query(strCmd).ToString().Trim().Replace("\r","").Replace("\n","");
        //    if (strRet.ToUpper() == "V")
        //        return EnumVoltUnit.V;
        //    else if (strRet.ToUpper() == "DB")
        //        return EnumVoltUnit.DB;
        //    else if (strRet.ToUpper() == "DBM")
        //        return EnumVoltUnit.DBM;
        //    else
        //        throw new Exception(string.Format("Invalid volt unit while get {0} unit", voltType == EnumDACType.AC ? "AC" : "DC"));
        //}

        //public bool SetVoltAutoRange(EnumDACType voltType,bool bEnable)
        //{
        //    string strCmd = "";
        //    strCmd = string.Format("VOLTage:{0}:RANGe:AUTO {1}", voltType == EnumDACType.AC ? "AC" : "DC", bEnable ? "ON" : "OFF");
        //    return (bool)Excute(strCmd);
        //}

        public void SetCurrentAutoRange(EnumDACType voltType, bool bEnable)
        {
            string strCmd = "";
            strCmd = string.Format("CURRent:{0}:RANGe:AUTO {1}", voltType == EnumDACType.AC ? "AC" : "DC", bEnable ? "ON" : "OFF");
            K2000.GPIBwr(strCmd);
        }
        public void SetMeasureCurrentMode(EnumDACType voltType)
        {
            string strCmd = "";
            strCmd = string.Format(":CONFigure:CURRent:{0}", voltType == EnumDACType.AC ? "AC" : "DC");
            K2000.GPIBwr(strCmd);
        }
        public void SetMeasureVoltMode(EnumDACType voltType)
        {
            string strCmd = "";
            strCmd = string.Format(":CONFigure:VOLTage:{0}", voltType == EnumDACType.AC ? "AC" : "DC");
            K2000.GPIBwr(strCmd);
        }
    }
}
