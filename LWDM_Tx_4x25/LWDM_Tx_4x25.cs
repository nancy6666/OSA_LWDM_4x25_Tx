﻿using LWDM_Tx_4x25.Instruments;
using Stelight.TestLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using GY7501_I2C_Control;
using System.Collections;

namespace LWDM_Tx_4x25
{
    public partial class LWDM_Tx_4x25 : Form
    {
        #region Properties

        public delegate void MyCheckedBoxDelegate(bool disable);
        public CancellationTokenSource ctsTotal;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public CDatabase db = new CDatabase();
        public ConfigManagement cfg = new ConfigManagement();
        public CLog log = new CLog();
        public Bert Inst_Bert;
        public int Bert_Channel = -1;

        public bool IsWaveSubOk = false;

        public Keithley7001 K7001;
        public KEITHLEY2400 K2400_1;
        public KEITHLEY2400 K2400_2;
        public KEITHLEY2400 K2400_3;

        public Keithley2000 K2000;
        public LDT5525B L5525B;
        public PM212 pm212;
        public TC720 TC720;
        public JW8402 jw8402;
        public AQ6370 aQ6370;
        public List<double> lstAQ6370_StartWave;
        public List<double> lstAQ6370_StopWave;

        public List<double> lstITU = new List<double>();
        public Kesight_N1092D kesight_N1902D;
        delegate void ThreedShowMsgDelegate(string Message, bool bPass);
        delegate void ThreedShowTempDelegate(double Temp);
        private string strMsg;
        public string PN;
        public CTestSpec TestSpec;
        public CTestData_Channel TestData_Channel;
        private List<double> lstTecTemp;
        private List<double> lstTempProductInPlan;
        private List<string> lstTecTempNote;

        private double Temp_Environment;
        //   private CTestDataCommon TestDataCommon;
        private List<CTestDataCommon> lstTestDataCommon;
        public List<double> lstVcpa;
        public List<double> lstVeq;
        public List<double> lstVmod;
        public List<double> lstIsink;
        public List<double> lstLdd;

        public List<string> lstK7001Pin;
        public const int MaxChannel = 4;

        public double[] Pre_Cursor_Array = new double[MaxChannel];
        public double[] Main_Cursor_Array = new double[MaxChannel];
        public double[] Post_Cursor_Array = new double[MaxChannel];

        delegate void ThreedShowResultDelegate(string[] Message, int[] iPass);
        private CancellationTokenSource cts = null;
        private Task MonitorTask = null;
        private Task MonitorProductTempTask = null;
        /// <summary>
        /// 写入设备的温度值，与实际值之间有offset
        /// </summary>
        private double ProductTempSetToDevice;
        private double _productTemp;
        /// <summary>
        /// 实际客户想要达到的温度设定值
        /// </summary>
        private double ProductTempInPlan
        {
            get
            {
                return _productTemp;
            }
            set
            {
                _productTemp = value;
                this.ProductTempSetToDevice = value + L5525B.TempOffset;
            }
        }

        public string TestRate = "";

        /// <summary>
        /// 子窗体对象，全局使用
        /// </summary>
        private GY7501_I2C gY7501_I2C;

        private GY7501_DataManagement GY7501_Data;

        private int[] iFail = new Int32[5];
        private string[] testResult = new string[6];
        private double _realTimeTemperature = 0;
        public double RealTimeTemperature
        {
            get { return _realTimeTemperature; }
            set
            {
                if (_realTimeTemperature != value)
                {
                    _realTimeTemperature = value;
                }
            }
        }
        public bool TemperatureIsTimeOut
        {
            get;
            set;
        }
        private bool _temperatureIsOk = false;
        public bool TemperatureIsOk
        {
            get { return _temperatureIsOk; }
            set
            {
                if (_temperatureIsOk != value)
                {
                    _temperatureIsOk = value;
                    if (value)
                    {
                        TecTimer.Stop();
                        if (TickCountTotal <= TC720.TimeOut)
                        {
                            ShowMsg($"环境温度已经稳定为{this.Temp_Environment}左右...", true);
                        }
                        TickCountTotal = 0;
                    }
                }
            }
        }
        private int nTickCount = 0;
        public int TickCount
        {
            get { return nTickCount; }
            set
            {
                if (nTickCount != value)
                {
                    nTickCount = value;
                }
            }
        }
        public int TickCountTotal = 0;

        //Product temp
        private double _realTimeTemperature_Product = 0;
        public double RealTimeTemperature_Product
        {
            get { return _realTimeTemperature_Product; }
            set
            {
                if (_realTimeTemperature_Product != value)
                {
                    _realTimeTemperature_Product = value;
                }
            }
        }
        public bool TemperatureIsTimeOut_Product
        {
            get;
            set;
        }
        private bool _temperatureIsOk_Product = false;
        public bool TemperatureIsOk_Product
        {
            get { return _temperatureIsOk_Product; }
            set
            {
                if (_temperatureIsOk_Product != value)
                {
                    _temperatureIsOk_Product = value;
                    if (value)
                    {
                        ProductTempTimer1.Stop();
                        if (TickCountTotal_Product <= L5525B.TimeOut)
                        {
                            ShowMsg($"产品温度已经稳定为{this.ProductTempInPlan}左右...", true);
                        }
                        TickCountTotal_Product = 0;
                    }
                }
            }
        }
        private int nTickCount_Product = 0;
        public int TickCount_Product
        {
            get { return nTickCount_Product; }
            set
            {
                if (nTickCount_Product != value)
                {
                    nTickCount_Product = value;
                }
            }
        }
        public int TickCountTotal_Product = 0;
        #endregion

        #region Consturctor

        public LWDM_Tx_4x25()
        {
            InitializeComponent();
        }
        #endregion

        private void LWDM_Tx_Load(object sender, EventArgs e)
        {
            ShowMsg("打开所有设备...", true);
            try
            {
                Inst_Bert = new Bert(cfg.BertCom);
                K2400_1 = new KEITHLEY2400(Convert.ToInt16(cfg.K2400_1_GPIB));
                K2400_2 = new KEITHLEY2400(Convert.ToInt16(cfg.K2400_2_GPIB));
                K2400_3 = new KEITHLEY2400(Convert.ToInt16(cfg.K2400_3_GPIB));
                K2000 = new Keithley2000(Convert.ToInt16(cfg.K2000_GPIB));
                K7001 = new Keithley7001(Convert.ToInt16(cfg.K7001_GPIB));
                kesight_N1902D = new Kesight_N1092D();
                TC720 = new TC720(cfg.T720Com);
                L5525B = new LDT5525B(cfg.LDT5525BCom);
                aQ6370 = new AQ6370(Convert.ToInt16(cfg.AQ6370_GPIB));
                pm212 = new PM212(cfg.PM212Com);
                jw8402 = new JW8402(cfg.JW8402Com);
            }
            catch (Exception ex)
            {
                ShowMsg($"{ex.Message}", false);
                return;
            }
            try
            {
                this.cbxPN.SelectedIndexChanged -= new System.EventHandler(this.LoadTestSpec);
                this.cbxPN.DataSource = db.GetAllPN();
                this.cbxPN.SelectedIndex = -1;
                this.cbxPN.SelectedIndexChanged += new System.EventHandler(this.LoadTestSpec);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                return;
            }
            ReadRealTECTemp_Out();
            ReadRealTECTemp_Product();
            ShowMsg("请填写基础信息，并选择PN！", true);
        }

        #region Interface Display Methods

        public void ShowMsg(string msg, bool bPass)
        {
            this.Invoke(new ThreedShowMsgDelegate(MsgEvent), new object[] { msg, bPass });
            //this.BeginInvoke(new Action<string, bool>((x, y) => { MsgEvent(x, y); }), new object[] { msg, bPass });
        }
        public void ShowResult(string[] Msg, int[] iPass)
        {
            this.Invoke(new ThreedShowResultDelegate(SetParameterDetailRaw), new object[] { Msg, iPass });//, new object[] { iPass });
        }
        public void ShowRealTemp_Case(double Temp)
        {
            this.BeginInvoke(new ThreedShowTempDelegate(ShowTemp), new object[] { Temp });
            //  this.BeginInvoke(new Action<double>((n) => { ShowTemp(n); }),new object[] { Temp});
        }

