using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25.Instruments
{
   public class AQ6370
    {
        #region Properties

        private GPIB gb;
        public double PeakWL
        {
            get;
            set;
        }
        public double SMSR
        {
            get;
            set;
        }
        private enum EnumSweepMode
        {
            SINGle = 1,
            REPeat,
            AUTO,
            SEGMen
        }
        #endregion

        public AQ6370(int addr)
        {
            try
            {
                gb = new GPIB(addr);
                //  gb.GPIBwr("*RST");//setting init
                gb.GPIBwr("CFORM1");//set(AQ637X mode)
            }
            catch(Exception ex)
            {
                throw new Exception($"Open AQ6370 error,pls make sure the Remote Interface has been set to GPIB Mode,{ex.Message}");
            }
        }

        private void SetAQ6370(double startWave, double stopWave)
        {
            try
            {
                SetStartWavelength(startWave);
                SetStopWavelength(stopWave);
                gb.GPIBwr(":sens: sens mid");//sens mode=MID
                gb.GPIBwr(":sens:sweep:points:auto on");//Sampling point=Auto
                SetSweepMode(EnumSweepMode.SINGle);//设置扫描模式为single
                gb.GPIBwr("* CLS");
                gb.GPIBwr(":INITIATE");
            }
            catch(Exception ex)
            {
                throw new Exception($"Set AQ6370 error,{ ex.Message}");
            }
        }

        public void StartSweep(double startWave, double stopWave)
        {
            try
            {
                SetAQ6370(startWave, stopWave);
                
                //get sweep status, the last bit of status is 1 when a sweep ends
                byte status=0;
                do
                {
                    gb.GPIBwr(":stat:oper:even?");
                    Byte.TryParse(gb.GPIBrd(100), out status);
                } while ((status & 1) != 1);

                gb.GPIBwr(":CALCulate:CATegory DFBLd");//设置算法为DFBLD
                ExcuteAnalysis();
                ReadAnalysisData();
            }
            catch (Exception ex)
            {
                throw new Exception($"AQ6370D执行扫描出错{ex.Message}");
            }
        }

      
        #region Private Methods

        private void SetStartWavelength(double startWave)
        {
            try
            {
                gb.GPIBwr($":SENSE:WAVELENGTH:START {startWave}NM");
            }
            catch (Exception ex)
            {
                throw new Exception($"设置AQ6370D的起始波长出错{ex.Message}");
            }
        }

        private void SetStopWavelength(double stopWave)
        {
            try
            {
                gb.GPIBwr($":SENSE:WAVELENGTH:STOP {stopWave}NM");
            }
            catch (Exception ex)
            {
                throw new Exception($"设置AQ6370D的终止波长出错{ex.Message}");
            }
        }

        private void SetSweepMode(EnumSweepMode sweepMode)
        {
            try
            {
                gb.GPIBwr($":INITIATE:SMODE {sweepMode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"设置AQ6370D的扫描模式出错{ex.Message}");
            }
        }

        private void SetSpan(float span)
        {
            try
            {
                gb.GPIBwr($":SENSe:WAVelength:SPAN {span}nm");
            }
            catch (Exception ex)
            {
                throw new Exception($"AQ6370D设置span出错{ex.Message}");
            }
        }

        private void SetCWL(float cwl)
        {
            try
            {
                gb.GPIBwr($":SENSe:WAVelength:CENTer {cwl}nm");
            }
            catch (Exception ex)
            {
                throw new Exception($"AQ6370D设置CWL出错{ex.Message}");
            }
        }
        /// <summary>
        /// 执行一次参数计算
        /// </summary>
        private void ExcuteAnalysis()
        {
            try
            {
                gb.GPIBwr("CALCulate");
            }
            catch (Exception ex)
            {
                throw new Exception($"AQ6370D执行一次参数计算时出错{ex.Message}");
            }
        }

        private void ReadAnalysisData()
        {
            try
            {
               // gb.GPIBwr(":CALCulate:DATA:DFBLd?");
                gb.GPIBwr(":CALCulate:DATA?");
                /*
                 <peak wl>,<peak lvl>,<center wl>,<spec
                 wd>,<smsr(L)>,<smsr(R)>,<modeofst(L)>,<mode ofst(R)>,<snr>,<power>,<rms>,<Krms>
               */
                string str = gb.GPIBrd(200);
                string[] data = str.Split(',');

                //??????数据索引 PeakWL的单位转换
                this.SMSR = Convert.ToDouble(data[11]);//smsr(L)
                this.PeakWL = Convert.ToDouble(data[4]); //unit is m/Hz,need to be nm
            }
            catch (Exception ex)
            {
                throw new Exception($"读取AQ6370D数据出错{ex.Message}");
            }
        }

        /// <summary>
        /// Sets/queries the automatic analysis function,When the automatic analysis function is ON,automatically activates an analysis function that is active after a sweep has ended
        /// </summary>
        private void SetAutoAnalysis()
        {
            try
            {
                gb.GPIBwr(":CALCULATE:AUTO ON");
            }
            catch (Exception ex)
            {
                throw new Exception($"AQ6370D设置为Auto Analysis出错{ex.Message}");
            }
        }

        #endregion
    }
}
