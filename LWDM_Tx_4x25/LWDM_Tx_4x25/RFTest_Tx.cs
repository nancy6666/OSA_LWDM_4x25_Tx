using RFTest_Tx_PAM4_50G.Instruments;
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

namespace RFTest_Tx_PAM4_50G
{
    public partial class RFTest_Tx : Form
    {
        #region Properties

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public CDatabase db = new CDatabase();
        public ConfigManagement cfg = new ConfigManagement();
        public Inst_PAM4_Bert Inst_PAM4_Bert;
        public DP832A DP832A = new DP832A();
        public Bert Bert = new Bert();
        public AAB AAB;
        public MS9740A MS9740A;
        public TC720 TC720 = new TC720();
        public Kesight_N1092D kesight_N1902D = new Kesight_N1092D();
        delegate void ThreedShowMsgDelegate(string Message, bool bPass);
        delegate void ThreedShowTempDelegate(double Temp);
        private string strMsg;
        public string PN;
        public CTestSpec TestSpec;
        public CTestData TestData;
        private List<double> lstTecTemp;
        private List<string> lstTecTempNote;
        private CTestDataCommon TestDataCommon = new CTestDataCommon();
        private int TecTempIndex = 0;
        private int TECTempCount;
        private int BertChannel;
        delegate void ThreedShowResultDelegate(string[] Message, int[] iPass);
        private CancellationTokenSource cts = null;
        private Task MonitorTask = null;

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
                            ShowMsg($"温度已经稳定为{lstTecTemp[TecTempIndex]}左右，可以进行测试!", true);
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
        #endregion

        #region Consturctor

        public RFTest_Tx()
        {
            InitializeComponent();
        }
        #endregion
        private void RFTest_Tx_Load(object sender, EventArgs e)
        {
            InitInstruments();
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
            }

            ReadRealTECTemp();
        }
        #region Public Methods

        public void ShowMsg(string msg, bool bPass)
        {
            this.Invoke(new ThreedShowMsgDelegate(MsgEvent), new object[] { msg, bPass });
        }
        public void ShowResult(string[] Msg, int[] iPass)
        {
            this.Invoke(new ThreedShowResultDelegate(setParameterDetailRaw), new object[] { Msg, iPass });//, new object[] { iPass });
        }
        public void ShowRealTemp_Case(double Temp)
        {
            this.BeginInvoke(new ThreedShowTempDelegate(ShowTemp), new object[] { Temp });
        }
        #endregion


        #region Private Methods

        private void InitInstruments()
        {
            strMsg = "正在进行设备连接操作，请稍等 . . .";
            ShowMsg(strMsg, true);
            try
            {
                if (DP832A.Init(cfg.DP832ACom))
                {
                    DP832A.SetOutput(DP832A.CHANNEL.CH3, false);
                    Thread.Sleep(100);
                    DP832A.SetOutput(DP832A.CHANNEL.CH2, false);
                    Thread.Sleep(100);
                    DP832A.SetOutput(DP832A.CHANNEL.CH1, false);
                    Thread.Sleep(100);

                    //设置过流保护
                    DP832A.SetProtection(DP832A.OPMODE.OCP, DP832A.CHANNEL.CH1, 0.4);
                    Thread.Sleep(50);
                    DP832A.SetProtection(DP832A.OPMODE.OVP, DP832A.CHANNEL.CH1, 3.5);
                    DP832A.SetProtection(DP832A.OPMODE.OCP, DP832A.CHANNEL.CH2, 1);
                    Thread.Sleep(50);
                    DP832A.SetProtection(DP832A.OPMODE.OVP, DP832A.CHANNEL.CH2, 3.5);
                }
                MS9740A = new MS9740A(cfg.MS9740AGPIB);
                Inst_PAM4_Bert = new Inst_PAM4_Bert(cfg.BertCom);

                Inst_PAM4_Bert.remoteControl();
                TC720.Init(cfg.TC720Com);

                kesight_N1902D.Init();
                ShowMsg("设备连接已完成", true);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
                return;
            }
        }

        private void LoadTestSpec(object sender, EventArgs e)
        {
            TestDataCommon = new CTestDataCommon();
            TestSpec = new CTestSpec();
            this.lstViewTestData.Items.Clear();
            this.lstViewLog.Items.Clear();
            TestDataCommon.lstTestData.Clear();
            if (this.cbxPN.SelectedIndex != -1)
            {
                PN = this.cbxPN.SelectedItem.ToString();
                TestSpec = db.GetTestSpec(PN);
                TestDataCommon.Spec_id = TestSpec.ID;
                ReadTestPlan(PN);
            }
        }

