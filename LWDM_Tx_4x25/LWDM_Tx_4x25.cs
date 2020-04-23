using LWDM_Tx_4x25.Instruments;
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

namespace LWDM_Tx_4x25
{
    public partial class LWDM_Tx_4x25 : Form
    {
        #region Properties

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public CDatabase db = new CDatabase();
        public ConfigManagement cfg = new ConfigManagement();
        public Bert Inst_Bert;
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

        public Kesight_N1092D kesight_N1902D;
        delegate void ThreedShowMsgDelegate(string Message, bool bPass);
        delegate void ThreedShowTempDelegate(double Temp);
        private string strMsg;
        public string PN;
        public CTestSpec TestSpec;
        public CTestData_Channel TestData_Channel;
        public CTestData_Temp TestData_Temp;
        private List<double> lstTecTemp;
        private List<double> lstTecTemp_Product;
        private List<string> lstTecTempNote;

        private double Temp_Environment;
        private CTestDataCommon TestDataCommon;

        public List<double> lstVcpa;
        public List<double> lstVeq;
        public List<double> lstVmod;
        public List<double> lstIsink;
        public List<double> lstLdd;

        public List<string> lstK7001Pin;
        public const double MaxChannel = 4;

        delegate void ThreedShowResultDelegate(string[] Message, int[] iPass);
        private CancellationTokenSource cts = null;
        private Task MonitorTask = null;
        private Task MonitorProductTempTask = null;
        private double ProductTemp;

        /// <summary>
        /// 子窗体对象，全局使用
        /// </summary>
        private GY7501_I2C gY7501_I2C;

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
                            ShowMsg($"环境温度已经稳定为{this.Temp_Environment}左右，可以进行测试!", true);
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
                        ProductTempTimer.Stop();
                        if (TickCountTotal_Product <= L5525B.TimeOut)
                        {
                            ShowMsg($"产品温度已经稳定为{this.ProductTemp}左右...", true);
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
            ShowMsg("Open All Devices...", true);
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
                ShowMsg($"{ex.Message}",false);
                return;
            }

            ShowMsg("Getting PN numbers from database...", true);
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
        }

        #region Interface Display Methods

        public void ShowMsg(string msg, bool bPass)
        {
            this.BeginInvoke(new ThreedShowMsgDelegate(MsgEvent), new object[] { msg, bPass });
        }
        public void ShowResult(string[] Msg, int[] iPass)
        {
            this.Invoke(new ThreedShowResultDelegate(SetParameterDetailRaw), new object[] { Msg, iPass });//, new object[] { iPass });
        }
        public void ShowRealTemp_Case(double Temp)
        {
            this.BeginInvoke(new ThreedShowTempDelegate(ShowTemp), new object[] { Temp });
        }

        public void ShowRealTemp_Product(double Temp)
        {
            this.BeginInvoke(new ThreedShowTempDelegate(ShowTemp_Product), new object[] { Temp });
        }