        public void ShowRealTemp_Product(double Temp)
        {
            this.BeginInvoke(new ThreedShowTempDelegate(ShowTemp_Product), new object[] { Temp });
        }

        private void MsgEvent(string msg, bool bPass)
        {
            log.WriteLog(msg);
            if (lstViewLog.Items.Count >= 50)
            {
                lstViewLog.Items.Clear();
            }
            lstViewLog.BeginUpdate();
            try
            {
                ListViewItem lvi = new ListViewItem(msg);
                lstViewLog.Items.Add(lvi);
                lstViewLog.Items[lstViewLog.Items.Count - 1].EnsureVisible();//滚动到最后

                if (!bPass)
                    lvi.ForeColor = Color.Red;
                else
                    lvi.ForeColor = Color.Green;
            }
            finally
            {
                lstViewLog.EndUpdate();
            }
        }

        private void ShowTemp(double Temp)
        {
            this.lblRealTemp_Case.Text = RealTimeTemperature.ToString();
        }
        private void ShowTemp_Product(double temp)
        {
            this.lblRealTemp_Product.Text = RealTimeTemperature_Product.ToString();
        }
        #endregion

        #region Big Methods

        /// <summary>
        /// Set devices according to test plan file
        /// </summary>
        /// <param name="pn"> product name,also is sheet name in excel</param>
        private void ReadTestPlan(string pn)
        {
            lstTecTempNote = new List<string>();
            strMsg = "正在获取test plan 的内容,请稍等...";
            ShowMsg(strMsg, true);

            //var task1 = new Task(() =>
            //{
            string TestPlanFile = $"{ Directory.GetCurrentDirectory()}\\config\\LWDM 4x25 TEST PLAN.xlsx";
            Excel.Application excelApp = new Excel.Application
            {
                Visible = false
            };
            Excel.Workbook workbook = excelApp.Workbooks.Open(TestPlanFile);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(pn);
            Excel.Range excelCell = worksheet.UsedRange;
            try
            {
                //Bert paras

                Inst_Bert.Ppg_data_rate = excelCell[4, 2].value;
                Inst_Bert.Ppg_PRBS_pattern = Convert.ToString(excelCell[5, 2].value);
                Inst_Bert.Clock = excelCell[6, 2].value;

                K2400_1.Vcc = Convert.ToDecimal(excelCell[9, 2].value);
                K2400_1.I_limit = Convert.ToDecimal(excelCell[10, 2].value);

                K2400_2.Vcc = Convert.ToDecimal(excelCell[13, 2].value);
                K2400_2.I_limit = Convert.ToDecimal(excelCell[14, 2].value);

                K2400_3.Vcc = Convert.ToDecimal(excelCell[17, 2].value);
                K2400_3.I_limit = Convert.ToDecimal(excelCell[18, 2].value);

                lstK7001Pin = new List<string>();
                lstK7001Pin.Add(excelCell[21, 2].value);
                lstK7001Pin.Add(excelCell[22, 2].value);
                lstK7001Pin.Add(excelCell[23, 2].value);
                lstK7001Pin.Add(excelCell[24, 2].value);

                //N1092 paras
                kesight_N1902D.Channel = Convert.ToString(excelCell[27, 2].value);
                kesight_N1902D.Rate = Convert.ToString(excelCell[28, 2].value);
                kesight_N1902D.AOP_Offset = Convert.ToDouble(excelCell[29, 2].value);

                //T720
                lstTecTemp = new List<double>();
                for (int index = 32; index < 35; index++)
                {
                    string blTempChoose = Convert.ToString(excelCell[index, 4].value);
                    if (blTempChoose.ToUpper().Contains("TRUE"))
                    {
                        lstTecTemp.Add(excelCell[index, 2].value);
                        lstTecTempNote.Add(Convert.ToString(excelCell[index, 3].value));
                    }
                }
                TC720.TempSpan = excelCell[35, 2].value;
                TC720.StablizaitonTime = Convert.ToInt32(excelCell[36, 2].value);
                TC720.TimeOut = Convert.ToInt32(excelCell[37, 2].value);

                //LDT5525B
                L5525B.StablizationTime = Convert.ToInt32(excelCell[40, 2].value);
                L5525B.TempSpan = Convert.ToDouble(excelCell[41, 2].value);
                L5525B.TimeOut = Convert.ToInt32(excelCell[42, 2].value);
                L5525B.TempOffset = Convert.ToDouble(excelCell[43, 2].value);

                //AQ6730
                lstAQ6370_StartWave = new List<double>();
                lstAQ6370_StopWave = new List<double>();
                this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[46, 2].value));
                this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[48, 2].value));
                this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[50, 2].value));
                this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[52, 2].value));
                this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[47, 2].value));
                this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[49, 2].value));
                this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[51, 2].value));
                this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[53, 2].value));

                //PM212
                pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[56, 2].value));
                pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[57, 2].value));
                pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[58, 2].value));
                pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[59, 2].value));

                //Product Temp
                this.lstTempProductInPlan = new List<double>();
                this.lstTempProductInPlan.Add(Convert.ToDouble(excelCell[62, 2].value));
                this.lstTempProductInPlan.Add(Convert.ToDouble(excelCell[63, 2].value));
                this.lstTempProductInPlan.Add(Convert.ToDouble(excelCell[64, 2].value));
                //ITU
                this.lstITU.Add(Convert.ToDouble(excelCell[67, 2].value));
                this.lstITU.Add(Convert.ToDouble(excelCell[68, 2].value));
                this.lstITU.Add(Convert.ToDouble(excelCell[69, 2].value));
                this.lstITU.Add(Convert.ToDouble(excelCell[70, 2].value));

            }
            catch (Exception ex)
            {
                ShowMsg($"读取Test plan 出错，{ex.Message}", false);
                return;
            }
            finally
            {
                workbook.Close();
                excelApp.Quit();
                IntPtr t = new IntPtr(excelApp.Hwnd);
                int kill = 0;
                GetWindowThreadProcessId(t, out kill);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(kill);
                p.Kill();
            }
            //});
            //task1.Start();
        }

        /// <summary>
        /// init devices,confirm prodcut temp by wavesub
        /// </summary>
        private void InitInstruments()
        {
            strMsg = "对设备进行初始设置...";
            ShowMsg(strMsg, true);
            //TEC 的初始设置
            try
            {
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[0]);
                this.Temp_Environment = lstTecTemp[0];
                ShowMsg($"将TEC的温度设置为{lstTecTemp[0]}", true);
                TickCountTotal = 0;
                TecTimer.Start();
            }
            catch (Exception ex)
            {
                ShowMsg($"对TEC进行初始设置时出错，{ex.Message}", false);
                return;
            }
           
            //N1092A 的初始设置
            try
            {
                kesight_N1902D.Init();
            }
            catch (Exception ex)
            {
                ShowMsg($"对N1092A进行初始设置时出错，{ex.Message}", false);
                return;
            }
            //K2400的设置
            try
            {
                K2400_1.SetToVoltageSource();
                // K2400_1.SetSOURCEVOLTlevel(K2400_1.Vcc);
                K2400_1.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_1.I_limit);

                K2400_2.SetToVoltageSource();
                // K2400_2.SetSOURCEVOLTlevel(K2400_2.Vcc);
                K2400_2.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_2.I_limit);

                K2400_3.SetToVoltageSource();
                K2400_3.SetTerminalPanel(KEITHLEY2400.TeminalPanel.FRONT);
                //  K2400_3.SetSOURCEVOLTlevel(K2400_3.Vcc);
                K2400_3.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_3.I_limit);
            }
            catch (Exception ex)
            {
                ShowMsg($"对K2400进行初始设置时出错，{ex.Message}", false);
                return;
            }
            //K2000
            try
            {
                K2000.SetMeasureCurrentMode(Keithley2000.EnumDACType.DC);
            }

            catch (Exception ex)
            {
                ShowMsg($"Init K2000 error,{ex.Message}", false);
            }
            //LDT5525B
            try
            {
                //LIMII set to 1A； LIMIT set to 70℃
                L5525B.SetLIMI_I(1);
                L5525B.SetLIMI_T(70);
                L5525B.SetMode(LDT5525B.EnumTECMode.Temperature);
            }
            catch (Exception ex)
            {
                ShowMsg($"Init LDT5525B error,{ex.Message}", false);
            }

            //pm212
            if (!pm212.SetWavelength(PM212.EnumWave.w1310))
            {
                ShowMsg("PM212设置波长时出错", false);
                return;
            }
            try
            {
                //GY7501
                gY7501_I2C = new GY7501_I2C(GY7501_Data);
                GY7501_Data = gY7501_I2C.dataManagement;
            }
            catch (Exception ex)
            {
                ShowMsg($"Init GY7501 error,{ex.Message}", false);
            }
            ShowMsg("设备初始化完成！", true);
        }
        /// <summary>
        /// 实施监控TEC环境温度
        /// </summary>
        private void ReadRealTECTemp_Out()
        {
            try
            {
                if (MonitorTask == null || MonitorTask.IsCanceled || MonitorTask.IsCompleted)
                {
                    cts = new CancellationTokenSource();
                    MonitorTask = new Task(() =>
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            Thread.Sleep(100);
                            if (TC720 != null)
                            {
                                RealTimeTemperature = TC720.ReadTemperature(Channel.CH1);
                                ShowRealTemp_Case(RealTimeTemperature);
                            }
                        }
                    }, cts.Token);
                }
                MonitorTask.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 实施监控产品温度
        /// </summary>
        private void ReadRealTECTemp_Product()
        {
            try
            {
                if (MonitorProductTempTask == null || MonitorProductTempTask.IsCanceled || MonitorProductTempTask.IsCompleted)
                {
                    cts = new CancellationTokenSource();
                    MonitorProductTempTask = new Task(() =>
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            try
                            {
                                Thread.Sleep(100);
                                if (L5525B != null)
                                {
                                    RealTimeTemperature_Product = L5525B.ReadTemperature();
                                    ShowRealTemp_Product(RealTimeTemperature);
                                }
                            }
                            catch (Exception ex)
                            {
                                string err = $" 实时监控产品温度时出错，{ ex.Message}";
                                ShowMsg(err, false);
                                throw new Exception(err);
                            }
                        }
                    }, cts.Token);
                }
                MonitorProductTempTask.Start();
            }
            catch (Exception ex)
            {
                ShowMsg($"实时监控产品温度时出错，{ex.Message}", false);
            }
        }

        public enum CurrentUint { A, mA, uA, nA }
        /// <summary>
        /// K7001切换通道，K2000读取MPD的值,一个通道一次，结果存入list
        /// </summary>
        /// <returns>所有通道的MPD电流值</returns>
        private List<double> ReadMPDAllChannel(CurrentUint unit)
        {
            List<double> lstMpd = new List<double>();
            double factor = 0;
            try
            {
                switch (unit)
                {
                    case CurrentUint.A:
                        factor = 1;
                        break;
                    case CurrentUint.mA:
                        factor = Math.Pow(10, 3);
                        break;
                    case CurrentUint.uA:
                        factor = Math.Pow(10, 6);
                        break;
                    case CurrentUint.nA:
                        factor = Math.Pow(10, 9);
                        break;

                }
                for (int chl = 0; chl < MaxChannel; chl++)
                {

                    K7001.OpenAllRelay();
                    Thread.Sleep(200);
                    K7001.CloseRelay(lstK7001Pin[chl]);
                    Thread.Sleep(100);
                    lstMpd.Add(K2000.Fetch() * factor);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                ShowMsg($"获取MPD电流出错，{ex.Message}", false);
            }
            return lstMpd;
        }

        /// <summary>
        /// 控制GY7501所有TxDisable ratio控件的状态
        /// </summary>
        /// <param name="disable">true:Tx disable,false:Tx enable</param>
        private void ControlGY7501TxDisableRadio(bool disable)
        {
            gY7501_I2C.RadioTx0ChlDisable.GetMouseClick(disable);

            gY7501_I2C.RadioTx1ChlDisable.GetMouseClick(disable);
            gY7501_I2C.RadioTx2ChlDisable.GetMouseClick(disable);
            gY7501_I2C.RadioTx3ChlDisable.GetMouseClick(disable);
        }
        /// <summary>
        /// 测试结束的操作，断电，关闭产品温度，环境温度设回常温，保存测试数据到数据库
        /// </summary>
        private void FinishedTest()
        {
            try
            {
                if (DialogResult.No == MessageBox.Show("测试完成，是否给产品断电？", "产品断电", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;//产品不断电返回
                }
                ShowMsg($"Test process ended,turn off the K2400...", true);
                //产品断电
                PowerOffK2400();
                ShowMsg($"Turn off the LDT5525 ...", true);
                //关闭产品温度
                L5525B.SetOutputStatus(false);
                ShowMsg($"Set the environment temperature to room temperature.", true);
                //设置环境温度为常温
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[0]);//测试完成，将TEC设置回常温
                this.Temp_Environment = lstTecTemp[0];
                TickCountTotal = 0;
                TecTimer.Start();
                if (ctsTotal.Token.IsCancellationRequested)
                    return;
                //保存数据
                if (DialogResult.Yes == MessageBox.Show("测试完成，是否保存数据到数据库？", "保存数据", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {

                    ShowMsg("Save the test data to Database...", true);
                    db.SaveTestData(lstTestDataCommon);

                    ShowMsg("Saving the test data to Database is Done！", true);
                }
                WaitEnvironmTempOK(this.Temp_Environment);
                strMsg = $"测试完成，环境温度已达常温,请卸载产品！";
                ShowMsg(strMsg, true);
               
                //  MessageBox.Show(strMsg, "Test Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //while (!TemperatureIsTimeOut)
                //{
                //    if (TemperatureIsOk)
                //    {
                //        strMsg = $"测试完成，环境温度已达常温,请卸载产品！";
                //        ShowMsg(strMsg, true);
                //        MessageBox.Show(strMsg, "Test Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        break;
                //    }
                //}
                //if (TemperatureIsTimeOut)
                //{
                //    ShowMsg("The temperature couldn't reach the room temperature,pls take care to unload the product ！", false);
                //}
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        #region Private Methods

        /// <summary>
        /// 获取一条测试数据
        /// </summary>
        private void GetTestData_Channel(int chl)
        {
            try
            {
                //GY7501
                TestData_Channel.Vcpa = lstVcpa[chl];
                TestData_Channel.Veq = lstVeq[chl];
                TestData_Channel.Vmod = lstVmod[chl];
                TestData_Channel.Isink = lstIsink[chl];
                TestData_Channel.Ldd = lstLdd[chl];

                //Bert
                TestData_Channel.Pre_Cursor = this.Pre_Cursor_Array[chl];
                TestData_Channel.Main_Cursor = this.Main_Cursor_Array[chl];
                TestData_Channel.Post_Cursor = this.Post_Cursor_Array[chl];
                //N1092D 数据读取
                kesight_N1902D.QueryMeasurementResults();
                TestData_Channel.Er = kesight_N1902D.ER;
                TestData_Channel.Mask_Margin = kesight_N1902D.MaskMargin;
                TestData_Channel.Fall_time = kesight_N1902D.FallTime;
                TestData_Channel.Rise_time = kesight_N1902D.RiseTime;
                TestData_Channel.Jitter_pp = kesight_N1902D.Jitter_pp;
                TestData_Channel.Jitter_rms = kesight_N1902D.JitterRMS;
                TestData_Channel.Crossing = kesight_N1902D.Crossing;
                //AQ6370
                TestData_Channel.SMSR = Math.Round(aQ6370.SMSR, 2);
                TestData_Channel.Cwl = Math.Round(aQ6370.PeakWL, 2);

                //PM212
                //  ShowMsg($"Read power with PM212 in Channel{chl + 1}", true);
                TestData_Channel.Power = pm212.ReadPower() + pm212.lstPower_Offset[chl];
            }
            catch (Exception ex)
            {
                ShowMsg($"{ex.Message}", false);
            }
        }

        /// <summary>
        /// 获取pf结果并在界面显示测试数据
        /// </summary>
        /// <param name="TestData"></param>
        /// <param name="tec_temp_out"></param>
        private void GetPfAndShowTestData(CTestData_Channel TestData, double tec_temp_out, string rate)
        {
            testResult = new string[] { tec_temp_out.ToString(), rate, TestData.Channel.ToString(), TestData.Cwl.ToString(), TestData.SMSR.ToString(), TestData.Power.ToString(), TestData.Impd.ToString(), TestData.Idark.ToString(), TestData.Jitter_pp.ToString(), TestData.Jitter_rms.ToString(), TestData.Crossing.ToString(), TestData.Fall_time.ToString(), TestData.Rise_time.ToString(), TestData.Er.ToString(), TestData.Mask_Margin.ToString() };
            iFail = new Int32[15];

            if (TestData.Cwl >= TestSpec.Cwl_min && TestData.Cwl <= TestSpec.Cwl_max)
            {
                if (TestData.SMSR >= TestSpec.SMSR_min && TestData.SMSR <= TestSpec.SMSR_max)
                {
                    if (TestData.Power >= TestSpec.Power_min && TestData.Power <= TestSpec.Power_max)
                    {
                        if (TestData.Impd >= TestSpec.Impd_min && TestData.Impd <= TestSpec.Impd_max)
                        {
                            if (TestData.Idark >= TestSpec.Idark_min && TestData.Idark <= TestSpec.Idark_max)
                            {
                                if (TestData.Jitter_pp >= TestSpec.Jitter_pp_min && TestData.Jitter_pp <= TestSpec.Jitter_pp_max)
                                {
                                    if (TestData.Jitter_rms >= TestSpec.Jitter_rms_min && TestData.Jitter_rms <= TestSpec.Jitter_rms_max)
                                    {
                                        if (TestData.Crossing >= TestSpec.Crossing_min && TestData.Crossing <= TestSpec.Crossing_max)
                                        {
                                            if (TestData.Fall_time >= TestSpec.Fall_time_min && TestData.Fall_time <= TestSpec.Fall_time_max)
                                            {
                                                if (TestData.Rise_time >= TestSpec.Rise_time_min && TestData.Rise_time <= TestSpec.Rise_time_max)
                                                {
                                                    if (TestData.Er >= TestSpec.Er_min && TestData.Er <= TestSpec.Er_max)
                                                    {
                                                        if (TestData.Mask_Margin >= TestSpec.Mask_margin_min && TestData.Mask_Margin <= TestSpec.Mask_margin_max)
                                                        {
                                                            TestData.Pf = true;
                                                        }
                                                        else
                                                        {
                                                            TestData.Pf = false;
                                                            iFail[14] = 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        TestData.Pf = false;
                                                        iFail[13] = 1;
                                                    }
                                                }
                                                else
                                                {
                                                    TestData.Pf = false;
                                                    iFail[12] = 1;
                                                }
                                            }
                                            else
                                            {
                                                TestData.Pf = false;
                                                iFail[11] = 1;
                                            }
                                        }
                                        else
                                        {
                                            TestData.Pf = false;
                                            iFail[10] = 1;
                                        }
                                    }
                                    else
                                    {
                                        TestData.Pf = false;
                                        iFail[9] = 1;
                                    }
                                }
                                else
                                {
                                    TestData.Pf = false;
                                    iFail[8] = 1;
                                }
                            }
                            else
                            {
                                TestData.Pf = false;
                                iFail[7] = 1;
                            }
                        }
                        else
                        {
                            TestData.Pf = false;
                            iFail[6] = 1;
                        }
                    }
                    else
                    {
                        TestData.Pf = false;
                        iFail[5] = 1;
                    }
                }
                else
                {
                    TestData.Pf = false;
                    iFail[4] = 1;
                }
            }
            else
            {
                TestData.Pf = false;
                iFail[3] = 1;
            }

            ShowResult(testResult, iFail);
        }

        /// <summary>
        /// 确认界面需要输入的控件已输入值
        /// </summary>
        /// <returns></returns>
        private bool InterfaceChecked(CTestDataCommon testDataCommon)
        {
            if (txtSN.Text != null && txtSN.Text != "" && txtOperator.Text != null && this.cbxSelectTestRate.SelectedIndex != -1)
            {

                testDataCommon.SN = txtSN.Text;
                testDataCommon.Operator = txtOperator.Text;
                testDataCommon.Spec_id = TestSpec.ID;
                return true;
            }
            else
            {
                MessageBox.Show("请将输入信息填写完整!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 保存眼图
        /// </summary>
        /// <returns></returns>
        private bool SaveEyeImage(int temp_index, int channel, CTestDataCommon dataCommon)
        {
            string temp_note = "";
            switch (temp_index)
            {
                case 1:
                    temp_note = "room";
                    break;
                case 2:
                    temp_note = "cold";
                    break;
                case 3:
                    temp_note = "hot";
                    break;
            }
            try
            {
                string ImageFilePath = $"{Directory.GetCurrentDirectory()}\\Eye Images";
                if (!Directory.Exists(ImageFilePath))
                {
                    Directory.CreateDirectory(ImageFilePath);
                }
                string ImageFileName = $"{ImageFilePath}\\{DateTime.Now.ToString("yyyy-MM-dd")}";
                if (!Directory.Exists(ImageFileName))
                {
                    Directory.CreateDirectory(ImageFileName);
                }
                string ImageName = $"{ImageFileName}\\{dataCommon.SN}-{dataCommon.Rate}-{temp_note}-Ch{channel}-{DateTime.Now.ToString("hh-mm-ss")}.jpg";
                if (File.Exists(ImageName))
                {
                    if (DialogResult.Yes == MessageBox.Show($"{ImageName}已存在，是否替换？", "眼图图片保存", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        File.Delete(ImageName);
                        kesight_N1902D.SaveImage(ImageName);
                    }
                }
                else
                {
                    kesight_N1902D.SaveImage(ImageName);

                }
                return true;
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                return false;
            }
        }

        /// <summary>
        /// 获取GY7501调试界面的的调试结果值
        /// </summary>
        private void GetDriverGY7501Data()
        {
            try
            {
                if (gY7501_I2C != null)
                {
                    lstVcpa = new List<double>();
                    lstVeq = new List<double>();
                    lstVmod = new List<double>();
                    lstIsink = new List<double>();
                    lstLdd = new List<double>();
                    lstVcpa.Add(Convert.ToDouble(gY7501_I2C.NumTx0Vcpa.Value));
                    lstVcpa.Add(Convert.ToDouble(gY7501_I2C.NumTx1Vcpa.Value));
                    lstVcpa.Add(Convert.ToDouble(gY7501_I2C.NumTx2Vcpa.Value));
                    lstVcpa.Add(Convert.ToDouble(gY7501_I2C.NumTx3Vcpa.Value));

                    lstVeq.Add(Convert.ToDouble(gY7501_I2C.NumTx0Veq.Value));
                    lstVeq.Add(Convert.ToDouble(gY7501_I2C.NumTx1Veq.Value));
                    lstVeq.Add(Convert.ToDouble(gY7501_I2C.NumTx2Veq.Value));
                    lstVeq.Add(Convert.ToDouble(gY7501_I2C.NumTx3Veq.Value));

                    lstVmod.Add(Convert.ToDouble(gY7501_I2C.NumTx0Mod.Value));
                    lstVmod.Add(Convert.ToDouble(gY7501_I2C.NumTx1Mod.Value));
                    lstVmod.Add(Convert.ToDouble(gY7501_I2C.NumTx2Mod.Value));
                    lstVmod.Add(Convert.ToDouble(gY7501_I2C.NumTx3Mod.Value));

                    lstIsink.Add(Convert.ToDouble(gY7501_I2C.NumTx0ISNK.Value));
                    lstIsink.Add(Convert.ToDouble(gY7501_I2C.NumTx1ISNK.Value));
                    lstIsink.Add(Convert.ToDouble(gY7501_I2C.NumTx2ISNK.Value));
                    lstIsink.Add(Convert.ToDouble(gY7501_I2C.NumTx3ISNK.Value));

                    lstLdd.Add(Convert.ToDouble(gY7501_I2C.NumTx0Ldd.Value));
                    lstLdd.Add(Convert.ToDouble(gY7501_I2C.NumTx1Ldd.Value));
                    lstLdd.Add(Convert.ToDouble(gY7501_I2C.NumTx2Ldd.Value));
                    lstLdd.Add(Convert.ToDouble(gY7501_I2C.NumTx3Ldd.Value));
                }
                else
                {
                    throw new Exception("Pls open GY7501 interface and set driver data!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Read driver data from Interface GY7501_I2C error!{ex.Message}");
            }
        }

        /// <summary>
        /// 在界面上显示测试数据及Pass/Fail状态，Fail显示红色
        /// </summary>
        /// <param name="strResult"></param>
        /// <param name="iFail"></param>
        private void SetParameterDetailRaw(string[] strResult, int[] iFail)
        {
            lstViewTestData.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                lvi = new ListViewItem(strResult);

                lstViewTestData.Items.Add(lvi);
                lvi.UseItemStyleForSubItems = false;
                for (int i = 0; i < strResult.Length - 1; i++)
                {
                    if (iFail[i] == 1)
                        lvi.SubItems[i].ForeColor = Color.Red;
                }
            }
            finally
            {
                lstViewTestData.EndUpdate();
            }
        }

        private void ConfirmProductTempByWave()
        {
            ShowMsg($"波长与ITU对比测试...", true);
            try
            {
                this.ProductTempInPlan = lstTempProductInPlan[0];
                SetAndWaitProductTempOK(this.ProductTempSetToDevice);

                PowerOnK2400();

                strMsg = "产品加电已完成，请reset产品，同时解锁K2400！";
                if (MessageBox.Show(strMsg, "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {  //根据GY7501的config文件设置芯片参数
                    GY7501_Data.ReadConfigValues($"{ Directory.GetCurrentDirectory()}\\config\\GY7501_config.csv");
                    GY7501_Data.SetValuesToChip();
                    //ReadK2400();
                    //波长差值大于0.8nm，温度降到中间档，继续波长比对
                    if (GoOnWaveComparisonOrNot(0))
                    {
                        //波长差值大于0.8nm，温度降到最后一档
                        if (GoOnWaveComparisonOrNot(1))
                        {
                            //最后一档温度
                            GoOnWaveComparisonOrNot(2);
                        }
                    }
                    ShowMsg("产品温度确认已完成，请调试眼图!", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"波长对比测试失败，{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PowerOnK2400()
        {
            try
            {
                K2400_3.SetSOURCEVOLTlevel(0);
                K2400_1.SetSOURCEVOLTlevel(0);
                K2400_2.SetSOURCEVOLTlevel(0);
                ShowMsg($"给产品加电...", true);
                K2400_3.OUTPUT(true);
                K2400_1.OUTPUT(true);
                K2400_2.OUTPUT(true);
                K2400_3.SetSOURCEVOLTlevel(K2400_3.Vcc);
                K2400_1.SetSOURCEVOLTlevel(K2400_1.Vcc);
                K2400_2.SetSOURCEVOLTlevel(K2400_2.Vcc);
            }
            catch (Exception ex)
            {
                ShowMsg($"K2400加电失败，{ex.Message}", false);
            }

        }
        private void PowerOffK2400()
        {
            try
            {
                K2400_3.SetSOURCEVOLTlevel(0);
                K2400_1.SetSOURCEVOLTlevel(0);
                K2400_2.SetSOURCEVOLTlevel(0);
                K2400_3.OUTPUT(false);
                K2400_1.OUTPUT(false);
                K2400_2.OUTPUT(false);
            }
            catch (Exception ex)
            {
                ShowMsg($"K2400断电失败，{ex.Message}", false);
            }

        }
        private void ReadK2400()
        {
            K2400_1.Current = K2400_1.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;
            K2400_2.Current = K2400_2.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;
            K2400_3.Current = K2400_3.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;
        }
        /// <summary>
        /// 根据波长差值范围判断，如果大于0.7nm，温度下降一档后返回true，表明可进行再一次的波长对比
        /// </summary>
        /// <returns>true:进入下一次比对；false:不进入下一次比对，此时温度已达标或者产品Fail</returns>
        private bool GoOnWaveComparisonOrNot(int CurrentTempIndex)
        {
            int chlPassNum = 0;
            double waveSub = 0;
            for (int i = 0; i < MaxChannel; i++)
            {  //光开关切换通道，选择通道
                jw8402.SetChannel(i + 1);
                //AQ6370扫描一次，读取peakWL
                aQ6370.StartSweep(lstAQ6370_StartWave[i], lstAQ6370_StopWave[i]);
                waveSub = aQ6370.PeakWL - lstITU[i];
                //某个通道波长与ITU的差值小于0.7，则产品直接fail，无需再进行测试
                if (waveSub < -0.7)
                {
                    strMsg = $"产品温度{ProductTempInPlan}℃下，通道波长与ITU差值已低于0.7nm，判定该产品Fail，停止测试！";
                    ShowMsg(strMsg, false);
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (waveSub > -0.7 & waveSub < 0.7)
                {
                    chlPassNum++;
                }
            }
            //四个通道都在+-0.7范围内，温度pass，无需降温，返回false
            if (chlPassNum == 4)
            {
                strMsg = $"产品温度{ProductTempInPlan}℃下，四个通道波长与ITU差值在-0.7~0.7nm，温度pass，可以继续测试！";
                ShowMsg(strMsg, true);
                IsWaveSubOk = true;
                return false;
            }
            //有通道差值大于0.7，需降温
            else
            {
                //如果当前温度不是最后一档温度，下降温度到下一档
                if (CurrentTempIndex < 2)
                {
                    this.ProductTempInPlan = lstTempProductInPlan[CurrentTempIndex + 1];
                    //下一档的温度设置成功，返回true，表明可进行洗一次的GoOnWaveComparisonOrNot
                    SetAndWaitProductTempOK(this.ProductTempSetToDevice);
                    return true;
                }
                //如果当前温度是最后一档温度，不再继续下降温度
                else
                {
                    strMsg = "最后一档温度，波长差依然大于0.7nm，该产品Fail！";
                    ShowMsg(strMsg, false);
                    MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// 设置环境温度，温度设置成功并稳定，返回true
        /// </summary>
        /// <param name="tempSetToDevice">要设置的温度</param>
        /// <returns></returns>
        private void SetAndWaitProductTempOK(double tempSetToDevice)
        {
            L5525B.SetTemperature(tempSetToDevice);
            TickCountTotal_Product = 0;
            this.Invoke(new Action(() =>
            {
                ProductTempTimer1.Start();
            }));
            Thread.Sleep(1000);

            ShowMsg($"设置产品温度为 {this.ProductTempInPlan}℃，等待中...", true);
            //直到产品温度达到要求，开始给产品加电         

            while (!TemperatureIsTimeOut_Product)
            {
                if (TemperatureIsOk_Product)
                {
                    return;
                }
            }
            if (TemperatureIsTimeOut_Product)
            {
                if (DialogResult.Yes == MessageBox.Show($"产品温度设置已经超过{L5525B.TimeOut}s，还未达到设定温度{ProductTempInPlan},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    TemperatureIsOk_Product = true;
                    ShowMsg($"产品温度设置未达到设定值{ProductTempInPlan}℃，但可以继续进行测试！", true);
                    return;
                }
                else
                {
                    strMsg = $"产品温度设置未达到设定值{ProductTempInPlan}℃，不可以继续进行测试！";
                    ShowMsg(strMsg, false);
                    throw new Exception(strMsg);
                }
            }
        }

        #endregion

        #region 
        private void Deinit()
        {
            ShowMsg("断开设备连接...", true);
            //Inst_PAM4_Bert.();
            TC720.DeInit();
            ShowMsg("设备连接已断开...", true);

        }
        private void DisableContols()
        {
            foreach (Control whole in Controls)
            {
                whole.Enabled = false;
            }
        }
        private void EnableControls()
        {
            foreach (Control whole in Controls)
            {
                whole.Enabled = true;
            }
        }

        #endregion

        #endregion

        #region Callback Methods

        private void LWDM_Tx_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Deinit();
        }

        private void LoadTestSpec(object sender, EventArgs e)
        {
            TestSpec = new CTestSpec();
            //  this.lstViewTestData.Items.Clear();
            // this.lstViewLog.Items.Clear();
            if (this.cbxPN.SelectedIndex != -1)
            {
                PN = this.cbxPN.SelectedItem.ToString();
                TestSpec = db.GetTestSpec(PN);
                ReadTestPlan(PN);
                InitInstruments();
                ShowMsg("请选择Rate！", true);
            }
        }
        private void WaitEnvironmTempOK(double temp)
        {
            ShowMsg($"等待环境温度设为 {temp}℃...", true);
            //等待环境温度达到设定值并稳定

            Thread.Sleep(500);//当前线程阻塞500ms，这样会先执行timer的回调函数
            while (!TemperatureIsTimeOut)
            {
                if (TemperatureIsOk)
                {
                    return;
                }
            }
            if (TemperatureIsTimeOut)
            {
                if (DialogResult.Yes == MessageBox.Show($"环境温度设置已经超过{TC720.TimeOut}s，还未达到设定温度{temp},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    TemperatureIsOk = true;
                    ShowMsg($"环境温度设置未达到设定值{temp}℃，但可以继续进行测试！", true);
                    return;
                }
                else
                {
                    strMsg = $"环境温度设置未达到设定值{temp}℃，不可以继续进行测试！";
                    ShowMsg(strMsg, false);
                    throw new Exception(strMsg);
                }
            }
        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Deinit();
        }

        /// <summary>
        /// 进行一个完整的测试并保存数据
        /// </summary>
        private void btnTestProcess_Click(object sender, EventArgs e)
        {
            if (!IsWaveSubOk)
            {
                strMsg = "产品温度确认失败，不能进行测试！";
                ShowMsg(strMsg, false);
                MessageBox.Show(strMsg, "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ReadK2400();

            this.lstViewTestData.Items.Clear();
            this.lstViewLog.Items.Clear();

            CTestDataCommon testDataCommon = new CTestDataCommon();

            if (!InterfaceChecked(testDataCommon))
            {
                return;
            }

            ShowMsg("Start Test Process...", true);
            testDataCommon.Test_Start_Time = DateTime.Now.ToString();
            testDataCommon.Rate = this.TestRate;
            var strRate = this.cbxSelectTestRate.Text;
            //如果是25G & 28G，则会测试2个速率，得到两组testcommon数据
            if (strRate == "25G & 28G")
            {
                lstTestDataCommon = Enumerable.Repeat<CTestDataCommon>(testDataCommon, 2).ToList();//该list长度为2，内容为testDataCommon
            }
            else
            {
                lstTestDataCommon = Enumerable.Repeat<CTestDataCommon>(testDataCommon, 1).ToList();
            }
            try
            {
                GetDriverGY7501Data();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                return;
            }
            try
            {
                var testData_Temp = new CTestData_Temp();

                testData_Temp.Temp_in = this.ProductTempInPlan;
                testData_Temp.Vcc1 = Convert.ToDouble(K2400_1.Vcc);
                testData_Temp.Vcc2 = Convert.ToDouble(K2400_2.Vcc);
                testData_Temp.Vcc3 = Convert.ToDouble(K2400_3.Vcc);

                testData_Temp.Icc1 = K2400_1.Current;
                testData_Temp.Icc2 = K2400_2.Current;
                testData_Temp.Icc3 = K2400_3.Current;
                ctsTotal = new CancellationTokenSource();

                var task = new Task(() =>
                {
                    for (int TecTempIndex = 0; TecTempIndex < lstTecTemp.Count(); TecTempIndex++)
                    {
                        testData_Temp.Temp_out = lstTecTemp[TecTempIndex];

                        ShowMsg($"Set environment temperature to {lstTecTemp[TecTempIndex]}...", true);
                        TC720.WriteTemperature(Channel.CH1, lstTecTemp[TecTempIndex]);
                        this.Temp_Environment = lstTecTemp[TecTempIndex];
                        TickCountTotal = 0;
                        TecTimer.Start();
                        Thread.Sleep(200);
                        WaitEnvironmTempOK(this.Temp_Environment);
                        //L5525B.SetTemperature(ProductTempSetToDevice);
                        //ShowMsg($"Set product temperature to {this.ProductTempInPlan}...", true);

                        //TickCountTotal_Product = 0;
                        //ProductTempTimer1.Start();

                        //等待环境温度和产品温度达到设定值并稳定
                        //ShowMsg("等待环境温度和产品温度达到设定值...", true);

                        //Thread.Sleep(500);//当前线程阻塞500ms，这样会先执行timer的回调函数
                        //while (!TemperatureIsOk | !TemperatureIsOk_Product)
                        //{
                        //    if (ctsTotal.Token.IsCancellationRequested)
                        //    {
                        //        ShowMsg($"Test is stopped!!!", false);
                        //        return;
                        //    }
                        //    Thread.Sleep(300);
                        //    if (TemperatureIsTimeOut_Product | TemperatureIsTimeOut)
                        //    {
                        //        ShowMsg("Time is out for setting Temperature!", false);
                        //        //温度设置超时，用户选择Yes则break当前的while循环，继续执行下面的语句；用户选No，则return结束整个测试函数
                        //        if (DialogResult.No == MessageBox.Show($"温度设置已经超时，还未达到设定温度,是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        //        {
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        if (ctsTotal.Token.IsCancellationRequested)
                        {
                            ShowMsg($"Test is stopped!!!", false);
                            return;
                        }
                        TestProcessWithSpecificTemp(TecTempIndex, lstTestDataCommon[0], testData_Temp);
                        if (strRate == "25G & 28G")
                        {
                            SelectAndSetRate(EnumRate._25G);

                            TestProcessWithSpecificTemp(TecTempIndex, lstTestDataCommon[1], testData_Temp);
                            //如果不是最后一个温度，则速率设置回28G
                            if (TecTempIndex < lstTecTemp.Count - 1)
                            {
                                SelectAndSetRate(EnumRate._28G);
                            }
                        }
                    }
                    try
                    {
                        FinishedTest();
                    }
                    catch (Exception ex)
                    {
                        ShowMsg($"Error happened when saving test data,{ex.Message}", true);
                    }
                });
                task.Start();
                //  DisableContols();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                EnableControls();
            }
        }
        private enum EnumRate
        {
            _25G,
            _28G
        }

        /// <summary>
        /// 速率确定后，给相关参数赋想要的值
        /// </summary>
        /// <param name="rate">25G或者28G</param>
        private void SelectAndSetRate(EnumRate rate)
        {
            switch (rate)
            {
                case EnumRate._25G:
                    this.TestRate = "25G";
                    Inst_Bert.Ppg_data_rate = Bert.PPGRATE25G;
                    kesight_N1902D.MaskMarginPattern = kesight_N1902D.MaskPattern25G;
                    kesight_N1902D.Rate = Kesight_N1092D.RATE25G;
                    break;
                case EnumRate._28G:
                    this.TestRate = "28G";
                    Inst_Bert.Ppg_data_rate = Bert.PPGRATE28G;
                    kesight_N1902D.MaskMarginPattern = kesight_N1902D.MaskPattern28G;
                    kesight_N1902D.Rate = Kesight_N1092D.RATE28G;
                    break;
            }
            Inst_Bert.SetBert();
            kesight_N1902D.SetN1092();
        }
        private void TestProcessWithSpecificTemp(int TecTempIndex, CTestDataCommon dataCommon, CTestData_Temp TestData_Temp)
        {
            dataCommon.Rate = this.TestRate;
            TestData_Temp.lstTestData_Channel = new List<CTestData_Channel>();
            for (int channel = 0; channel < MaxChannel; channel++)
            {
                TestData_Channel = new CTestData_Channel();
                TestData_Channel.Channel = channel + 1;
                // ShowMsg($"Switch Optical Channel to channel{channel + 1}", true);
                if (ctsTotal.Token.IsCancellationRequested)
                {
                    ShowMsg($"Test is stopped!!!", false);
                    return;
                }
                if (!jw8402.SetChannel(TestData_Channel.Channel))
                {
                    ShowMsg($"Error happened when switching Optical Channel to channel{channel + 1}", false);
                }

                this.Invoke(new Action(() => { this.cbxChlIndex.SelectedIndex = channel; }));

                ShowMsg($"Start testing in {lstTecTemp[TecTempIndex]}℃ and channel {channel + 1}...", true);
                ShowMsg($"Running Eye Diagram...", true);
                if (ctsTotal.Token.IsCancellationRequested)
                {
                    ShowMsg($"Test is stopped!!!", false);
                    return;
                }
                kesight_N1902D.Run();
                if (ctsTotal.Token.IsCancellationRequested)
                {
                    ShowMsg($"Test is stopped!!!", false);
                    return;
                }
                ShowMsg($"AQ6370 Sweeping...", true);

                aQ6370.StartSweep(lstAQ6370_StartWave[channel], lstAQ6370_StopWave[channel]);
                if (ctsTotal.Token.IsCancellationRequested)
                {
                    ShowMsg($"Test is stopped!!!", false);
                    return; ;
                }
                //   ShowMsg($"Read test data in {lstTecTemp[TecTempIndex]}℃ and channel {channel + 1}...", true);

                GetTestData_Channel(channel);

                // ShowMsg("Save Eye Diagram...", true);
                SaveEyeImage(TecTempIndex, channel, dataCommon);
                // progHandle.Report(100);
                //添加一条测试数据到TestData_Temp
                TestData_Temp.lstTestData_Channel.Add(TestData_Channel);
            }
            if (ctsTotal.Token.IsCancellationRequested)
            {
                ShowMsg($"Test is stopped!!!", false);
                return;
            }
            //  ShowMsg("Read TEC Current with LDT5525B", true);
            TestData_Temp.Itec = L5525B.ReadCurrent();

            if (ctsTotal.Token.IsCancellationRequested)
            {
                ShowMsg($"Test is stopped!!!", false);
                return; ;
            }

            ShowMsg("Start MPD testing... ", true);
            var lstMpd = ReadMPDAllChannel(CurrentUint.mA);
            //   ShowMsg("Start Idark testing... ", true);
            // ShowMsg("Disable all GY7501 Tx Channel.", true);
            ControlGY7501TxDisableRadio(true);
            var lstIdark = ReadMPDAllChannel(CurrentUint.nA);
            //  ShowMsg("Finished Idark Test,Enable all GY7501 Tx Channel.", true);
            ControlGY7501TxDisableRadio(false);
            if (ctsTotal.Token.IsCancellationRequested)
            {
                ShowMsg($"Test is stopped!!!", false);
                return;
            }
            //将lstIdark和lstMpd 插入当前TestData_Temp的lstTestData_Channel中，通道是一一对应的，所以可以使用索引
            // ShowMsg($"Start dealing with test data...", true);
            for (int ch = 0; ch < MaxChannel; ch++)
            {
                if (ctsTotal.Token.IsCancellationRequested)
                {
                    ShowMsg($"Test is stopped!!!", false);
                    return;
                }
                TestData_Temp.lstTestData_Channel[ch].Idark = lstIdark[ch];
                TestData_Temp.lstTestData_Channel[ch].Impd = lstMpd[ch];
                //一个通道获取一次pf结果，并在界面显示一行
                GetPfAndShowTestData(TestData_Temp.lstTestData_Channel[ch], lstTecTemp[TecTempIndex], dataCommon.Rate);

                TestData_Temp.Pf = true & TestData_Temp.lstTestData_Channel[ch].Pf;
            }
            //获取只与温度有关的测试参数的pf
            if (TestData_Temp.Itec >= TestSpec.Itec_min & TestData_Temp.Itec <= TestSpec.Itec_max)
            {
                TestData_Temp.Pf &= true;
            }
            else
            {
                TestData_Temp.Pf = false;
            }
            dataCommon.Pf &= TestData_Temp.Pf;
            //添加TestData_Temp即4条数据到TestDataCommon
            dataCommon.lstTestData_Temp.Add(TestData_Temp);
            if (ctsTotal.Token.IsCancellationRequested)
                return;
            //当前测试结果为fail，且Test plan设置fail后不继续测试，则跳出循环
            if (TecTempIndex < lstTecTemp.Count - 1)
            {
                if (TestData_Temp.Pf == false && lstTecTempNote[TecTempIndex].ToUpper().Contains("FALSE"))
                {
                    strMsg = $"{lstTecTemp[TecTempIndex]}℃下测试Fail，根据TestPlan的设置，余下的温度无需测试！";
                    ShowMsg(strMsg, true);
                    MessageBox.Show(strMsg, "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            //   ShowMsg($"Finish test under temperature{lstTecTemp[TecTempIndex]}!", true);
        }
        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            ReadK2400();

            ShowMsg("AutoScaling Eye diagran，pls wait...", true);
            try
            {
                //var task = new Task(() =>
                //{
                kesight_N1902D.SetN1092();
                kesight_N1902D.AutoScale();
                //});
                //task.Start();
                ShowMsg("AutoScaling Eye diagran Done!", true);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ReadK2400();
            try
            {
                ShowMsg("Running Eye diagram，pls wait...", true);

                var task = new Task(() =>
                {
                    kesight_N1902D.SetN1092();
                    kesight_N1902D.Run();
                });
                task.Start();
                task.Wait();
                ShowMsg("Running Eye Done!", true);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        private void btnOpenGY1202Interface_Click(object sender, EventArgs e)
        {
            if (!TemperatureIsOk | !TemperatureIsOk_Product)
            {
                strMsg = $"The temperature haven't reached the set point,can't set driver data！";
                ShowMsg(strMsg, false);
                return;
            }

            // gY7501_I2C = new GY7501_I2C(GY7501_Data);
            gY7501_I2C.Show();
        }

        private void cbxChlIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!jw8402.SetChannel(this.cbxChlIndex.SelectedIndex + 1))
            {
                ShowMsg($"Error happened when Switching optical channel to CH{this.cbxChlIndex.SelectedIndex + 1}！", false);
            }
        }

        private void btnSetProductTemp1_Click(object sender, EventArgs e)
        {
            if (cbxPN.SelectedIndex == -1)
            {
                ShowMsg("Pls choose PN at first!", false);
                return;
            }
            if (this.txtProductTemp_Room.Text == null | this.txtProductTemp_Room.Text == "")
            {
                ShowMsg("Pls input the product temperature at first!", false);
                return;
            }
            this.ProductTempInPlan = Convert.ToDouble(this.txtProductTemp_Room.Text);

            Task task = new Task(() =>
            {
                SetAndWaitProductTempOK(this.ProductTempSetToDevice);

                PowerOnK2400();
            });
            task.Start();
        }

        private void btnRestTemp_Click(object sender, EventArgs e)
        {
            if (this.txtEnvirnTemp.Text != "")
            {
                this.Temp_Environment = Convert.ToDouble(this.txtEnvirnTemp.Text);
                TC720.WriteTemperature(Channel.CH1, this.Temp_Environment);//将TEC设置回常温
                TickCountTotal = 0;
                TecTimer.Start();
            }
            else
            {
                ShowMsg("请先输入环境温度!", false);
            }
        }

        private void TECTimer_Tick(object sender, EventArgs e)
        {
            TickCountTotal++;
            if (RealTimeTemperature >= this.Temp_Environment - TC720.TempSpan && RealTimeTemperature <= this.Temp_Environment + TC720.TempSpan)
            {
                if (++TickCount > TC720.StablizaitonTime)
                {
                    TemperatureIsOk = true;
                    TickCount = TC720.StablizaitonTime + 1; //防止一直加
                }
                else
                    TemperatureIsOk = false;
            }
            else
            {
                TickCount = 0;
                TemperatureIsOk = false;
                TemperatureIsTimeOut = false;
                if (TickCountTotal > TC720.TimeOut)
                {
                    TecTimer.Stop();
                    TemperatureIsTimeOut = true;
                    if (DialogResult.Yes == MessageBox.Show($"环境温度设置已经超过{TC720.TimeOut}s，还未达到设定温度{this.Temp_Environment},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        TemperatureIsOk = true;
                        ShowMsg($"环境温度设置未达到设定值{TC720.TimeOut}℃，但可以继续进行测试！", true);
                    }
                    else
                    {
                        ShowMsg($"温度设置未达到设定值{TC720.TimeOut}℃，不可以继续进行测试！", false);
                    }
                }
            }
        }

        private void ProductTempTimer_Tick(object sender, EventArgs e)
        {
            TickCountTotal_Product++;
            TemperatureIsOk_Product = false;
            if (RealTimeTemperature_Product >= this.ProductTempInPlan - L5525B.TempSpan && RealTimeTemperature_Product <= this.ProductTempInPlan + L5525B.TempSpan)
            {
                if (++TickCount_Product > L5525B.StablizationTime)
                {
                    TemperatureIsOk_Product = true;
                    TickCount_Product = L5525B.StablizationTime + 1; //防止一直加
                }
                else
                    TemperatureIsOk_Product = false;
            }
            else
            {
                TickCount_Product = 0;
                TemperatureIsOk_Product = false;
                TemperatureIsTimeOut_Product = false;
                if (TickCountTotal_Product > L5525B.TimeOut)
                {
                    ProductTempTimer1.Stop();
                    TemperatureIsTimeOut_Product = true;
                }
            }
        }

        private void txtSN_TextChanged(object sender, EventArgs e)
        {
            this.lstViewLog.Items.Clear();
            this.lstViewTestData.Items.Clear();
        }

        private void btnSetPre_Cusor_Click(object sender, EventArgs e)
        {
            Inst_Bert.Inst_PAM4_Bert.setPretap(Bert_Channel, Convert.ToDouble(this.txtPre_Cursor.Text));
            this.lblPre_Cursor.Text = this.txtPre_Cursor.Text;
            Pre_Cursor_Array[Bert_Channel] = Convert.ToDouble(txtPre_Cursor.Text);
        }

        private void btnSetMain_Cusor_Click(object sender, EventArgs e)
        {
            Inst_Bert.Inst_PAM4_Bert.setMaintap(Bert_Channel, Convert.ToDouble(this.txtMain_Cursor.Text));
            this.lblMain_Cursor.Text = this.txtMain_Cursor.Text;
            Main_Cursor_Array[Bert_Channel] = Convert.ToDouble(txtMain_Cursor.Text);

        }

        private void btnSetPost_Cusor_Click(object sender, EventArgs e)
        {
            Inst_Bert.Inst_PAM4_Bert.setPosttap(Bert_Channel, Convert.ToDouble(this.txtPost_Cursor.Text));
            this.lblPost_Cursor.Text = this.txtPost_Cursor.Text;
            Post_Cursor_Array[Bert_Channel] = Convert.ToDouble(txtPost_Cursor.Text);
        }


        private void btnStopTestProcess_Click(object sender, EventArgs e)
        {
            ctsTotal?.Cancel();
            Thread.Sleep(100);
        }

        private void TrackBarPre_Cursor_ValueChanged(object sender, EventArgs e)
        {
            Inst_Bert.Inst_PAM4_Bert.setPretap(Bert_Channel, TrackBarPre_Cursor.Value);
            string value = Inst_Bert.Inst_PAM4_Bert.queryPreTap(Bert_Channel);
            Pre_Cursor_Array[Bert_Channel] = Convert.ToDouble(value);
            lblPre_Cursor.Text = value;
            this.txtPre_Cursor.Text = value;
        }

        private void TrackBarMain_Cursor_ValueChanged(object sender, EventArgs e)
        {
            lblMain_Cursor.Text = TrackBarMain_Cursor.Value.ToString();
            Inst_Bert.Inst_PAM4_Bert.setMaintap(Bert_Channel, TrackBarMain_Cursor.Value);
            string value = Inst_Bert.Inst_PAM4_Bert.queryMain_Tap(Bert_Channel);
            Main_Cursor_Array[Bert_Channel] = Convert.ToDouble(value);
            this.txtMain_Cursor.Text = value;
        }

        private void TrackBarPost_Cursor_ValueChanged(object sender, EventArgs e)
        {
            lblPost_Cursor.Text = TrackBarPost_Cursor.Value.ToString();
            Inst_Bert.Inst_PAM4_Bert.setPosttap(Bert_Channel, TrackBarPost_Cursor.Value);
            string value = Inst_Bert.Inst_PAM4_Bert.queryPostTap(Bert_Channel);

            Post_Cursor_Array[Bert_Channel] = Convert.ToDouble(value);
            this.txtPost_Cursor.Text = value;
        }
        #endregion

        private void cbxBertChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Bert_Channel = this.cbxBertChannel.SelectedIndex;
            this.label14.Text = $"CH{this.Bert_Channel + 1}";
        }

        private void cbxSelectTestRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbxPN.SelectedValue == null | this.cbxPN.SelectedValue.ToString() == "")
            {
                this.cbxSelectTestRate.SelectedIndexChanged -= cbxSelectTestRate_SelectedIndexChanged;
                this.cbxSelectTestRate.SelectedIndex = -1;
                this.cbxSelectTestRate.SelectedIndexChanged += cbxSelectTestRate_SelectedIndexChanged;

                MessageBox.Show("请先选择PN!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            switch (this.cbxSelectTestRate.Text)
            {
                case "25G":
                    SelectAndSetRate(EnumRate._25G);
                    break;
                case "28G":
                case "25G & 28G":
                    SelectAndSetRate(EnumRate._28G);
                    break;
            }
            ShowMsg("速率设置已完成，请确认产品温度！", true);
        }

        private void lstViewLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnConfirmProdcutTemp_Click(object sender, EventArgs e)
        {
            try
            {
                Task task = new Task(() =>
                {
                    WaitEnvironmTempOK(this.Temp_Environment);
                    ConfirmProductTempByWave();
                });
                task.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