        /// <summary>
        /// Set devices according to test plan file
        /// </summary>
        /// <param name="pn"> product name,also is sheet name in excel</param>
        private void ReadTestPlan(string pn)
        {
            lstTecTemp = new List<double>();
            lstTecTempNote = new List<string>();
            strMsg = "正在获取test plan 的内容，并根据其内容进行初始设置,请稍等...";
            ShowMsg(strMsg, true);
            
            var task1 = new Task(() =>
            {
                string TestPlanFile = $"{ Directory.GetCurrentDirectory()}\\config\\PAM4 TOSA TEST PLAN.xlsx";
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
                    TestDataCommon.Ppg_data_rate = excelCell[4, 2].value;
                    TestDataCommon.Ppg_channel = Convert.ToInt32(excelCell[5, 2].value);
                    TestDataCommon.Ppg_pattern =Convert.ToString(excelCell[6, 2].value);
                    TestDataCommon.Pre_cursor = excelCell[7, 2].value;
                    TestDataCommon.Main_cursor = excelCell[8, 2].value;
                    TestDataCommon.Post_cursor = excelCell[9, 2].value;
                    TestDataCommon.Inner_1 = excelCell[10, 2].value;
                    TestDataCommon.Inner_2 = excelCell[11, 2].value;
                    //DP832A paras
                    TestDataCommon.Vcc1 = excelCell[14, 2].value;
                    TestDataCommon.Vcc2 = excelCell[15, 2].value;
                    //Driver Board paras
                    TestDataCommon.TxTEC = Convert.ToUInt16(excelCell[18, 2].value);
                    TestDataCommon.TxVB = Convert.ToUInt16(excelCell[19, 2].value);
                    TestDataCommon.TxVEA = Convert.ToUInt16(excelCell[20, 2].value);
                    TestDataCommon.TxVG = Convert.ToUInt16(excelCell[21, 2].value);
                    TestDataCommon.LDBias = Convert.ToUInt16(excelCell[22, 2].value);
                    //N1092 paras
                    kesight_N1902D.Channel = Convert.ToString(excelCell[25, 2].value);
                    kesight_N1902D.Filter_rate = Convert.ToString(excelCell[26, 2].value);
                    kesight_N1902D.Channel_bandWidth = Convert.ToString(excelCell[27, 2].value);
                    kesight_N1902D.DCA_Offset = excelCell[28, 2].value;
                    //Tec Paras
                    lstTecTemp.Add(excelCell[31, 2].value);
                    lstTecTemp.Add(excelCell[32, 2].value);
                    lstTecTemp.Add(excelCell[33, 2].value);
                    lstTecTempNote.Add(Convert.ToString(excelCell[31, 3].value));
                    lstTecTempNote.Add(Convert.ToString(excelCell[32, 3].value));
                    lstTecTempNote.Add(Convert.ToString(excelCell[33, 3].value));
                    TC720.TempSpan = excelCell[34, 2].value;
                    TC720.StablizaitonTime = Convert.ToInt32(excelCell[35, 2].value);
                    TC720.TimeOut= Convert.ToInt32(excelCell[36, 2].value);
                    TECTempCount = lstTecTemp.Count;
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
            //TEC 的初始设置
            try
            {
                TecTempIndex = 0;
                TC720.WriteTemperature(0, lstTecTemp[TecTempIndex]);
                ShowMsg($"将TEC的温度设置为{lstTecTemp[TecTempIndex]}", true);
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
                BertChannel = TestDataCommon.Ppg_channel - 1;

                Inst_PAM4_Bert.setTimebase(TestDataCommon.Ppg_data_rate);// set rate
                
                Inst_PAM4_Bert.ppgOutputControl(BertChannel, true);//Enable PPG CH1
                Inst_PAM4_Bert.setPPGSignalType(BertChannel, "PAM");//Set PPG signal as PAM for CH1
                Inst_PAM4_Bert.setPPGpatternType(BertChannel,"PBRS"); //设置patternType 为[PRBS| SSPRQ |JP03A|JP03B|SQWV|LINEAR|FIXED|CJT

                Inst_PAM4_Bert.setPPGPRBSpattern(BertChannel, TestDataCommon.Ppg_pattern);// 
                Inst_PAM4_Bert.setPpgprbsMode(BertChannel, "COMBINE");//设置信号发生器PRBS 为combine或者MSB LSB模式
                Inst_PAM4_Bert.setPretap(BertChannel, TestDataCommon.Pre_cursor);
                this.TrackBarPre_Cursor.ValueChanged -= this.TrackBarPre_Cursor_ValueChanged;
                this.TrackBarPre_Cursor.Value = Convert.ToInt32(TestDataCommon.Pre_cursor);
                this.lblPre_Cursor.Text = TestDataCommon.Pre_cursor.ToString();
                this.txtPre_Cursor.Text= TestDataCommon.Pre_cursor.ToString();
                this.TrackBarPre_Cursor.ValueChanged += this.TrackBarPre_Cursor_ValueChanged;


                Inst_PAM4_Bert.setMaintap(BertChannel, TestDataCommon.Main_cursor);
                this.TrackBarMain_Cursor.ValueChanged -= this.TrackBarMain_Cursor_ValueChanged;
                this.TrackBarMain_Cursor.Value = Convert.ToInt32(TestDataCommon.Main_cursor);
                this.lblMain_Cursor.Text = TestDataCommon.Main_cursor.ToString();
                this.txtMain_Cursor.Text = TestDataCommon.Main_cursor.ToString();
                this.TrackBarMain_Cursor.ValueChanged += this.TrackBarMain_Cursor_ValueChanged;

                Inst_PAM4_Bert.setPosttap(BertChannel, TestDataCommon.Post_cursor);
                this.TrackBarPost_Cursor.ValueChanged -= this.TrackBarPost_Cursor_ValueChanged;
                this.TrackBarPost_Cursor.Value = Convert.ToInt32(TestDataCommon.Post_cursor);
                this.lblPost_Cursor.Text = TestDataCommon.Post_cursor.ToString();
                this.txtPost_Cursor.Text = TestDataCommon.Post_cursor.ToString();
                this.TrackBarPost_Cursor.ValueChanged += this.TrackBarPost_Cursor_ValueChanged;

                Inst_PAM4_Bert.setInner1(BertChannel, TestDataCommon.Inner_1);
                this.TrackBarInner_1.ValueChanged -= this.TrackBarInner_1_ValueChanged;
                this.TrackBarInner_1.Value = Convert.ToInt32(TestDataCommon.Inner_1);
                this.lblInner_1.Text = TestDataCommon.Inner_1.ToString();
                this.txtInner_1.Text = TestDataCommon.Inner_1.ToString();
                this.TrackBarInner_1.ValueChanged += this.TrackBarInner_1_ValueChanged;

                Inst_PAM4_Bert.setInner2(BertChannel, TestDataCommon.Inner_2);
                this.TrackBarInner_2.ValueChanged -= this.TrackBarInner_2_ValueChanged;
                this.TrackBarInner_2.Value = Convert.ToInt32(TestDataCommon.Inner_2);
                this.lblInner_2.Text = TestDataCommon.Inner_2.ToString();
                this.txtInner_2.Text = TestDataCommon.Inner_2.ToString();
                this.TrackBarInner_2.ValueChanged += this.TrackBarInner_2_ValueChanged;

            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对Bert进行初始设置时出错，{ex.Message}", false);
                return;
            }

            //DP832A的初始设置
            try
            {
                if (!DP832A.SetOutput(DP832A.CHANNEL.CH1, true))
                {
                    ShowMsg(string.Format("打开通道{0}失败", DP832A.CHANNEL.CH1.ToString()), false);
                }
                DP832A.SetVoltLevel(DP832A.CHANNEL.CH1, TestDataCommon.Vcc1);

                if (!DP832A.SetOutput(DP832A.CHANNEL.CH2, true))
                {
                    ShowMsg(string.Format("打开通道{0}失败", DP832A.CHANNEL.CH2.ToString()), false);
                }
                DP832A.SetVoltLevel(DP832A.CHANNEL.CH2, TestDataCommon.Vcc2);
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对DP832进行初始设置时出错，{ex.Message}", false);
              //  return;
            }
            //Driver Board AAB的初始设置
            try
            {
                AAB = new AAB();

                AAB.SetTxTEC(TestDataCommon.TxTEC);
                this.txtTxTEC.Text = TestDataCommon.TxTEC.ToString();
                AAB.SetTxVB(TestDataCommon.TxVB);
                this.txtTxVB.Text = TestDataCommon.TxVB.ToString();
                AAB.SetTxVEA(TestDataCommon.TxVEA);
                this.txtTxVEA.Text = TestDataCommon.TxVEA.ToString();
                AAB.SetTxVG(TestDataCommon.TxVG);
                this.txtTxVG.Text = TestDataCommon.TxVG.ToString();
                AAB.SetLDiBias(TestDataCommon.LDBias);
                this.txtLDiBias.Text = TestDataCommon.LDBias.ToString();
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对AAB进行初始设置时出错，{ex.Message}", false);
                return;
            }
            //N1092A 的初始设置
            try
            {
                kesight_N1902D.SetN1092();
            }
            catch (Exception ex)
            {
                ShowMsg($"根据test plan对N1092A进行初始设置时出错，{ex.Message}", false);
            }
        }

        #region Adjust AAB & Bert paras

        private void TrackBarPre_Cursor_ValueChanged(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setPretap(BertChannel, TrackBarPre_Cursor.Value);
            string value = Inst_PAM4_Bert.queryPreTap(BertChannel);
            TestDataCommon.Pre_cursor = Convert.ToDouble(value);
            lblPre_Cursor.Text = value;
            this.txtPre_Cursor.Text = value;
        }

        private void TrackBarMain_Cursor_ValueChanged(object sender, EventArgs e)
        {
            lblMain_Cursor.Text = TrackBarMain_Cursor.Value.ToString();
            Inst_PAM4_Bert.setMaintap(BertChannel, TrackBarMain_Cursor.Value);
            string value = Inst_PAM4_Bert.queryMain_Tap(BertChannel);
            TestDataCommon.Main_cursor = Convert.ToDouble(value);
            this.txtMain_Cursor.Text = value;
        }

        private void TrackBarPost_Cursor_ValueChanged(object sender, EventArgs e)
        {
            lblPost_Cursor.Text = TrackBarPost_Cursor.Value.ToString();
            Inst_PAM4_Bert.setPosttap(BertChannel, TrackBarPost_Cursor.Value);           
            string value = Inst_PAM4_Bert.queryPostTap(BertChannel);

            TestDataCommon.Post_cursor = Convert.ToDouble(value);
            this.txtPost_Cursor.Text = value;
        }

        private void TrackBarInner_1_ValueChanged(object sender, EventArgs e)
        {
            lblInner_1.Text = TrackBarInner_1.Value.ToString();
            Inst_PAM4_Bert.setInner1(BertChannel, TrackBarInner_1.Value);
            string value = Inst_PAM4_Bert.queryInner1(BertChannel);

            TestDataCommon.Inner_1 = Convert.ToDouble(value);
            this.txtInner_1.Text = value;
        }

        private void TrackBarInner_2_ValueChanged(object sender, EventArgs e)
        {
            lblInner_2.Text = TrackBarInner_2.Value.ToString();
            Inst_PAM4_Bert.setInner2(BertChannel, TrackBarInner_2.Value);
            string value = Inst_PAM4_Bert.queryInner2(BertChannel);

            TestDataCommon.Inner_2 = Convert.ToDouble(value);
            this.txtInner_2.Text= value;
        }

       

        private void btnSetTxTEC_Click(object sender, EventArgs e)
        {
            AAB.SetTxTEC(Convert.ToUInt16(txtTxTEC.Text));
            TestDataCommon.TxTEC = Convert.ToUInt16(txtTxTEC.Text);
        }

        private void btnSetRxVPD_Click(object sender, EventArgs e)
        {
            AAB.SetRxVPD(Convert.ToUInt16(txtRxVPD.Text));
            TestDataCommon.RxVPD = Convert.ToUInt16(txtRxVPD.Text);
        }

        private void btnSetTxVB_Click(object sender, EventArgs e)
        {
            AAB.SetTxVB(Convert.ToUInt16(txtTxVB.Text));
            TestDataCommon.TxVB = Convert.ToUInt16(txtTxVB.Text);
        }

        private void btnSetTxVEA_Click(object sender, EventArgs e)
        {
            AAB.SetTxVEA(Convert.ToUInt16(txtTxVEA.Text));
            TestDataCommon.TxVEA = Convert.ToUInt16(txtTxVEA.Text);
        }

        private void btnSetTxVG_Click(object sender, EventArgs e)
        {
            AAB.SetTxVG(Convert.ToUInt16(txtTxVG.Text));
            TestDataCommon.TxVG = Convert.ToUInt16(txtTxVG.Text);
        }

        private void btnSetLDiBias_Click(object sender, EventArgs e)
        {
            AAB.SetLDiBias(Convert.ToUInt16(txtLDiBias.Text));
            TestDataCommon.LDBias = Convert.ToUInt16(txtLDiBias.Text);
        }

        private void btnSetRxVGC_Click(object sender, EventArgs e)
        {
            AAB.SetRxVGC(Convert.ToUInt16(txtRxVGC.Text));
        }

        private void BtnReadADC_Click(object sender, EventArgs e)
        {
            txtMCUTemp.Text = AAB.ReadMCUTempADC().ToString();
            txtMCUVCC.Text = AAB.ReadMCUVcc().ToString();
            txtTxBias.Text = AAB.ReadBiasADC().ToString();
            txtTxPower.Text = AAB.ReadTxPowerADC().ToString();
            txtRxPower.Text = AAB.ReadRxPowerADC().ToString();
            txtTxITEC.Text = AAB.ReadTosaITEC().ToString();
            txtTxTemp.Text = AAB.ReadTosaTemperature().ToString();
            txtTxVP.Text = AAB.ReadTxVP().ToString();
        }
        #endregion 

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
        /// <summary>
        /// 实施监控TEC温度
        /// </summary>
        private void ReadRealTECTemp()
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

        private void ShowTemp(double Temp)
        {
            this.lblRealTemp_Case.Text = RealTimeTemperature.ToString();
        }
        /// <summary>
        /// 测试结束的操作，环境温度设回常温，保存测试数据到数据库
        /// </summary>
        private void FinishedTest()
        {
            try
            {
                TecTempIndex = 0;
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[TecTempIndex]);//测试完成，将TEC设置回常温
                TickCountTotal = 0;
                TecTimer.Start();
                ShowMsg($"当前产品测试已完成,环境温度设回{lstTecTemp[TecTempIndex]}...", true);

                //保存数据
                if (DialogResult.Yes == MessageBox.Show("测试已完成，是否需要保存数据到数据库？", "保存数据", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    ShowMsg("保存数据到数据库...", true);
                    db.SaveTestData(TestDataCommon);
                    //MES 数据上传
                    SaveMesData();
                    ShowMsg("数据上传完成！", true);
                }
                //TOSA 断电
                if (!DP832A.SetOutput(DP832A.CHANNEL.CH2, false))
                {
                    ShowMsg(string.Format("关闭通道{0}失败", DP832A.CHANNEL.CH2.ToString()), false);
                }
                var task = new Task(() =>
                {
                    while (!TemperatureIsTimeOut)
                    {
                        if (TemperatureIsOk)
                        {
                            strMsg = $"测试完成，温度已经设置到{lstTecTemp[TecTempIndex]},请装卸产品！";
                            ShowMsg(strMsg, true);
                            MessageBox.Show(strMsg, "测试完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                    if (TemperatureIsTimeOut)
                    {
                        ShowMsg("温度未正常设置回常温，装卸产品请注意！", false);
                    }
                });
                task.Start();
            }
            catch(Exception ex)
            {
                ShowMsg(ex.Message,false);
            }
        }
        private void SaveMesData()
        {
            try
            {
                MES_TEST_DATA mes = new MES_TEST_DATA();
                mes.Serial_no = TestDataCommon.SN;
                mes.Test_start =DateTime.Now;
                mes.Test_time = mes.Test_start;
                mes.Operator = TestDataCommon.Operator;
                mes.Part_no = this.cbxPN.Text;
                mes.Result = TestDataCommon.Pf ? "Pass" : "Fail";
                mes.Test_no = SystemInformation.ComputerName;
                mes.Workshop_id = "OSA";
                mes.Current_station = "OSA RF Test";
                mes.IN_OUT = "OUT";
                mes.Create_by = TestDataCommon.Operator;
                mes.Create_date = DateTime.Now;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                string post_json = JsonConvert.SerializeObject(mes, Formatting.Indented, timeConverter);
                db.SaveMesData(post_json, mes.Operator);
            }
            catch (Exception ex)
            {
                throw new Exception($"上传MES数据出错，{ex.Message}");
            }
        }
        private bool SaveEyeImage()
        {
            try
            {
                string ImageFilePath = $"{Directory.GetCurrentDirectory()}\\RF Eye";
                if (!Directory.Exists(ImageFilePath))
                {
                    Directory.CreateDirectory(ImageFilePath);
                }
                string ImageFileName = $"{ImageFilePath}\\{PN}";
                if (!Directory.Exists(ImageFileName))
                {
                    Directory.CreateDirectory(ImageFileName);
                }
                string ImageName = $"{ImageFileName}\\{TestDataCommon.SN}-{lstTecTemp[TecTempIndex]}C-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.jpg";
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
            catch(Exception ex)
            {
                ShowMsg(ex.Message, false);
                return false;
            }
        }
        /// <summary>
        /// 下一个测试温度设置及测试完成的数据保存
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            TestData.Test_end_time = DateTime.Now.ToLongTimeString();
           
            //添加一条测试数据到TestDataCommon
            TestDataCommon.Pf = true & TestData.Pf;
            TestDataCommon.lstTestData.Add(TestData);

            //保存上一次的眼图图片
            SaveEyeImage();
            //当前测试结果为fail，且Test plan设置fail后不继续测试，则disable 眼图仪操作按钮，同时将TEC设置回常温
            if (TestData.Pf == false && lstTecTempNote[TecTempIndex].ToUpper().Contains("FALSE")&& TecTempIndex!=TECTempCount-1)
            {
                strMsg = $"{lstTecTemp[TecTempIndex]}℃下测试Fail，根据TestPlan的设置，余下的温度无需测试！";
                ShowMsg(strMsg, true);
                MessageBox.Show(strMsg, "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FinishedTest();
                return;
            }
            TecTempIndex++;
            if (TecTempIndex >= TECTempCount)
            {
                FinishedTest();
            }
            else
            {
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[TecTempIndex]);
                TickCountTotal = 0;

                TecTimer.Start();
                ShowMsg($"将TEC的温度设置为{lstTecTemp[TecTempIndex]}", true);
            }
            this.btnNext.Enabled = false;
        }
        private bool InterfaceChecked()
        {
            
            if (txtSN.Text != null && txtSN.Text != "" && txtOperator.Text != null && txtOperator.Text != "" && cbxPN.SelectedIndex != -1)
            {
                if (TemperatureIsOk)
                {
                    if (TecTempIndex == 0)
                    {
                        TestDataCommon.SN = txtSN.Text;
                        TestDataCommon.Operator = txtOperator.Text;
                        this.lstViewTestData.Items.Clear();
                        this.lstViewLog.Items.Clear();
                        TestDataCommon.lstTestData.Clear();
                        TestDataCommon.Test_Date = DateTime.Now.ToShortDateString();
                        if (!DP832A.GetOutputState(DP832A.CHANNEL.CH2))
                        {
                            if (!DP832A.SetOutput(DP832A.CHANNEL.CH2, true))
                            {
                                ShowMsg(string.Format("打开通道{0}失败", DP832A.CHANNEL.CH2.ToString()), false);
                            }
                            DP832A.SetVoltLevel(DP832A.CHANNEL.CH2, TestDataCommon.Vcc2);
                        }
                    }
                    return true;
                }
                else
                {
                    strMsg = $"温度未达到设定值{lstTecTemp[TecTempIndex]}，不能进行测试！";
                    ShowMsg(strMsg, false);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("请将输入信息填写完整!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            TestData = new CTestData();
            if(!InterfaceChecked())
            {
                return;
            }
            ShowMsg("正在绘制眼图，请稍后...", true);
            TestData.Test_start_time = DateTime.Now.ToShortDateString();
            try
            {

                IProgress<int> progHandle = new Progress<int>(prog =>
                {
                    if (prog == 100)
                    {
                        EnableControls();
                        this.btnNext.Enabled = true;
                    }
                });

                var task = new Task(() =>
                {
                    //Inst_PAM4_Bert.setPPGPRBSpattern(BertChannel, "13Q");// for OER,AOP,OOMA,Linearity
                    //kesight_N1902D.AutoScale();

                   // Inst_PAM4_Bert.setPPGPRBSpattern(BertChannel, "15Q");// for OER,AOP,OOMA,Linearity
                    kesight_N1902D.AutoScale();
                    kesight_N1902D.QueryMeasurementResults();
                    kesight_N1902D.GetTEDCQ();
                    GetTestData();

                    progHandle.Report(100);
                    
                });
                task.Start();
                DisableContols();
            }
            catch (Exception ex)
            {
                EnableControls();
                ShowMsg(ex.Message, false);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            TestData = new CTestData();
            if(!InterfaceChecked())
            {
                return;
            }
            try
            {
                ShowMsg("正在绘制眼图，请稍后...", true);
                TestData.Test_start_time = DateTime.Now.ToLongTimeString();
                IProgress<int> progHandle = new Progress<int>(prog =>
                {
                    if (prog == 100)
                    {
                        EnableControls();
                        this.btnNext.Enabled = true;
                    }
                });
                var task=new Task(() =>
                {
                    kesight_N1902D.Run();
                    GetTestData();
                    progHandle.Report(100);
                });
                task.Start();
                DisableContols();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }
        /// <summary>
        /// 获取一条测试数据和pf结果并在界面显示
        /// </summary>
        private void GetTestData()
        {
            //读取CWL 和SMSR
            MS9740A.ReadTestData();
            TestData.CWL = MS9740A.CWL;
            TestData.SMSR = MS9740A.SMSR;

            TestData.Temp_case = lstTecTemp[TecTempIndex];
            TestData.Io = Math.Round(Convert.ToDouble(TestDataCommon.LDBias) / 4500 * 250, 2);
            TestData.EA_Voltage = Math.Round(Convert.ToDouble(TestDataCommon.TxVEA) / 4500 * 2.5, 2);
            double a = TestDataCommon.TxTEC * 2.5 / (Math.Pow(2, 12) - 1);
            double b = 5.9 * a / (2.5 - a);
            TestData.Temp_Coc = Math.Round(1 / ((Math.Log(b / 10)) / 3900 + 1 / 298.15) - 273.15, 2);
            TestData.OuterOMA = Math.Round(kesight_N1902D.OOMA, 2);
            TestData.OuterER = Math.Round(kesight_N1902D.OuterER, 2);
            TestData.TDECQ = Math.Round(kesight_N1902D.TDEQ, 2);
            TestData.Lanch_power = Math.Round(kesight_N1902D.Lanch_power, 2);
            TestData.Linearity = Math.Round(kesight_N1902D.Linearity, 2);

            testResult = new string[] { TestData.CWL.ToString(), TestData.SMSR.ToString(), TestData.OuterOMA.ToString(), TestData.OuterER.ToString(), TestData.TDECQ.ToString(), TestData.Lanch_power.ToString(), TestData.Linearity.ToString(), TestData.Temp_case.ToString() };
            iFail = new Int32[7];
            if (TestData.CWL >= TestSpec.CWL_Min && TestData.CWL <= TestSpec.CWL_Max)
            {
                if (TestData.SMSR >= TestSpec.SMSR_Min && TestData.SMSR <= TestSpec.SMSR_Max)
                {
                    if (TestData.OuterOMA >= TestSpec.Outer_OMA_Min && TestData.SMSR <= TestSpec.Outer_OMA_Max)
                    {
                        if (TestData.OuterER >= TestSpec.Outer_ER_Min && TestData.OuterER <= TestSpec.Outer_ER_Max)
                        {
                            if (TestData.TDECQ >= TestSpec.TDECQ_Min && TestData.TDECQ <= TestSpec.TDECQ_Max)
                            {
                                if (TestData.Lanch_power >= TestSpec.Lanch_Power_Min && TestData.Lanch_power <= TestSpec.Lanch_Power_Max)
                                {
                                    if (TestData.Linearity >= TestSpec.Linearity_Min && TestData.Linearity <= TestSpec.Linearity_Max)
                                    {
                                        TestData.Pf = true;
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
                }
                else
                {
                    TestData.Pf = false;
                    iFail[1] = 1;
                }
            }
            else
            {
                TestData.Pf = false;
                iFail[0] = 1;
            }

            ShowResult(testResult, iFail);
            ShowMsg("眼图运行完毕，请确认！如果没问题，请点击Next进入下一步", true);
        }

        private void setParameterDetailRaw(string[] strResult, int[] iFail)
        {
            lstViewTestData.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                lvi = new ListViewItem(strResult);
                if (lstViewTestData.Items.Count == TecTempIndex + 1)
                {
                    lstViewTestData.Items.RemoveAt(TecTempIndex);
                }
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

        private void TECTimer_Tick(object sender, EventArgs e)
        {
            TickCountTotal++;
            if (RealTimeTemperature >= lstTecTemp[TecTempIndex] - TC720.TempSpan && RealTimeTemperature <= lstTecTemp[TecTempIndex] + TC720.TempSpan)
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
                    if (DialogResult.Yes == MessageBox.Show($"温度设置已经超过{TC720.TimeOut}s，还未达到设定温度{lstTecTemp[TecTempIndex]},是否继续测试？", "控温超时", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        TemperatureIsOk = true;
                        ShowMsg($"温度设置未达到设定值{TC720.TimeOut}℃，但可以继续进行测试！", true);
                    }
                    else
                    {
                        TickCountTotal = 0;
                        ShowMsg($"温度设置未达到设定值{TC720.TimeOut}℃，不可以继续进行测试！", false);
                     }
                }
            }
        }
        #endregion

        private void btnRestTemp_Click(object sender, EventArgs e)
        {
            if (lstTecTemp != null)
            {
                TecTempIndex = 0;
                TC720.WriteTemperature(Channel.CH1, lstTecTemp[TecTempIndex]);//测试完成，将TEC设置回常温
                TickCountTotal = 0;
                TecTimer.Start();
                ShowMsg($"环境温度重置为{lstTecTemp[TecTempIndex]}...", true);
            }
            else
            {
                ShowMsg("请先选择PN!",false);
            }
        }
      
        private void btnSetPre_Cusor_Click(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setPretap(this.BertChannel, Convert.ToDouble(this.txtPre_Cursor.Text));
            this.lblPre_Cursor.Text = this.txtPre_Cursor.Text;
            TestDataCommon.Pre_cursor = Convert.ToDouble(txtPre_Cursor.Text);
        }

        private void btnSetMain_Cusor_Click(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setMaintap(this.BertChannel, Convert.ToDouble(this.txtMain_Cursor.Text));
            this.lblMain_Cursor.Text = this.txtMain_Cursor.Text;
            TestDataCommon.Main_cursor = Convert.ToDouble(txtMain_Cursor.Text);

        }

        private void btnSetPost_Cusor_Click(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setPosttap(this.BertChannel, Convert.ToDouble(this.txtPost_Cursor.Text));
            this.lblPost_Cursor.Text = this.txtPost_Cursor.Text;
            TestDataCommon.Post_cursor = Convert.ToDouble(txtPost_Cursor.Text);
        }

        private void btnSetInner_1_Click(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setInner1(this.BertChannel, Convert.ToDouble(this.txtInner_1.Text));
            this.lblInner_1.Text = this.txtInner_1.Text;
            TestDataCommon.Inner_1 = Convert.ToDouble(txtInner_1.Text);
        }

        private void btnSetInner_2_Click(object sender, EventArgs e)
        {
            Inst_PAM4_Bert.setInner2(this.BertChannel, Convert.ToDouble(this.txtInner_2.Text));
            this.lblInner_2.Text = this.txtInner_2.Text;
            TestDataCommon.Inner_2 = Convert.ToDouble(txtInner_2.Text);

        }
        private void Deinit()
        {
            ShowMsg("断开设备连接...", true);
            Inst_PAM4_Bert.closeSerialPort();
            TC720.DeInit();
            DP832A.DeInit();
            ShowMsg("设备连接已断开...", true);

        }
        private void RFTest_Tx_FormClosed(object sender, FormClosedEventArgs e)
        {
           // Deinit();
        }

        private void btnConnetDevices_Click(object sender, EventArgs e)
        {
                InitInstruments();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Deinit();
        }
        private void DisableContols()
        {
            foreach (Control whole in Controls)
            {
                whole.Enabled = false;
                //foreach (Control c in groupBox1.Controls)   //在groupBox区域内的所有controls
                //{
                //    if ((c is TextBox) || (c is ComboBox))
                //    {
                //        c.Text = "";
                //    }
                //}

            }
        }
        private void EnableControls()
        {
            foreach(Control whole in Controls)
            {
                whole.Enabled = true;
            }
        }

    }
}