        private void MsgEvent(string msg, bool bPass)
        {
            //txtLog.AppendText(msg + "\r\n\r\n");
            lstViewLog.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                string temp = "";
                if (msg.Contains("."))
                {
                    int iPos = msg.IndexOf('.');
                    temp = msg.Substring(0, iPos - 1);
                    lvi = lstViewLog.FindItemWithText(temp);
                    if (lvi != null)
                    {
                        lstViewLog.Items.Remove(lvi);
                    }
                }
                lvi = new ListViewItem(msg);
                lstViewLog.Items.Add(lvi);
                //lvi.UseItemStyleForSubItems = false;

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
        private void ReadTestPlanAndInitInstruments(string pn)
        {
            lstTecTempNote = new List<string>();
            strMsg = "正在获取test plan 的内容,请稍等...";
            ShowMsg(strMsg, true);

            var task1 = new Task(() =>
            {
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
                    var com = excelCell[4, 2].value;
                    //// Inst_Bert = new Bert(com);
                    Inst_Bert.Ppg_data_rate = excelCell[5, 2].value;
                    Inst_Bert.Ppg_PRBS_pattern = Convert.ToString(excelCell[6, 2].value);
                    Inst_Bert.Clock = excelCell[7, 2].value;

                    //K2400
                    //  var gpibAddr = Convert.ToUInt16(excelCell[10, 2].value);
                    //K2400_1 = new KEITHLEY2400(gpibAddr);
                    K2400_1.Vcc = Convert.ToDecimal(excelCell[11, 2].value);
                    K2400_1.I_limit = Convert.ToDecimal(excelCell[12, 2].value);

                   // gpibAddr = Convert.ToUInt16(excelCell[15, 2].value);
                   // K2400_2 = new KEITHLEY2400(gpibAddr);
                    K2400_2.Vcc = Convert.ToDecimal(excelCell[16, 2].value);
                    K2400_2.I_limit = Convert.ToDecimal(excelCell[17, 2].value);

                   // gpibAddr = Convert.ToUInt16(excelCell[20, 2].value);
                    //K2400_3 = new KEITHLEY2400(gpibAddr);
                    K2400_3.Vcc = Convert.ToDecimal(excelCell[21, 2].value);
                    K2400_3.I_limit = Convert.ToDecimal(excelCell[22, 2].value);

                    //K2000
                   // gpibAddr = Convert.ToUInt16(excelCell[25, 2].value);
                   // K2000 = new Keithley2000(gpibAddr);

                    //K7001
                   //gpibAddr = Convert.ToUInt16(excelCell[28, 2].value);
                   // K7001 = new Keithley7001(gpibAddr);
                    lstK7001Pin = new List<string>();
                    lstK7001Pin.Add(excelCell[29, 2].value);
                    lstK7001Pin.Add(excelCell[30, 2].value);
                    lstK7001Pin.Add(excelCell[31, 2].value);
                    lstK7001Pin.Add(excelCell[32, 2].value);

                    //N1092 paras
                    kesight_N1902D.Channel = Convert.ToString(excelCell[35, 2].value);
                    kesight_N1902D.Channel_bandWidth = Convert.ToString(excelCell[36, 2].value);
                    kesight_N1902D.AOP_Offset = Convert.ToDouble(excelCell[37, 2].value);

                    //T720
                   // com = excelCell[40, 2].value;
                   // TC720 = new TC720(com);
                    lstTecTemp = new List<double>();
                    lstTecTemp.Add(excelCell[41, 2].value);
                    lstTecTemp.Add(excelCell[42, 2].value);
                    lstTecTemp.Add(excelCell[43, 2].value);
                    lstTecTempNote.Add(Convert.ToString(excelCell[41, 3].value));
                    lstTecTempNote.Add(Convert.ToString(excelCell[42, 3].value));
                    lstTecTempNote.Add(Convert.ToString(excelCell[43, 3].value));
                    TC720.TempSpan = excelCell[44, 2].value;
                    TC720.StablizaitonTime = Convert.ToInt32(excelCell[45, 2].value);
                    TC720.TimeOut = Convert.ToInt32(excelCell[46, 2].value);

                    //LDT5525B
                   // com = excelCell[49, 2].value;
                    //L5525B = new LDT5525B(com);
                    L5525B.StablizationTime = Convert.ToInt32(excelCell[50, 2].value);
                    L5525B.TempSpan = Convert.ToDouble(excelCell[51, 2].value);
                    L5525B.TimeOut = Convert.ToInt32(excelCell[52, 2].value);
                    L5525B.TempOffset = Convert.ToDouble(excelCell[53, 2].value);

                    //AQ6730
                   // gpibAddr = Convert.ToUInt16(excelCell[56, 2].value);
                    //aQ6370 = new AQ6370(gpibAddr);
                    lstAQ6370_StartWave = new List<double>();
                    lstAQ6370_StopWave = new List<double>();
                    this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[57, 2].value));
                    this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[59, 2].value));
                    this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[61, 2].value));
                    this.lstAQ6370_StartWave.Add(Convert.ToDouble(excelCell[63, 2].value));
                    this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[58, 2].value));
                    this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[60, 2].value));
                    this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[62, 2].value));
                    this.lstAQ6370_StopWave.Add(Convert.ToDouble(excelCell[64, 2].value));

                    //PM212
                   // com = excelCell[67, 2].value;
                   // pm212 = new PM212(com);
                    pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[68, 2].value));
                    pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[69, 2].value));
                    pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[70, 2].value));
                    pm212.lstPower_Offset.Add(Convert.ToDouble(excelCell[71, 2].value));

                    //JW8402
                   // com = excelCell[74, 2].value;
                    //jw8402 = new JW8402(com);
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
            });
            task1.Start();
            task1.Wait();

            strMsg = "根据Test Plan对设备进行初始设计...";
            ShowMsg(strMsg, true);
            //TEC 的初始设置
            try
            {
                TC720.WriteTemperature(0, lstTecTemp[0]);
                this.Temp_Environment = lstTecTemp[0];
                ShowMsg($"将TEC的温度设置为{lstTecTemp[0]}", true);
                TickCountTotal = 0;
                TecTimer.Start();
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对TEC进行初始设置时出错，{ex.Message}", false);
                return;
            }
            //Bert的初始设置
            try
            {
                ShowMsg("根据test plan对Bert进行初始设置", true);
                Inst_Bert.SetBert();
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对Bert进行初始设置时出错，{ex.Message}", false);
                return;
            }

            //N1092A 的初始设置
            try
            {
                kesight_N1902D.Init();
                ShowMsg("根据test plan对N1092进行初始设置", true);
                kesight_N1902D.SetN1092();
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对N1092A进行初始设置时出错，{ex.Message}", false);
                return;
            }
            //K2400的设置
            try
            {
                ShowMsg("根据test plan对K2400进行初始设置", true);

                K2400_1.SetToVoltageSource();
                K2400_1.SetSOURCEVOLTlevel(K2400_1.Vcc);
                K2400_1.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_1.I_limit);

                K2400_2.SetToVoltageSource();
                K2400_2.SetSOURCEVOLTlevel(K2400_2.Vcc);
                K2400_2.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_2.I_limit);

                K2400_3.SetToVoltageSource();
                K2400_3.SetTerminalPanel(KEITHLEY2400.TeminalPanel.FRONT);
                K2400_3.SetSOURCEVOLTlevel(K2400_3.Vcc);
                K2400_3.SetComplianceofCURR(KEITHLEY2400.ComplianceLIMIT.REAL, K2400_3.I_limit);
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对K2400进行初始设置时出错，{ex.Message}", false);
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
            catch(Exception ex)
            {
                ShowMsg($"Init LDT5525B error,{ex.Message}", false);
            }

            //pm212
           if(!pm212.SetWavelength(PM212.EnumWave.w1310))
            {
                ShowMsg("PM212设置波长时出错", false);
                return;
            }
            //GYI2C
            if (USB_I2C_Adapter.GYI2C_Open(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
            {
                ShowMsg("GYI2C设备打开失败！", false);
                return;
            }
            //设置I2C Adapter 模式和时钟
            if (USB_I2C_Adapter.GYI2C_SetMode(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
            {
                ShowMsg("设置I2C 适配器的Mode出错！", false);
                return;
            }
            if (USB_I2C_Adapter.GYI2C_SetClk(USB_I2C_Adapter.DEV_GY7501A, 0, 100) != 1)
            {
                ShowMsg("设置I2C 适配器的时钟出错！", false);
                return;
            }
            ShowMsg("初始设置已完成", true);
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
                            catch(Exception ex)
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

        /// <summary>
        /// K7001切换通道，K2000读取MPD的值,一个通道一次，结果存入list
        /// </summary>
        /// <returns>所有通道的MPD电流值</returns>
        private List<double> ReadMPDAllChannel()
        {
            List<double> lstMpd = new List<double>();
            try
            {
                for (int chl = 0; chl < MaxChannel; chl++)
                {

                    K7001.OpenAllRelay();
                    Thread.Sleep(200);
                    K7001.CloseRelay(lstK7001Pin[chl]);
                    Thread.Sleep(100);
                    lstMpd.Add(K2000.Fetch());
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
        /// <param name="blSelect">true:选中,false:不选</param>
        private void ControlGY7501TxDisableRadio(bool blSelect)
        {
            gY7501_I2C.RadioTx0ChlDisable.Checked = blSelect;
            gY7501_I2C.RadioTx1ChlDisable.Checked = blSelect;
            gY7501_I2C.RadioTx2ChlDisable.Checked = blSelect;
            gY7501_I2C.RadioTx3ChlDisable.Checked = blSelect;
        }
        /// <summary>
        /// 测试结束的操作，断电，关闭产品温度，环境温度设回常温，保存测试数据到数据库
        /// </summary>
        private void FinishedTest()
        {
            try
            {
                ShowMsg($"Test process ended,turn off the K2400...", true);
                //产品断电
                K2400_1.OUTPUT(false);
                K2400_2.OUTPUT(false);
                K2400_3.OUTPUT(false);
                ShowMsg($"Turn off the LDT5525 ...", true);
                //关闭产品温度
                L5525B.SetOutputStatus(false);
                ShowMsg($"Set the environment temperature to room temperature.", true);
                //设置环境温度为常温
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[0]);//测试完成，将TEC设置回常温
                this.Temp_Environment = lstTecTemp[0];
                TickCountTotal = 0;
                TecTimer.Start();

                //保存数据
                if (DialogResult.Yes == MessageBox.Show("Test done，Save the test data to Database？", "Save test data", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    var task1 = new Task(() =>
                    {
                        ShowMsg("Save the test data to Database...", true);
                        db.SaveTestData(TestDataCommon);

                        ShowMsg("Saving the test data to Database is Done！", true);
                    });
                    task1.Start();
                }

                var task = new Task(() =>
                {
                    while (!TemperatureIsTimeOut)
                    {
                        if (TemperatureIsOk)
                        {
                            strMsg = $"Test is done，the temperature reached the room temperature,pls unload the product！";
                            ShowMsg(strMsg, true);
                            MessageBox.Show(strMsg, "Test Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                    if (TemperatureIsTimeOut)
                    {
                        ShowMsg("The temperature couldn't reach the room temperature,pls take care to unload the product ！", false);
                    }
                });
                task.Start();
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
            //GY7501
            {
                TestData_Channel.Vcpa = lstVcpa[chl];
                TestData_Channel.Veq = lstVeq[chl];
                TestData_Channel.Vmod = lstVmod[chl];
                TestData_Channel.Isink = lstIsink[chl];
                TestData_Channel.Ldd = lstLdd[chl];

                //N1092D 数据读取
                kesight_N1902D.QueryMeasurementResults();
                TestData_Channel.Er = kesight_N1902D.ER;
                TestData_Channel.Mask_Margin = kesight_N1902D.MaskMargin;
                TestData_Channel.Fall_time = kesight_N1902D.FallTime;
                TestData_Channel.Rise_time = kesight_N1902D.RiseTime;
                TestData_Channel.Jitter_pp = kesight_N1902D.Jitter_pp;
                TestData_Channel.Jitter_rms = kesight_N1902D.JitterRMS;

                //AQ6370
                aQ6370.ReadTestData();
                TestData_Channel.SMSR = aQ6370.SMSR;
                TestData_Channel.Cwl = aQ6370.PeakWL;

                //PM212
                ShowMsg($"Read power with PM212 in Channel{chl + 1}", true);
                TestData_Channel.Power = pm212.ReadPower() + pm212.lstPower_Offset[chl];
            }
            catch(Exception ex)
            {
                ShowMsg($"{ex.Message}", false);
            }
        }

        /// <summary>
        /// 获取pf结果并在界面显示测试数据
        /// </summary>
        /// <param name="TestData"></param>
        /// <param name="tec_temp_out"></param>
        private void GetPfAndShowTestData(CTestData_Channel TestData, double tec_temp_out)
        {
            testResult = new string[] { tec_temp_out.ToString(), TestData.Channel.ToString(), TestData.Cwl.ToString(), TestData.SMSR.ToString(), TestData.Power.ToString(), TestData.Impd.ToString(), TestData.Idark.ToString(), TestData.Jitter_pp.ToString(), TestData.Jitter_rms.ToString(), TestData.Crossing.ToString(), TestData.Fall_time.ToString(), TestData.Rise_time.ToString(), TestData.Er.ToString(), TestData.Mask_Margin.ToString() };
            iFail = new Int32[14];

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
            }
            else
            {
                TestData.Pf = false;
                iFail[2] = 1;
            }

            ShowResult(testResult, iFail);
        }

        /// <summary>
        /// 确认界面需要输入的控件已输入值
        /// </summary>
        /// <returns></returns>
        private bool InterfaceChecked()
        {
            if (txtSN.Text != null && txtSN.Text != "" && txtOperator.Text != null && txtOperator.Text != "" && cbxPN.SelectedIndex != -1 & this.txtProductTemp_Cold.Text != null & this.txtProductTemp_Cold.Text != "" & this.txtProductTemp_Room.Text != null & this.txtProductTemp_Room.Text != "" & this.txtProductTemp_Hot.Text != null & this.txtProductTemp_Hot.Text != "")
            {
                lstTecTemp_Product = new List<double>();
                lstTecTemp_Product.Add(Convert.ToDouble(this.txtProductTemp_Room.Text));
                lstTecTemp_Product.Add(Convert.ToDouble(this.txtProductTemp_Cold.Text));
                lstTecTemp_Product.Add(Convert.ToDouble(this.txtProductTemp_Hot.Text));

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
        private bool SaveEyeImage(int temp_index, int channel)
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
                string ImageFileName = $"{ImageFilePath}\\{PN}";
                if (!Directory.Exists(ImageFileName))
                {
                    Directory.CreateDirectory(ImageFileName);
                }
                string ImageName = $"{ImageFileName}\\{TestDataCommon.SN}-{temp_note}-Ch{channel}-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.jpg";
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
                ReadTestPlanAndInitInstruments(PN);
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
          //this.lstViewTestData.Clear();
            TestDataCommon = new CTestDataCommon();

            TestDataCommon.SN = txtSN.Text;
            TestDataCommon.Operator = txtOperator.Text;
            TestDataCommon.Spec_id = TestSpec.ID;

            if (!InterfaceChecked())
            {
                return;
            }
            ShowMsg("Start Test Process...", true);
            TestDataCommon.Test_Start_Time = DateTime.Now.ToLongTimeString();

            if (!TemperatureIsOk | !TemperatureIsOk_Product)
            {
                ShowMsg("温度设定未达到常温，不能开启测试流程", false);
                return;
            }
          
            ShowMsg("Read driver data from Interface GY7501_I2C...", true);
            try
            {
                GetDriverGY7501Data();
            }
            catch(Exception ex)
            {
                ShowMsg(ex.Message, false);
                return;
            }
            try
            {
                //IProgress<int> progHandle = new Progress<int>(prog =>
                //{
                //    if (prog == 100)
                //    {
                //        EnableControls();
                //        this.btnTestProcess.Enabled = true;
                //    }
                //});

                var task = new Task(() =>
                {
                    for (int TecTempIndex = 0; TecTempIndex < 3; TecTempIndex++)
                    {
                        TestData_Temp = new CTestData_Temp();
                        TestData_Temp.Temp_out = lstTecTemp[TecTempIndex];
                        TestData_Temp.Temp_in = lstTecTemp_Product[TecTempIndex];

                        ShowMsg($"将环境温度设置为{lstTecTemp[TecTempIndex]},请稍等...", true);
                        TC720.WriteTemperature(Channel.CH1, lstTecTemp[TecTempIndex]);
                        this.Temp_Environment = lstTecTemp[TecTempIndex];
                        TickCountTotal = 0;
                        TecTimer.Start();

                        L5525B.SetTemperature(lstTecTemp_Product[TecTempIndex]);
                       // L5525B.SetOutputStatus(true);
                        this.ProductTemp = lstTecTemp_Product[TecTempIndex] + L5525B.TempOffset;
                        ShowMsg($"将产品温度设置为{this.ProductTemp}，请稍等...", true);

                        TickCountTotal_Product = 0;
                        ProductTempTimer.Start();
                        //等待环境温度和产品温度达到设定值并稳定
                        while (!TemperatureIsOk | !TemperatureIsOk_Product)
                        {
                            if(TemperatureIsTimeOut_Product|TemperatureIsTimeOut)
                            {
                                break;
                            }
                        }
                        if (TemperatureIsTimeOut_Product | TemperatureIsTimeOut)
                        {
                            ShowMsg("Time is out for setting Temperature!", false);
                            return;
                        }
                        for (int channel = 0; channel < MaxChannel; channel++)
                        {
                            TestData_Channel = new CTestData_Channel();
                            TestData_Channel.Channel = channel + 1;
                            ShowMsg($"Switch Optical Channel to channel{channel + 1}", true);
                            if (!jw8402.SetChannel(TestData_Channel.Channel))
                            {
                                ShowMsg($"Error happened when switching Optical Channel to channel{channel + 1}", false);
                            }
                            ShowMsg($" Start testing in {lstTecTemp[TecTempIndex]}℃ and channel {channel + 1}...", true);
                            ShowMsg($"Running Eye...", true);
                            kesight_N1902D.Run();
                            ShowMsg($"Set AQ6370 start and stop wavelength...", true);
                            aQ6370.SetAQ6370(lstAQ6370_StartWave[channel], lstAQ6370_StopWave[channel]);
                            ShowMsg($"AQ6370 Sweeping...", true);

                            aQ6370.StartSweep();

                            ShowMsg($"Read test data in {lstTecTemp[TecTempIndex]}℃ and channel {channel + 1}...", true);

                            GetTestData_Channel(channel);

                            ShowMsg("Save Eye Diagram...", true);
                            SaveEyeImage(TecTempIndex, channel);
                           // progHandle.Report(100);
                            //添加一条测试数据到TestData_Temp
                            TestData_Temp.lstTestData_Channel.Add(TestData_Channel);
                        }
                        ShowMsg($"Finished {MaxChannel} channels eye diagram test", true);
                        ShowMsg("Read TEC Current with LDT5525B", true);
                        TestData_Temp.Itec = L5525B.ReadCurrent();
                        TestData_Temp.Vcc1 = Convert.ToDouble(K2400_1.Vcc);
                        TestData_Temp.Vcc2 = Convert.ToDouble(K2400_2.Vcc);
                        TestData_Temp.Vcc3 = Convert.ToDouble(K2400_3.Vcc);

                        ShowMsg("Read Current from K2400", true);
                        TestData_Temp.Icc1 = K2400_1.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;
                        TestData_Temp.Icc2 = K2400_2.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;
                        TestData_Temp.Icc3 = K2400_3.GetMeasuredData(KEITHLEY2400.EnumDataStringElements.CURR).Current;

                        ShowMsg("Start MPD testing... ", true);
                        var lstMpd = ReadMPDAllChannel();
                        ShowMsg("Start Idark testing... ", true);
                        ShowMsg("Disable all GY7501 Tx Channel.", true);
                        ControlGY7501TxDisableRadio(true);
                        var lstIdark = ReadMPDAllChannel();
                        ShowMsg("Finished Idark Test,Enable all GY7501 Tx Channel.", true);
                        ControlGY7501TxDisableRadio(false);
                        //将lstIdark和lstMpd 插入当前TestData_Temp的lstTestData_Channel中，通道是一一对应的，所以可以使用索引
                        ShowMsg($"Start dealing with test data...", true);
                        for (int ch = 0; ch < MaxChannel; ch++)
                        {
                            TestData_Temp.lstTestData_Channel[ch].Idark = lstIdark[ch];
                            TestData_Temp.lstTestData_Channel[ch].Impd = lstMpd[ch];
                            //一个通道获取一次pf结果，并在界面显示一行
                            GetPfAndShowTestData(TestData_Temp.lstTestData_Channel[ch], lstTecTemp[TecTempIndex]);

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
                        TestDataCommon.pf = true & TestData_Temp.Pf;
                        //添加TestData_Temp即4条数据到TestDataCommon
                        TestDataCommon.lstTestData_Temp.Add(TestData_Temp);

                        //当前测试结果为fail，且Test plan设置fail后不继续测试，则跳出循环
                        if (TecTempIndex < lstTecTemp.Count - 1)
                        {
                            if (TestData_Temp.Pf == false && lstTecTempNote[TecTempIndex].ToUpper().Contains("FALSE"))
                            {
                                strMsg = $"{lstTecTemp[TecTempIndex]}℃下测试Fail，根据TestPlan的设置，余下的温度无需测试！";
                                ShowMsg(strMsg, true);
                                MessageBox.Show(strMsg, "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                    }
                });
                task.Start();
              //  DisableContols();
                task.Wait();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                EnableControls();
            }

            try
            {
                FinishedTest();
            }
            catch (Exception ex)
            {
                ShowMsg($"Error happened when saving test data,{ex.Message}", true);
            }
        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            ShowMsg("正在绘制眼图，请稍后...", true);
            try
            {
                var task = new Task(() =>
                {
                    kesight_N1902D.AutoScale();
                });
                task.Start();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMsg("正在绘制眼图，请稍后...", true);

                var task = new Task(() =>
                {
                    kesight_N1902D.Run();
                });
                task.Start();
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
                strMsg = $"请注意温度还未达到设定值,不能设置driver data！";
                ShowMsg(strMsg, false);
                return;
            }
            gY7501_I2C = new GY7501_I2C();
            gY7501_I2C.Show();
        }

        private void cbxChlIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!jw8402.SetChannel(this.cbxChlIndex.SelectedIndex+1))
            {
                ShowMsg($"切换光通道到CH{this.cbxChlIndex.SelectedIndex + 1}时出错", false);
            }
        }

        private void btnSetProductTemp1_Click(object sender, EventArgs e)
        {
            if(cbxPN.SelectedIndex==-1)
            {
                ShowMsg("Pls choose PN at first!", false);
                return;
            }
            this.ProductTemp = Convert.ToDouble(this.txtProductTemp_Room.Text);
            L5525B.SetTemperature(this.ProductTemp);
           // L5525B.SetOutputStatus(true);
            this.ProductTemp += L5525B.TempOffset;//界面上填入的温度是设置温度，实际达到温度为界面温度+L5525B.TempOffset

            TickCountTotal_Product = 0;
            ProductTempTimer.Start();//启动产品温度监控计时器
           
            ShowMsg($"将产品温度设置为{this.ProductTemp}℃...", true);
            //直到产品温度达到要求，开始给产品加电
            while (!TemperatureIsTimeOut_Product)
            {
                if(TemperatureIsOk_Product)
                {
                    K2400_3.OUTPUT(true);
                    K2400_1.OUTPUT(true);
                    K2400_2.OUTPUT(true);
                    ShowMsg($"产品加电已完成.", true);
                    break;
                }
            }
            if (TemperatureIsTimeOut_Product)
            {
                if (DialogResult.Yes == MessageBox.Show($"产品温度设置已经超过{L5525B.TimeOut}s，还未达到设定温度{this.ProductTemp},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    TemperatureIsOk_Product = true;
                    ShowMsg($"产品温度设置未达到设定值{this.ProductTemp}℃，但可以继续进行测试！", true);
                    ShowMsg($"给产品加电...", true);
                    K2400_3.OUTPUT(true);
                    K2400_1.OUTPUT(true);
                    K2400_2.OUTPUT(true);
                }
                else
                {
                    ShowMsg($"产品温度设置未达到设定值{this.ProductTemp}℃，不可以继续进行测试！", false);
                }
                
            }
        }

        private void btnRestTemp_Click(object sender, EventArgs e)
        {
            if (lstTecTemp != null)
            {
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[0]);//测试完成，将TEC设置回常温
                this.Temp_Environment = lstTecTemp[0];
                TickCountTotal = 0;
                TecTimer.Start();
                ShowMsg($"环境温度重置为{lstTecTemp[0]}...", true);
            }
            else
            {
                ShowMsg("请先选择PN!", false);
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
                    if (DialogResult.Yes == MessageBox.Show($"温度设置已经超过{TC720.TimeOut}s，还未达到设定温度{this.Temp_Environment},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        TemperatureIsOk = true;
                        ShowMsg($"温度设置未达到设定值{TC720.TimeOut}℃，但可以继续进行测试！", true);
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
            if (RealTimeTemperature_Product >= this.ProductTemp - L5525B.TempSpan && RealTimeTemperature_Product <= this.ProductTemp + L5525B.TempSpan)
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
                    ProductTempTimer.Stop();
                    TemperatureIsTimeOut_Product = true;
                }
            }
        }
        private void txtSN_TextChanged(object sender, EventArgs e)
        {
            this.lstViewLog.Items.Clear();
            this.lstViewTestData.Items.Clear();
        }
        #endregion
    }
}
