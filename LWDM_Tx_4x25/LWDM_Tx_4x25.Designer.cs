namespace LWDM_Tx_4x25
{
    partial class LWDM_Tx_4x25
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lstViewLog = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAutoScale = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.N1092D = new System.Windows.Forms.GroupBox();
            this.btnStopTestProcess = new System.Windows.Forms.Button();
            this.cbxChlIndex = new System.Windows.Forms.ComboBox();
            this.btnTestProcess = new System.Windows.Forms.Button();
            this.labChlIndex = new System.Windows.Forms.Label();
            this.btnOpenGY1202Interface = new System.Windows.Forms.Button();
            this.lstViewTestData = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TecTimer = new System.Timers.Timer();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbxPN = new System.Windows.Forms.ComboBox();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.环境温度 = new System.Windows.Forms.Label();
            this.lblRealTemp_Case = new System.Windows.Forms.Label();
            this.TEC = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSetProductTemp1 = new System.Windows.Forms.Button();
            this.lblRealTemp_Product = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtProductTemp_Hot = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProductTemp_Cold = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProductTemp_Room = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRestTemp = new System.Windows.Forms.Button();
            this.ProductTempTimer = new System.Timers.Timer();
            this.N1092D.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TecTimer)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.TEC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProductTempTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // lstViewLog
            // 
            this.lstViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstViewLog.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstViewLog.GridLines = true;
            this.lstViewLog.HideSelection = false;
            this.lstViewLog.Location = new System.Drawing.Point(386, 12);
            this.lstViewLog.Name = "lstViewLog";
            this.lstViewLog.Size = new System.Drawing.Size(577, 512);
            this.lstViewLog.TabIndex = 7;
            this.lstViewLog.UseCompatibleStateImageBehavior = false;
            this.lstViewLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Information";
            this.columnHeader1.Width = 1000;
            // 
            // btnAutoScale
            // 
            this.btnAutoScale.Location = new System.Drawing.Point(44, 79);
            this.btnAutoScale.Name = "btnAutoScale";
            this.btnAutoScale.Size = new System.Drawing.Size(93, 39);
            this.btnAutoScale.TabIndex = 10;
            this.btnAutoScale.Text = "Auto Scale";
            this.btnAutoScale.UseVisualStyleBackColor = true;
            this.btnAutoScale.Click += new System.EventHandler(this.btnAutoScale_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(187, 79);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(101, 39);
            this.btnRun.TabIndex = 11;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // N1092D
            // 
            this.N1092D.Controls.Add(this.btnStopTestProcess);
            this.N1092D.Controls.Add(this.cbxChlIndex);
            this.N1092D.Controls.Add(this.btnTestProcess);
            this.N1092D.Controls.Add(this.labChlIndex);
            this.N1092D.Controls.Add(this.btnRun);
            this.N1092D.Controls.Add(this.btnAutoScale);
            this.N1092D.Controls.Add(this.btnOpenGY1202Interface);
            this.N1092D.Location = new System.Drawing.Point(32, 322);
            this.N1092D.Name = "N1092D";
            this.N1092D.Size = new System.Drawing.Size(311, 202);
            this.N1092D.TabIndex = 12;
            this.N1092D.TabStop = false;
            this.N1092D.Text = "眼图调试";
            // 
            // btnStopTestProcess
            // 
            this.btnStopTestProcess.Location = new System.Drawing.Point(187, 136);
            this.btnStopTestProcess.Name = "btnStopTestProcess";
            this.btnStopTestProcess.Size = new System.Drawing.Size(93, 43);
            this.btnStopTestProcess.TabIndex = 20;
            this.btnStopTestProcess.Text = "停止测试";
            this.btnStopTestProcess.UseVisualStyleBackColor = true;
            this.btnStopTestProcess.Click += new System.EventHandler(this.btnStopTestProcess_Click);
            // 
            // cbxChlIndex
            // 
            this.cbxChlIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChlIndex.FormattingEnabled = true;
            this.cbxChlIndex.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.cbxChlIndex.Location = new System.Drawing.Point(46, 39);
            this.cbxChlIndex.Name = "cbxChlIndex";
            this.cbxChlIndex.Size = new System.Drawing.Size(90, 20);
            this.cbxChlIndex.TabIndex = 19;
            this.cbxChlIndex.SelectedIndexChanged += new System.EventHandler(this.cbxChlIndex_SelectedIndexChanged);
            // 
            // btnTestProcess
            // 
            this.btnTestProcess.Location = new System.Drawing.Point(44, 136);
            this.btnTestProcess.Name = "btnTestProcess";
            this.btnTestProcess.Size = new System.Drawing.Size(93, 43);
            this.btnTestProcess.TabIndex = 12;
            this.btnTestProcess.Text = "开启测试流程";
            this.btnTestProcess.UseVisualStyleBackColor = true;
            this.btnTestProcess.Click += new System.EventHandler(this.btnTestProcess_Click);
            // 
            // labChlIndex
            // 
            this.labChlIndex.AutoSize = true;
            this.labChlIndex.Location = new System.Drawing.Point(11, 42);
            this.labChlIndex.Name = "labChlIndex";
            this.labChlIndex.Size = new System.Drawing.Size(41, 12);
            this.labChlIndex.TabIndex = 14;
            this.labChlIndex.Text = "通道：";
            // 
            // btnOpenGY1202Interface
            // 
            this.btnOpenGY1202Interface.Location = new System.Drawing.Point(187, 21);
            this.btnOpenGY1202Interface.Name = "btnOpenGY1202Interface";
            this.btnOpenGY1202Interface.Size = new System.Drawing.Size(101, 38);
            this.btnOpenGY1202Interface.TabIndex = 13;
            this.btnOpenGY1202Interface.Text = "打开调试界面";
            this.btnOpenGY1202Interface.UseVisualStyleBackColor = true;
            this.btnOpenGY1202Interface.Click += new System.EventHandler(this.btnOpenGY1202Interface_Click);
            // 
            // lstViewTestData
            // 
            this.lstViewTestData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader10,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15});
            this.lstViewTestData.GridLines = true;
            this.lstViewTestData.HideSelection = false;
            this.lstViewTestData.Location = new System.Drawing.Point(32, 550);
            this.lstViewTestData.Name = "lstViewTestData";
            this.lstViewTestData.Size = new System.Drawing.Size(931, 373);
            this.lstViewTestData.TabIndex = 13;
            this.lstViewTestData.UseCompatibleStateImageBehavior = false;
            this.lstViewTestData.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Tec_Temp";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 72;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Channel";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "CWL";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "SMSR";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "POWER";
            this.columnHeader2.Width = 56;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "IMPD";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 77;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "IDARK";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 66;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "JITTER_PP";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 86;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "JITTER_RMS";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 96;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "CROSSING";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "FALL_TIME";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "RISE_TIME";
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "ER";
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "MASK_MARGIN";
            // 
            // TecTimer
            // 
            this.TecTimer.Interval = 1000D;
            this.TecTimer.SynchronizingObject = this;
            this.TecTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.TECTimer_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbxPN);
            this.groupBox6.Controls.Add(this.txtOperator);
            this.groupBox6.Controls.Add(this.txtSN);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Location = new System.Drawing.Point(32, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(311, 92);
            this.groupBox6.TabIndex = 14;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "测试信息";
            // 
            // cbxPN
            // 
            this.cbxPN.FormattingEnabled = true;
            this.cbxPN.Location = new System.Drawing.Point(199, 49);
            this.cbxPN.Name = "cbxPN";
            this.cbxPN.Size = new System.Drawing.Size(107, 20);
            this.cbxPN.TabIndex = 5;
            this.cbxPN.SelectedIndexChanged += new System.EventHandler(this.LoadTestSpec);
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(109, 48);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(76, 21);
            this.txtOperator.TabIndex = 4;
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(7, 49);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(88, 21);
            this.txtSN.TabIndex = 3;
            this.txtSN.TextChanged += new System.EventHandler(this.txtSN_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(245, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "PN";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Operator";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "SN";
            // 
            // 环境温度
            // 
            this.环境温度.AutoSize = true;
            this.环境温度.Location = new System.Drawing.Point(11, 35);
            this.环境温度.Name = "环境温度";
            this.环境温度.Size = new System.Drawing.Size(65, 12);
            this.环境温度.TabIndex = 16;
            this.环境温度.Text = "环境温度：";
            // 
            // lblRealTemp_Case
            // 
            this.lblRealTemp_Case.AutoSize = true;
            this.lblRealTemp_Case.Location = new System.Drawing.Point(96, 35);
            this.lblRealTemp_Case.Name = "lblRealTemp_Case";
            this.lblRealTemp_Case.Size = new System.Drawing.Size(41, 12);
            this.lblRealTemp_Case.TabIndex = 17;
            this.lblRealTemp_Case.Text = "label5";
            // 
            // TEC
            // 
            this.TEC.Controls.Add(this.label12);
            this.TEC.Controls.Add(this.label11);
            this.TEC.Controls.Add(this.label10);
            this.TEC.Controls.Add(this.btnSetProductTemp1);
            this.TEC.Controls.Add(this.lblRealTemp_Product);
            this.TEC.Controls.Add(this.label9);
            this.TEC.Controls.Add(this.label8);
            this.TEC.Controls.Add(this.label7);
            this.TEC.Controls.Add(this.txtProductTemp_Hot);
            this.TEC.Controls.Add(this.label6);
            this.TEC.Controls.Add(this.txtProductTemp_Cold);
            this.TEC.Controls.Add(this.label5);
            this.TEC.Controls.Add(this.txtProductTemp_Room);
            this.TEC.Controls.Add(this.label4);
            this.TEC.Controls.Add(this.lblRealTemp_Case);
            this.TEC.Controls.Add(this.环境温度);
            this.TEC.Controls.Add(this.btnRestTemp);
            this.TEC.Location = new System.Drawing.Point(32, 124);
            this.TEC.Name = "TEC";
            this.TEC.Size = new System.Drawing.Size(311, 174);
            this.TEC.TabIndex = 18;
            this.TEC.TabStop = false;
            this.TEC.Text = "TEC";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(176, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 31;
            this.label12.Text = "产品温度：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(289, 127);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 30;
            this.label11.Text = "℃";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(143, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 29;
            this.label10.Text = "℃";
            // 
            // btnSetProductTemp1
            // 
            this.btnSetProductTemp1.Location = new System.Drawing.Point(187, 76);
            this.btnSetProductTemp1.Name = "btnSetProductTemp1";
            this.btnSetProductTemp1.Size = new System.Drawing.Size(101, 33);
            this.btnSetProductTemp1.TabIndex = 28;
            this.btnSetProductTemp1.Text = "设置产品常温";
            this.btnSetProductTemp1.UseVisualStyleBackColor = true;
            this.btnSetProductTemp1.Click += new System.EventHandler(this.btnSetProductTemp1_Click);
            // 
            // lblRealTemp_Product
            // 
            this.lblRealTemp_Product.AutoSize = true;
            this.lblRealTemp_Product.Location = new System.Drawing.Point(241, 127);
            this.lblRealTemp_Product.Name = "lblRealTemp_Product";
            this.lblRealTemp_Product.Size = new System.Drawing.Size(47, 12);
            this.lblRealTemp_Product.TabIndex = 27;
            this.lblRealTemp_Product.Text = "label10";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(147, 141);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "℃";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(147, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "℃";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "℃";
            // 
            // txtProductTemp_Hot
            // 
            this.txtProductTemp_Hot.Location = new System.Drawing.Point(76, 138);
            this.txtProductTemp_Hot.Name = "txtProductTemp_Hot";
            this.txtProductTemp_Hot.Size = new System.Drawing.Size(64, 21);
            this.txtProductTemp_Hot.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "产品高温：";
            // 
            // txtProductTemp_Cold
            // 
            this.txtProductTemp_Cold.Location = new System.Drawing.Point(77, 108);
            this.txtProductTemp_Cold.Name = "txtProductTemp_Cold";
            this.txtProductTemp_Cold.Size = new System.Drawing.Size(64, 21);
            this.txtProductTemp_Cold.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "产品低温：";
            // 
            // txtProductTemp_Room
            // 
            this.txtProductTemp_Room.Location = new System.Drawing.Point(77, 76);
            this.txtProductTemp_Room.Name = "txtProductTemp_Room";
            this.txtProductTemp_Room.Size = new System.Drawing.Size(64, 21);
            this.txtProductTemp_Room.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "产品常温：";
            // 
            // btnRestTemp
            // 
            this.btnRestTemp.Location = new System.Drawing.Point(187, 23);
            this.btnRestTemp.Name = "btnRestTemp";
            this.btnRestTemp.Size = new System.Drawing.Size(101, 37);
            this.btnRestTemp.TabIndex = 15;
            this.btnRestTemp.Text = "重置环境温度";
            this.btnRestTemp.UseVisualStyleBackColor = true;
            this.btnRestTemp.Click += new System.EventHandler(this.btnRestTemp_Click);
            // 
            // ProductTempTimer
            // 
            this.ProductTempTimer.Interval = 1000D;
            this.ProductTempTimer.SynchronizingObject = this;
            this.ProductTempTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.ProductTempTimer_Tick);
            // 
            // LWDM_Tx_4x25
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 881);
            this.Controls.Add(this.TEC);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.N1092D);
            this.Controls.Add(this.lstViewTestData);
            this.Controls.Add(this.lstViewLog);
            this.MaximizeBox = false;
            this.Name = "LWDM_Tx_4x25";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LWDM Tx 4x25";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LWDM_Tx_FormClosed);
            this.Load += new System.EventHandler(this.LWDM_Tx_Load);
            this.N1092D.ResumeLayout(false);
            this.N1092D.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TecTimer)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.TEC.ResumeLayout(false);
            this.TEC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProductTempTimer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lstViewLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnAutoScale;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox N1092D;
        private System.Windows.Forms.Button btnTestProcess;
        private System.Windows.Forms.ListView lstViewTestData;
        private System.Timers.Timer TecTimer;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cbxPN;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label 环境温度;
        private System.Windows.Forms.Label lblRealTemp_Case;
        private System.Windows.Forms.GroupBox TEC;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.TextBox txtProductTemp_Room;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labChlIndex;
        private System.Windows.Forms.Button btnOpenGY1202Interface;
        private System.Windows.Forms.ComboBox cbxChlIndex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtProductTemp_Hot;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProductTemp_Cold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblRealTemp_Product;
        private System.Windows.Forms.Button btnSetProductTemp1;
        private System.Windows.Forms.Button btnRestTemp;
        private  System.Timers.Timer ProductTempTimer;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnStopTestProcess;
    }
}

