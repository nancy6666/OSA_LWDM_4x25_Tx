using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GY7501_I2C_Control
{
    public partial class GY7501_I2C : Form
    {
        public const byte ISNK_TX0_MSB_ADDR = 0x05;
        public const byte ISNK_TX0_LSB_ADDR = 0x06;
        public const byte VEQ_TX0_ADDR = 0x01;
        public const byte VCPA_TX0_ADDR = 0x00;
        public const byte MOD_TX0_MSB_ADDR = 0x03;
        public const byte MOD_TX0_LSB_ADDR = 0x04;
        public const byte LDD_TX0_ADDR = 0x07;
        public const byte CONTROL_TX0_ADDR = 0x09;

        public const byte ISNK_TX1_MSB_ADDR = 0x15;
        public const byte ISNK_TX1_LSB_ADDR = 0x16;
        public const byte VEQ_TX1_ADDR = 0x11;
        public const byte VCPA_TX1_ADDR = 0x10;
        public const byte MOD_TX1_MSB_ADDR = 0x13;
        public const byte MOD_TX1_LSB_ADDR = 0x14;
        public const byte LDD_TX1_ADDR = 0x17;

        public const byte CONTROL_TX1_ADDR = 0x19;

        public const byte ISNK_TX2_MSB_ADDR = 0x25;
        public const byte ISNK_TX2_LSB_ADDR = 0x26;
        public const byte VEQ_TX2_ADDR = 0x21;
        public const byte VCPA_TX2_ADDR = 0x20;
        public const byte MOD_TX2_MSB_ADDR = 0x23;
        public const byte MOD_TX2_LSB_ADDR = 0x24;
        public const byte LDD_TX2_ADDR = 0x27;
        public const byte CONTROL_TX2_ADDR = 0x29;

        public const byte ISNK_TX3_MSB_ADDR = 0x35;
        public const byte ISNK_TX3_LSB_ADDR = 0x36;
        public const byte VEQ_TX3_ADDR = 0x31;
        public const byte VCPA_TX3_ADDR = 0x30;
        public const byte MOD_TX3_MSB_ADDR = 0x33;
        public const byte MOD_TX3_LSB_ADDR = 0x34;
        public const byte LDD_TX3_ADDR = 0x37;
        public const byte CONTROL_TX3_ADDR = 0x39;

        public const int Control_Enable = 0;
        public const int Control_Disable = 1;

        public byte Control_Tx0;
        public byte Control_Tx1;
        public byte Control_Tx2;

        public int UpDownLDDValue { get; private set; }

        public byte Control_Tx3;

        public static byte SlaveAddr;
        private bool VcpaEnable;
        private int upDownVcpaValue;
        private bool VeqEnable;
        private bool VeqPositive;
        private int upDownVeqValue;
        private int upDownModulationValue;
        private int upDownIsnkValue;
        private bool ChannelDisable;
        DataManagement dataManagement;

        public GY7501_I2C()
        {
            InitializeComponent();
            dataManagement = new DataManagement();
            if (this.txtSlaveAddress.Text != null & this.txtSlaveAddress.Text != " ")
            {
                SlaveAddr = Convert.ToByte(Int32.Parse(this.txtSlaveAddress.Text, NumberStyles.HexNumber));

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (USB_I2C_Adapter.GYI2C_Open(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
            //{
            //    MessageBox.Show("设备打开失败！", "打开设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            ////设置I2C Adapter 模式和时钟
            //if (USB_I2C_Adapter.GYI2C_SetMode(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
            //{
            //    MessageBox.Show("设置I2C 适配器的Mode出错！", "设置设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //if (USB_I2C_Adapter.GYI2C_SetClk(USB_I2C_Adapter.DEV_GY7501A, 0, 100) != 1)
            //{
            //    MessageBox.Show("设置I2C 适配器的时钟出错！", "设置设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }

        private void ReadInitalParametersFromChip()
        {
            if (this.txtSlaveAddress.Text == null | this.txtSlaveAddress.Text == "")
            {
                MessageBox.Show("请输入Slave Address！", "Slave Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                #region Tx0

                ReadSetParasFromChipPerChl(VCPA_TX0_ADDR);
                this.NumTx0Vcpa.Value = this.upDownVcpaValue;
                this.NumTx0Veq.Value = this.upDownVeqValue;
                this.NumTx0Mod.Value = this.upDownModulationValue;
                this.NumTx0ISNK.Value = this.upDownIsnkValue;
                this.NumTx0Ldd.Value = this.UpDownLDDValue;
                this.RadioTx0ChlDisable.Checked = this.ChannelDisable;
                this.Tx0VcpaEnable.Checked = this.VcpaEnable;
                this.Tx0VeqEnable.Checked = this.VeqEnable;
                this.Tx0VeqPositive.Checked = this.VeqPositive;
                #endregion

                #region Tx1

                ReadSetParasFromChipPerChl(VCPA_TX1_ADDR);
                this.NumTx1Vcpa.Value = this.upDownVcpaValue;
                this.NumTx1Veq.Value = this.upDownVeqValue;
                this.NumTx1Mod.Value = this.upDownModulationValue;
                this.NumTx1ISNK.Value = this.upDownIsnkValue;
                this.NumTx1Ldd.Value = this.UpDownLDDValue;
                this.RadioTx1ChlDisable.Checked = this.ChannelDisable;
                this.Tx1VcpaEnable.Checked = this.VcpaEnable;
                this.Tx1VeqEnable.Checked = this.VeqEnable;
                this.Tx1VeqPositive.Checked = this.VeqPositive;
                #endregion


                #region Tx2

                ReadSetParasFromChipPerChl(VCPA_TX2_ADDR);
                this.NumTx2Vcpa.Value = this.upDownVcpaValue;
                this.NumTx2Veq.Value = this.upDownVeqValue;
                this.NumTx2Mod.Value = this.upDownModulationValue;
                this.NumTx2ISNK.Value = this.upDownIsnkValue;
                this.NumTx2Ldd.Value = this.UpDownLDDValue;
                this.RadioTx2ChlDisable.Checked = this.ChannelDisable;
                this.Tx2VcpaEnable.Checked = this.VcpaEnable;
                this.Tx2VeqEnable.Checked = this.VeqEnable;
                this.Tx2VeqPositive.Checked = this.VeqPositive;
                #endregion

                #region Tx3

                ReadSetParasFromChipPerChl(VCPA_TX3_ADDR);
                this.NumTx3Vcpa.Value = this.upDownVcpaValue;
                this.NumTx3Veq.Value = this.upDownVeqValue;
                this.NumTx3Mod.Value = this.upDownModulationValue;
                this.NumTx3ISNK.Value = this.upDownIsnkValue;
                this.NumTx3Ldd.Value = this.UpDownLDDValue;
                this.RadioTx3ChlDisable.Checked = this.ChannelDisable;
                this.Tx3VcpaEnable.Checked = this.VcpaEnable;
                this.Tx3VeqEnable.Checked = this.VeqEnable;
                this.Tx3VeqPositive.Checked = this.VeqPositive;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadSetParasFromChipPerChl(byte regAddr)
        {
            var dataBuff = GlobalVar.uSB_I2C_Adapter.ReadValue(regAddr, 10);
            var bitArray = new BitArray(dataBuff);

            //Vcpa
            //获取最低位，为Vcpa的Enable/Disable
            if (bitArray.Get(0))
            {
                this.VcpaEnable = true;
            }
            else
            {
                this.VcpaEnable = false;
            }
            //获取高六位为value
            var value = dataBuff[0] >> 2;
            this.upDownVcpaValue = value;

            //Veq
            if (bitArray.Get(8))
            {
                this.VeqEnable = true;
            }
            else
            {
                this.VeqEnable = false;
            }
            if (bitArray.Get(9))
            {
                this.VeqPositive = true;
            }
            else
            {
                this.VeqPositive = false;
            }
            //获取高五位为Veq的value
            value = dataBuff[1] >> 3;
            this.upDownVeqValue = value;

            //Modulation
            value = (byte)dataBuff[3] & 0x03 << 8 | dataBuff[4];
            this.upDownModulationValue = value;

            //ISNK
            value = (byte)dataBuff[5] & 0x03 << 8 | dataBuff[6];
            this.upDownIsnkValue = value;

            //LDD 
            value = (byte)dataBuff[7];
            this.UpDownLDDValue = value;

            //Disable/Enable
            if (bitArray.Get(72))
            {
                this.ChannelDisable = true;
            }
            else
            {
                this.ChannelDisable = false;
            }
        }

        private void btnReadSlave_Click(object sender, EventArgs e)
        {
            ReadInitalParametersFromChip();
        }

        private void txtSlaveAddress_TextChanged(object sender, EventArgs e)
        {
            SlaveAddr = Convert.ToByte(Int32.Parse(this.txtSlaveAddress.Text, NumberStyles.HexNumber));
        }

        private void btnReadConfig_Click(object sender, EventArgs e)
        {
            string fileFullPath = $"{ Directory.GetCurrentDirectory()}\\config\\GY7501_config.csv";
            List<string> lstConfig = new List<string>();
            try
            {
                lstConfig = dataManagement.ReadConfigValues(fileFullPath);

                #region Tx0

                string[] configPerLine = lstConfig[0].Split(',');
                this.NumTx0Vcpa.Value = Convert.ToDecimal(configPerLine[1]);
                this.Tx0VcpaEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[2]));
                this.NumTx0Veq.Value = Convert.ToDecimal(configPerLine[3]);
                this.Tx0VeqEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[4]));
                this.Tx0VeqPositive.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[5]));
                this.NumTx0ISNK.Value = Convert.ToDecimal(configPerLine[6]);
                this.NumTx0Mod.Value = Convert.ToDecimal(configPerLine[7]);
                this.NumTx0Ldd.Value = Convert.ToDecimal(configPerLine[8]);
                this.RadioTx0ChlDisable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[9]));

                #endregion

                #region Tx1

                configPerLine = lstConfig[1].Split(',');
                this.NumTx1Vcpa.Value = Convert.ToDecimal(configPerLine[1]);
                this.Tx1VcpaEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[2]));
                this.NumTx1Veq.Value = Convert.ToDecimal(configPerLine[3]);
                this.Tx1VeqEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[4]));
                this.Tx1VeqPositive.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[5]));
                this.NumTx1ISNK.Value = Convert.ToDecimal(configPerLine[6]);
                this.NumTx1Mod.Value = Convert.ToDecimal(configPerLine[7]);
                this.NumTx1Ldd.Value = Convert.ToDecimal(configPerLine[8]);
                this.RadioTx1ChlDisable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[9]));
                #endregion

                #region Tx2

                configPerLine = lstConfig[2].Split(',');
                this.NumTx2Vcpa.Value = Convert.ToDecimal(configPerLine[1]);
                this.Tx2VcpaEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[2]));
                this.NumTx2Veq.Value = Convert.ToDecimal(configPerLine[3]);
                this.Tx2VeqEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[4]));
                this.Tx2VeqPositive.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[5]));
                this.NumTx2ISNK.Value = Convert.ToDecimal(configPerLine[6]);
                this.NumTx2Mod.Value = Convert.ToDecimal(configPerLine[7]);
                this.NumTx2Ldd.Value = Convert.ToDecimal(configPerLine[8]);
                this.RadioTx2ChlDisable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[9]));
                #endregion

                #region Tx3

                configPerLine = lstConfig[3].Split(',');
                this.NumTx3Vcpa.Value = Convert.ToDecimal(configPerLine[1]);
                this.Tx3VcpaEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[2]));
                this.NumTx3Veq.Value = Convert.ToDecimal(configPerLine[3]);
                this.Tx3VeqEnable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[4]));
                this.Tx3VeqPositive.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[5]));
                this.NumTx3ISNK.Value = Convert.ToDecimal(configPerLine[6]);
                this.NumTx3Mod.Value = Convert.ToDecimal(configPerLine[7]);
                this.NumTx3Ldd.Value = Convert.ToDecimal(configPerLine[8]);
                this.RadioTx3ChlDisable.Checked = Convert.ToBoolean(Convert.ToInt16(configPerLine[9]));
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// 将界面上显示的参数值设置到芯片中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetChip_Click(object sender, EventArgs e)
        {
            List<byte> dataBuffer;

            #region Tx0

            dataBuffer = new List<byte>();
            //Add 地址
            dataBuffer.Add(VCPA_TX0_ADDR);
            byte[] valueReadFromChip = new byte[10];
            try
            {
                valueReadFromChip = GlobalVar.uSB_I2C_Adapter.ReadValue(VCPA_TX0_ADDR, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Tx0 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //获取vcpa保留位的值（低二位）
            var vcpaReserved = valueReadFromChip[0] & 2;
            var value = Convert.ToByte(this.NumTx0Vcpa.Value) << 2 | vcpaReserved << 1 | Convert.ToByte(this.Tx0VcpaEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //获取veq的保留位
            var veqReserved = valueReadFromChip[1] & 4;
            value = Convert.ToByte(this.NumTx0Veq.Value) << 3 | veqReserved << 2 | Convert.ToByte(this.Tx0VeqPositive.Checked) << 1 | Convert.ToByte(this.Tx0VeqEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //第三个寄存器为保留位，直接将从芯片读到的该寄存数据加到数据列表
            dataBuffer.Add(valueReadFromChip[2]);
            //控件里Mod的值，分成两个字节存到两个寄存器，高位的值存到低地址的寄存器
            byte[] byteArray = BitConverter.GetBytes((ushort)(this.NumTx0Mod.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //ISNK同Mod
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx0ISNK.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //LDD
            dataBuffer.Add(Convert.ToByte(this.NumTx0Ldd.Value));
            //Reserved
            dataBuffer.Add(valueReadFromChip[8]);
            //Channel control
            value = valueReadFromChip[9] & ~(1) | Convert.ToByte(this.RadioTx0ChlDisable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            try
            {
                GlobalVar.uSB_I2C_Adapter.SetValue(dataBuffer.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Set Tx0 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Tx1

            dataBuffer = new List<byte>();
            dataBuffer.Add(VCPA_TX1_ADDR);
            try
            {
                valueReadFromChip = GlobalVar.uSB_I2C_Adapter.ReadValue(VCPA_TX1_ADDR, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Tx1 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //获取vcpa保留位的值（低二位）
            vcpaReserved = valueReadFromChip[0] & 2;
            value = Convert.ToByte(this.NumTx1Vcpa.Value) << 2 | vcpaReserved << 1 | Convert.ToByte(this.Tx1VcpaEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //获取veq的保留位
            veqReserved = valueReadFromChip[1] & 4;
            value = Convert.ToByte(this.NumTx1Veq.Value) << 3 | veqReserved << 2 | Convert.ToByte(this.Tx1VeqPositive.Checked) << 1 | Convert.ToByte(this.Tx1VeqEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //第三个寄存器为保留位，直接将从芯片读到的该寄存数据加到数据列表
            dataBuffer.Add(valueReadFromChip[2]);
            //控件里Mod的值，分成两个字节存到两个寄存器，高位的值存到低地址的寄存器
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx1Mod.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //ISNK同Mod
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx1ISNK.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //LDD
            dataBuffer.Add(Convert.ToByte(this.NumTx1Ldd.Value));
            //Reserved
            dataBuffer.Add(valueReadFromChip[8]);
            //Channel control
            value = valueReadFromChip[9] & ~(1) | Convert.ToByte(this.RadioTx1ChlDisable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            try
            {
                GlobalVar.uSB_I2C_Adapter.SetValue(dataBuffer.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Set Tx1 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion

            #region Tx2

            dataBuffer = new List<byte>();
            dataBuffer.Add(VCPA_TX2_ADDR);
            try
            {
                valueReadFromChip = GlobalVar.uSB_I2C_Adapter.ReadValue(VCPA_TX2_ADDR, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Tx2 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //获取vcpa保留位的值（低二位）
            vcpaReserved = valueReadFromChip[0] & 2;
            value = Convert.ToByte(this.NumTx2Vcpa.Value) << 2 | vcpaReserved << 1 | Convert.ToByte(this.Tx2VcpaEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //获取veq的保留位
            veqReserved = valueReadFromChip[1] & 4;
            value = Convert.ToByte(this.NumTx2Veq.Value) << 3 | veqReserved << 2 | Convert.ToByte(this.Tx2VeqPositive.Checked) << 1 | Convert.ToByte(this.Tx2VeqEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //第三个寄存器为保留位，直接将从芯片读到的该寄存数据加到数据列表
            dataBuffer.Add(valueReadFromChip[2]);
            //控件里Mod的值，分成两个字节存到两个寄存器，高位的值存到低地址的寄存器
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx2Mod.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //ISNK同Mod
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx2ISNK.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //LDD
            dataBuffer.Add(Convert.ToByte(this.NumTx2Ldd.Value));
            //Reserved
            dataBuffer.Add(valueReadFromChip[8]);
            //Channel control
            value = valueReadFromChip[9] & ~(1) | Convert.ToByte(this.RadioTx2ChlDisable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            try
            {
                GlobalVar.uSB_I2C_Adapter.SetValue(dataBuffer.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Set Tx2 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion

            #region Tx3

            dataBuffer = new List<byte>();
            dataBuffer.Add(VCPA_TX3_ADDR);
            try
            {
                valueReadFromChip = GlobalVar.uSB_I2C_Adapter.ReadValue(VCPA_TX3_ADDR, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Tx3 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //获取vcpa保留位的值（低二位）
            vcpaReserved = valueReadFromChip[0] & 2;
            value = Convert.ToByte(this.NumTx3Vcpa.Value) << 2 | vcpaReserved << 1 | Convert.ToByte(this.Tx3VcpaEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //获取veq的保留位
            veqReserved = valueReadFromChip[1] & 4;
            value = Convert.ToByte(this.NumTx3Veq.Value) << 3 | veqReserved << 2 | Convert.ToByte(this.Tx3VeqPositive.Checked) << 1 | Convert.ToByte(this.Tx3VeqEnable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            //第三个寄存器为保留位，直接将从芯片读到的该寄存数据加到数据列表
            dataBuffer.Add(valueReadFromChip[2]);
            //控件里Mod的值，分成两个字节存到两个寄存器，高位的值存到低地址的寄存器
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx3Mod.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //ISNK同Mod
            byteArray = BitConverter.GetBytes((ushort)(this.NumTx3ISNK.Value));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //LDD
            dataBuffer.Add(Convert.ToByte(this.NumTx3Ldd.Value));
            //Reserved
            dataBuffer.Add(valueReadFromChip[8]);
            //Channel control
            value = valueReadFromChip[9] & ~(1) | Convert.ToByte(this.RadioTx3ChlDisable.Checked);
            dataBuffer.Add(Convert.ToByte(value));
            try
            {
                GlobalVar.uSB_I2C_Adapter.SetValue(dataBuffer.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Set Tx3 Error,{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Data";
            //if (this.txtSN.Text != null & this.txtSN.Text != "")
            //{
            //    sb.Append($"{this.txtSN.Text},");
            //}
            //else
            //{
            //    MessageBox.Show("请输入SN号！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.txtSN.Focus();
            //    return;
            //}

            //将四个通道的参数放到一行
            #region Tx0

            sb.Append($"{this.NumTx0Vcpa.Value},");
            sb.Append($"{this.Tx0VcpaEnable.Checked},");
            sb.Append($"{this.NumTx0Veq.Value},");
            sb.Append($"{this.Tx0VeqEnable.Checked},");
            sb.Append($"{this.Tx0VeqPositive.Checked},");
            sb.Append($"{this.NumTx0ISNK.Value},");
            sb.Append($"{this.NumTx0Mod.Value},");
            sb.Append($"{this.NumTx0Ldd.Value},");
            sb.Append($"{this.RadioTx0ChlDisable.Checked},");

            #endregion

            #region Tx1

            sb.Append($"{this.NumTx1Vcpa.Value},");
            sb.Append($"{this.Tx1VcpaEnable.Checked},");
            sb.Append($"{this.NumTx1Veq.Value},");
            sb.Append($"{this.Tx1VeqEnable.Checked},");
            sb.Append($"{this.Tx1VeqPositive.Checked},");
            sb.Append($"{this.NumTx1ISNK.Value},");
            sb.Append($"{this.NumTx1Mod.Value},");
            sb.Append($"{this.NumTx1Ldd.Value},");
            sb.Append($"{this.RadioTx1ChlDisable.Checked},");
            #endregion

            #region Tx2

            sb.Append($"{this.NumTx2Vcpa.Value},");
            sb.Append($"{this.Tx2VcpaEnable.Checked},");
            sb.Append($"{this.NumTx2Veq.Value},");
            sb.Append($"{this.Tx2VeqEnable.Checked},");
            sb.Append($"{this.Tx2VeqPositive.Checked},");
            sb.Append($"{this.NumTx2ISNK.Value},");
            sb.Append($"{this.NumTx2Mod.Value},");
            sb.Append($"{this.NumTx2Ldd.Value},");
            sb.Append($"{this.RadioTx2ChlDisable.Checked},");
            #endregion

            #region Tx3

            sb.Append($"{this.NumTx3Vcpa.Value},");
            sb.Append($"{this.Tx3VcpaEnable.Checked},");
            sb.Append($"{this.NumTx3Veq.Value},");
            sb.Append($"{this.Tx3VeqEnable.Checked},");
            sb.Append($"{this.Tx3VeqPositive.Checked},");
            sb.Append($"{this.NumTx3ISNK.Value},");
            sb.Append($"{this.NumTx3Mod.Value},");
            sb.Append($"{this.NumTx3Ldd.Value},");
            sb.AppendLine($"{this.RadioTx3ChlDisable.Checked}");
            #endregion
            try
            {
                dataManagement.SaveValues(filePath, sb);
                MessageBox.Show("数据保存成功！", "数据保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
