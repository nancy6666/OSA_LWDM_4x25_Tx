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
        

        public const int Control_Enable = 0;
        public const int Control_Disable = 1;

        public byte Control_Tx0;
        public byte Control_Tx1;
        public byte Control_Tx2;

        public byte Control_Tx3;

      //  public static byte SlaveAddr;

        GY7501_DataManagement dataManagement;

        public GY7501_I2C()
        {
            InitializeComponent();
            dataManagement = new GY7501_DataManagement();
            //if (this.txtSlaveAddress.Text != null & this.txtSlaveAddress.Text != " ")
            //{
            //    SlaveAddr = Convert.ToByte(Int32.Parse(this.txtSlaveAddress.Text, NumberStyles.HexNumber));

            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (USB_I2C_Adapter.GYI2C_Open(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
                {
                    MessageBox.Show("设备打开失败！", "打开设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //设置I2C Adapter 模式和时钟
                if (USB_I2C_Adapter.GYI2C_SetMode(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
                {
                    MessageBox.Show("设置I2C 适配器的Mode出错！", "设置设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (USB_I2C_Adapter.GYI2C_SetClk(USB_I2C_Adapter.DEV_GY7501A, 0, 100) != 1)
                {
                    MessageBox.Show("设置I2C 适配器的时钟出错！", "设置设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ReadInitalParametersFromChip();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadInitalParametersFromChip()
        {
            try
            {
                dataManagement.ReadParasFromChip();
                #region Tx0

                this.NumTx0Vcpa.Value =(decimal) dataManagement.lstGY7501_Data[0].VcpaValue;
                this.NumTx0Veq.Value = (decimal)dataManagement.lstGY7501_Data[0].VeqValue;
                this.NumTx0Mod.Value = (decimal)dataManagement.lstGY7501_Data[0].ModulationValue;
                this.NumTx0ISNK.Value = (decimal)dataManagement.lstGY7501_Data[0].IsnkValue;
                this.NumTx0Ldd.Value = (decimal)dataManagement.lstGY7501_Data[0].LDDValue;
                this.RadioTx0ChlDisable.Checked = dataManagement.lstGY7501_Data[0].ChannelDisable;
                this.Tx0VcpaEnable.Checked = dataManagement.lstGY7501_Data[0].VcpaEnable;
                this.Tx0VeqEnable.Checked = dataManagement.lstGY7501_Data[0].VeqEnable;
                this.Tx0VeqPositive.Checked = dataManagement.lstGY7501_Data[0].VeqPositive;
                #endregion

                #region Tx1

                this.NumTx1Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[1].VcpaValue;
                this.NumTx1Veq.Value = (decimal)dataManagement.lstGY7501_Data[1].VeqValue;
                this.NumTx1Mod.Value = (decimal)dataManagement.lstGY7501_Data[1].ModulationValue;
                this.NumTx1ISNK.Value = (decimal)dataManagement.lstGY7501_Data[1].IsnkValue;
                this.NumTx1Ldd.Value = (decimal)dataManagement.lstGY7501_Data[1].LDDValue;
                this.RadioTx1ChlDisable.Checked = dataManagement.lstGY7501_Data[1].ChannelDisable;
                this.Tx1VcpaEnable.Checked = dataManagement.lstGY7501_Data[1].VcpaEnable;
                this.Tx1VeqEnable.Checked = dataManagement.lstGY7501_Data[1].VeqEnable;
                this.Tx1VeqPositive.Checked = dataManagement.lstGY7501_Data[1].VeqPositive;
                #endregion


                #region Tx2

                this.NumTx2Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[2].VcpaValue;
                this.NumTx2Veq.Value = (decimal)dataManagement.lstGY7501_Data[2].VeqValue;
                this.NumTx2Mod.Value = (decimal)dataManagement.lstGY7501_Data[2].ModulationValue;
                this.NumTx2ISNK.Value = (decimal)dataManagement.lstGY7501_Data[2].IsnkValue;
                this.NumTx2Ldd.Value = (decimal)dataManagement.lstGY7501_Data[2].LDDValue;
                this.RadioTx2ChlDisable.Checked = dataManagement.lstGY7501_Data[2].ChannelDisable;
                this.Tx2VcpaEnable.Checked = dataManagement.lstGY7501_Data[2].VcpaEnable; 
                this.Tx2VeqEnable.Checked = dataManagement.lstGY7501_Data[2].VeqEnable;
                this.Tx2VeqPositive.Checked = dataManagement.lstGY7501_Data[2].VeqPositive;
                #endregion

                #region Tx3

                this.NumTx3Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[3].VcpaValue;
                this.NumTx3Veq.Value = (decimal)dataManagement.lstGY7501_Data[3].VeqValue;
                this.NumTx3Mod.Value = (decimal)dataManagement.lstGY7501_Data[3].ModulationValue;
                this.NumTx3ISNK.Value = (decimal)dataManagement.lstGY7501_Data[3].IsnkValue;
                this.NumTx3Ldd.Value = (decimal)dataManagement.lstGY7501_Data[3].LDDValue;
                this.RadioTx3ChlDisable.Checked = dataManagement.lstGY7501_Data[3].ChannelDisable;
                this.Tx3VcpaEnable.Checked = dataManagement.lstGY7501_Data[3].VcpaEnable;
                this.Tx3VeqEnable.Checked = dataManagement.lstGY7501_Data[3].VeqEnable;
                this.Tx3VeqPositive.Checked = dataManagement.lstGY7501_Data[3].VeqPositive;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReadSlave_Click(object sender, EventArgs e)
        {
            //if (this.txtSlaveAddress.Text == null | this.txtSlaveAddress.Text == "")
            //{
            //    MessageBox.Show("请输入Slave Address！", "Slave Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            ReadInitalParametersFromChip();
        }

        private void txtSlaveAddress_TextChanged(object sender, EventArgs e)
        {
           // SlaveAddr = Convert.ToByte(Int32.Parse(this.txtSlaveAddress.Text, NumberStyles.HexNumber));
        }

        private void btnReadConfigSetChip_Click(object sender, EventArgs e)
        {
            string fileFullPath = $"{ Directory.GetCurrentDirectory()}\\config\\GY7501_config.csv";
            List<string> lstConfig = new List<string>();
            try
            {
                //read values from config file
                dataManagement.ReadConfigValues(fileFullPath);

                #region Tx0

                this.NumTx0Vcpa.Value =(decimal) dataManagement.lstGY7501_Data[0].VcpaValue;
                this.Tx0VcpaEnable.Checked = dataManagement.lstGY7501_Data[0].VcpaEnable;
                this.NumTx0Veq.Value = (decimal)dataManagement.lstGY7501_Data[0].VeqValue;
                this.Tx0VeqEnable.Checked = dataManagement.lstGY7501_Data[0].VeqEnable;
                this.Tx0VeqPositive.Checked = dataManagement.lstGY7501_Data[0].VeqPositive;
                this.NumTx0ISNK.Value = (decimal)dataManagement.lstGY7501_Data[0].IsnkValue;
                this.NumTx0Mod.Value = (decimal)dataManagement.lstGY7501_Data[0].ModulationValue;
                this.NumTx0Ldd.Value = (decimal)dataManagement.lstGY7501_Data[0].LDDValue;
                this.RadioTx0ChlDisable.Checked = dataManagement.lstGY7501_Data[0].ChannelDisable;

                #endregion

                #region Tx1

                this.NumTx1Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[1].VcpaValue;
                this.Tx1VcpaEnable.Checked = dataManagement.lstGY7501_Data[1].VcpaEnable;
                this.NumTx1Veq.Value = (decimal)dataManagement.lstGY7501_Data[1].VeqValue;
                this.Tx1VeqEnable.Checked = dataManagement.lstGY7501_Data[1].VeqEnable;
                this.Tx1VeqPositive.Checked = dataManagement.lstGY7501_Data[1].VeqPositive;
                this.NumTx1ISNK.Value = (decimal)dataManagement.lstGY7501_Data[1].IsnkValue;
                this.NumTx1Mod.Value = (decimal)dataManagement.lstGY7501_Data[1].ModulationValue;
                this.NumTx1Ldd.Value = (decimal)dataManagement.lstGY7501_Data[1].LDDValue;
                this.RadioTx1ChlDisable.Checked = dataManagement.lstGY7501_Data[1].ChannelDisable;
                #endregion

                #region Tx2

                this.NumTx2Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[2].VcpaValue;
                this.Tx2VcpaEnable.Checked = dataManagement.lstGY7501_Data[2].VcpaEnable;
                this.NumTx2Veq.Value = (decimal)dataManagement.lstGY7501_Data[2].VeqValue;
                this.Tx2VeqEnable.Checked = dataManagement.lstGY7501_Data[2].VeqEnable;
                this.Tx2VeqPositive.Checked = dataManagement.lstGY7501_Data[2].VeqPositive;
                this.NumTx2ISNK.Value = (decimal)dataManagement.lstGY7501_Data[2].IsnkValue;
                this.NumTx2Mod.Value = (decimal)dataManagement.lstGY7501_Data[2].ModulationValue;
                this.NumTx2Ldd.Value = (decimal)dataManagement.lstGY7501_Data[2].LDDValue;
                this.RadioTx2ChlDisable.Checked = dataManagement.lstGY7501_Data[2].ChannelDisable;
                #endregion

                #region Tx3

                this.NumTx3Vcpa.Value = (decimal)dataManagement.lstGY7501_Data[3].VcpaValue;
                this.Tx3VcpaEnable.Checked = dataManagement.lstGY7501_Data[3].VcpaEnable;
                this.NumTx3Veq.Value = (decimal)dataManagement.lstGY7501_Data[3].VeqValue;
                this.Tx3VeqEnable.Checked = dataManagement.lstGY7501_Data[3].VeqEnable;
                this.Tx3VeqPositive.Checked = dataManagement.lstGY7501_Data[3].VeqPositive;
                this.NumTx3ISNK.Value = (decimal)dataManagement.lstGY7501_Data[3].IsnkValue;
                this.NumTx3Mod.Value = (decimal)dataManagement.lstGY7501_Data[3].ModulationValue;
                this.NumTx3Ldd.Value = (decimal)dataManagement.lstGY7501_Data[3].LDDValue;
                this.RadioTx3ChlDisable.Checked = dataManagement.lstGY7501_Data[3].ChannelDisable;
                #endregion

                //set values to chip
                dataManagement.SetValuesToChip();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

       
        private void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Data";
          
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

        private void GY7501_I2C_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (USB_I2C_Adapter.GYI2C_Close(USB_I2C_Adapter.DEV_GY7501A, 0, 0) != 1)
            //{
            //    MessageBox.Show("Close GYI2C Error！", "Close GYI2C", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
        }

       
    }
}
