using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GY7501_I2C_Control
{
   public class GY7501_DataManagement
    {
        #region Properties
        public enum EnumStartAddr
        {
           TX0_ADDR = 0x00,
           TX1_ADDR = 0x10,
           TX2_ADDR = 0x20,
           TX3_ADDR = 0x30
        }
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

        public bool VcpaEnable;
        public double VcpaValue;
        public bool VeqEnable;
        public bool VeqPositive;
        public double VeqValue;
        public double ModulationValue;
        public double IsnkValue;
        public double LDDValue;
        public bool ChannelDisable;
        public List<GY7501_DataManagement> lstGY7501_Data = new List<GY7501_DataManagement>();
        #endregion

        public void OpenGY7501()
        {
          //  GlobalVar.uSB_I2C_Adapter.OpenGY7501();
        }
        public void ReadConfigValues(string fileFullPath)
        {
            lstGY7501_Data = new List<GY7501_DataManagement>();
            if (!File.Exists(fileFullPath))
            {
                throw new Exception($"配置文件{fileFullPath}不存在!请先建立配置文件！");
            }
            using (FileStream fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    sr.ReadLine();
                    string rl;
                    string[] configPerLine;
                    for (int i = 0; i < 4; i++)
                    {
                        if ((rl = sr.ReadLine()) != null)
                        {
                            configPerLine = rl.Split(',');
                            this.VcpaValue = Convert.ToDouble(configPerLine[1]);
                            
                            this.VcpaEnable= Convert.ToBoolean(Convert.ToInt16(configPerLine[2]));
                            this.VeqValue = Convert.ToDouble(configPerLine[3]);
                            this.VeqEnable = Convert.ToBoolean(Convert.ToInt16(configPerLine[4]));
                            this.VeqPositive = Convert.ToBoolean(Convert.ToInt16(configPerLine[5]));
                            this.IsnkValue = Convert.ToDouble(configPerLine[6]);
                            this.ModulationValue = Convert.ToDouble(configPerLine[7]);
                            this.LDDValue = Convert.ToDouble(configPerLine[8]);
                            this.ChannelDisable = Convert.ToBoolean(Convert.ToInt16(configPerLine[9]));
                            lstGY7501_Data.Add(this);
                        }
                        else
                        {
                            throw new Exception("配置文件格式出错，请确认有4个通道的配置参数");
                        }
                    }
                }
            }
        }

        public void SetValuesToChip()
        {
            try
            {
                SetValuesToChipPerChl(EnumStartAddr.TX0_ADDR, lstGY7501_Data[0]);
                SetValuesToChipPerChl(EnumStartAddr.TX1_ADDR, lstGY7501_Data[1]);
                SetValuesToChipPerChl(EnumStartAddr.TX2_ADDR, lstGY7501_Data[2]);
                SetValuesToChipPerChl(EnumStartAddr.TX3_ADDR, lstGY7501_Data[3]);

            }
            catch (Exception ex)
            {

                throw new Exception($"Set values to chip error!{ex.Message}");
            }
        }

        private void SetValuesToChipPerChl(EnumStartAddr startAddr,GY7501_DataManagement gY7501_Data)
        {
            List<byte> dataBuffer;         

            dataBuffer = new List<byte>();
            //Add 地址
            dataBuffer.Add((byte)startAddr);
            byte[] valueReadFromChip = new byte[10];
            try
            {
                valueReadFromChip = GlobalVar.uSB_I2C_Adapter.ReadValue((byte)startAddr, 10);
            }
            catch (Exception ex)
            {
                throw new Exception($"Read from {startAddr.ToString()} Error,{ex.Message}");
            }
            //获取vcpa保留位的值（低二位）
            var vcpaReserved = valueReadFromChip[0] & 2;
            var value = Convert.ToByte(gY7501_Data.VcpaValue) << 2 | vcpaReserved << 1 | Convert.ToByte(gY7501_Data.VcpaEnable);
            dataBuffer.Add(Convert.ToByte(value));
            //获取veq的保留位
            var veqReserved = valueReadFromChip[1] & 4;
            value = Convert.ToByte(gY7501_Data.VeqValue) << 3 | veqReserved << 2 | Convert.ToByte(gY7501_Data.VeqPositive) << 1 | Convert.ToByte(gY7501_Data.VeqEnable);
            dataBuffer.Add(Convert.ToByte(value));
            //第三个寄存器为保留位，直接将从芯片读到的该寄存数据加到数据列表
            dataBuffer.Add(valueReadFromChip[2]);
            //控件里Mod的值，分成两个字节存到两个寄存器，高位的值存到低地址的寄存器
            byte[] byteArray = BitConverter.GetBytes((ushort)(gY7501_Data.ModulationValue));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //ISNK同Mod
            byteArray = BitConverter.GetBytes((ushort)(gY7501_Data.IsnkValue));
            dataBuffer.Add(byteArray[1]);
            dataBuffer.Add(byteArray[0]);
            //LDD
            dataBuffer.Add(Convert.ToByte(gY7501_Data.LDDValue));
            //Reserved
            dataBuffer.Add(valueReadFromChip[8]);
            //Channel control
            value = valueReadFromChip[9] & ~(1) | Convert.ToByte(gY7501_Data.ChannelDisable);
            dataBuffer.Add(Convert.ToByte(value));
            try
            {
                GlobalVar.uSB_I2C_Adapter.SetValue(dataBuffer.ToArray());
                Thread.Sleep(300);
                //byte[] readdata=GlobalVar.uSB_I2C_Adapter.ReadValue((byte)startAddr, 10);
            }
            catch (Exception ex)
            {
               throw new Exception($"Set to {startAddr.ToString()} Error,{ex.Message}");
            }
        }

        public void SaveValues(string folderPath, StringBuilder sbVlues)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd");
            string filePath = $"{folderPath}\\{fileName}.csv";
            StringBuilder sbParas = new StringBuilder();
            if (!File.Exists(filePath))
            {
                sbParas.Append("SN,");
                for (int i = 0; i < 4; i++)
                {
                    sbParas.Append($"Isnk_{i},Modulation_{i},Vcpa_{i},Veq_{i},Vcpa_Enable{i},Veq_Enable{i},Veq_Polarity_{i},LDD_{i},Tx_Disable_{1},");
                }
                sbParas.Append("\r\n");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sbParas.Append(sbVlues);
                    sw.Write(sbParas);
                }
            }
        }

        public void ReadParasFromChip()
        {
            lstGY7501_Data = new List<GY7501_DataManagement>();
            try
            {
                GY7501_DataManagement data;
                ReadParasFromChipPerChl(EnumStartAddr.TX0_ADDR, out data);
                lstGY7501_Data.Add(data);
                ReadParasFromChipPerChl(EnumStartAddr.TX1_ADDR, out data);
                lstGY7501_Data.Add(data);
                ReadParasFromChipPerChl(EnumStartAddr.TX2_ADDR, out data);
                lstGY7501_Data.Add(data);
                ReadParasFromChipPerChl(EnumStartAddr.TX3_ADDR, out data);
                lstGY7501_Data.Add(data);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error happened when reading parameters from chip!{ex.Message}") ;
            }
        }

        private void ReadParasFromChipPerChl(EnumStartAddr startAddr,out GY7501_DataManagement gY7501_Data)
        {
            var dataBuff = GlobalVar.uSB_I2C_Adapter.ReadValue((byte)startAddr, 10);
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
            this.VcpaValue = value;

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
            this.VeqValue = value;

            //Modulation
            value = (byte)dataBuff[3] << 8 & 0x03 << 8 | dataBuff[4];
            this.ModulationValue = value;

            //ISNK
            value = (byte)dataBuff[5] << 8 & 0x03 << 8 | dataBuff[6];
            this.IsnkValue = value;

            //LDD 
            value = (byte)dataBuff[7];
            this.LDDValue = value;

            //Disable/Enable
            if (bitArray.Get(72))
            {
                this.ChannelDisable = true;
            }
            else
            {
                this.ChannelDisable = false;
            }
            gY7501_Data = this;
        }

    }
}
