using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CyUSB;
using System.Collections;

namespace RFTest_Tx_PAM4_50G.Instruments
{
    public class AAB
    {
        private int iSleeptime = 50;
        private int iSleeptime1 = 30;
        private bool bEngMode = false;
        
        private byte SlaveAddModule = 0x50;
        USBDeviceList usbDevices;
        CyUSBDevice curCyUsbDev;
        UInt16 iDataNum = 5;
        int iUSBNum;
        public AAB()
        {
            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);

            if (usbDevices.Count < 1)
            {
                throw new Exception("Please Connect AAB USB !");
            }
            else
            {
                byte USBSearchLimit = 4;

                iUSBNum = 0;
                do
                {
                    curCyUsbDev = usbDevices[iUSBNum] as CyUSBDevice;
                    iUSBNum++;
                }
                while ((curCyUsbDev.ProductID != 0x00F3) && (iUSBNum < USBSearchLimit));    // product ID programmed into USB port
            }
            string sMOde = GetEngMode();
            if (sMOde.ToLower().Contains("eng"))
                bEngMode = true;
            else
                //bOpen = bResult;
                bEngMode = false;
        }
        public Boolean ClassUSBFound()
        {
            //usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);
            bool IsUSBFound = false;
            byte USBSearchLimit = 4;
            if (iUSBNum < USBSearchLimit) IsUSBFound = true;
            return (IsUSBFound);
        }

        public Boolean ClassUSBTX(byte[] buffer)
        {
            CyUSBEndPoint bulkEpt = curCyUsbDev.EndPointOf(0x02);
            bool TXferCompleted = false;
            bool IsPkt = false;
            int bytes = buffer.Length;      //5 byte USB-MDIO protocol

            if (bulkEpt != null)
            {
                TXferCompleted = bulkEpt.XferData(ref buffer, ref bytes, IsPkt);
            }
            return (TXferCompleted);
        }
        public Boolean ClassUSBTXMult(byte[] buffer)
        {
            CyUSBEndPoint bulkEpt = curCyUsbDev.EndPointOf(0x02);
            bool TXferCompleted = false;
            bool IsPkt = false;
            int bytes = buffer.Length;      //5 byte USB-MDIO protocol

            if (bulkEpt != null)
            {
                TXferCompleted = bulkEpt.XferData(ref buffer, ref bytes, IsPkt);
            }
            return (TXferCompleted);
        }

        public Boolean ClassUSBRX(ref Byte[] buffer, int bufferSize)
        {
            CyUSBEndPoint bulkEpt = curCyUsbDev.EndPointOf(0x81);
            bool RXferCompleted = false;
            bool IsPkt = false;
            int bytes = bufferSize;

            if (bulkEpt != null)
            {
                RXferCompleted = bulkEpt.XferData(ref buffer, ref bytes, IsPkt);
            }
            return (RXferCompleted);
        }

        /* Close USB inter-connection while close the window */
        public void USBPort_Close()
        {
            usbDevices.Dispose();
        }
        public byte set_bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        public bool Get_Bit(byte byte1, int index)
        {
            bool bResult = false;
            if (index > 7 || index < 0)
                throw new ArgumentOutOfRangeException();
            if (index == 7)
                bResult = (byte1 & 128) == 128 ? true : false;
            if (index == 6)
                bResult = (byte1 & 64) == 64 ? true : false;
            if (index == 5)
                bResult = (byte1 & 32) == 32 ? true : false;
            if (index == 4)
                bResult = (byte1 & 16) == 16 ? true : false;
            if (index == 3)
                bResult = (byte1 & 8) == 8 ? true : false;
            if (index == 2)
                bResult = (byte1 & 4) == 4 ? true : false;
            if (index == 1)
                bResult = (byte1 & 2) == 2 ? true : false;
            if (index == 0)
                bResult = (byte1 & 1) == 1 ? true : false;
            return bResult;
        }
        public bool GetBit(UInt16 byte1, int index)
        {
            bool bResult = false;
            if (index > 15 || index < 0)
                throw new ArgumentOutOfRangeException();

            if (index == 15)
                bResult = (byte1 & 32768) == 32768 ? true : false;
            if (index == 14)
                bResult = (byte1 & 16384) == 16384 ? true : false;
            if (index == 13)
                bResult = (byte1 & 8192) == 8192 ? true : false;
            if (index == 12)
                bResult = (byte1 & 4096) == 4096 ? true : false;
            if (index == 11)
                bResult = (byte1 & 2048) == 2048 ? true : false;
            if (index == 10)
                bResult = (byte1 & 1024) == 1024 ? true : false;
            if (index == 9)
                bResult = (byte1 & 512) == 512 ? true : false;
            if (index == 8)
                bResult = (byte1 & 256) == 256 ? true : false;
            if (index == 7)
                bResult = (byte1 & 128) == 128 ? true : false;
            if (index == 6)
                bResult = (byte1 & 64) == 64 ? true : false;
            if (index == 5)
                bResult = (byte1 & 32) == 32 ? true : false;
            if (index == 4)
                bResult = (byte1 & 16) == 16 ? true : false;
            if (index == 3)
                bResult = (byte1 & 8) == 8 ? true : false;
            if (index == 2)
                bResult = (byte1 & 4) == 4 ? true : false;
            if (index == 1)
                bResult = (byte1 & 2) == 2 ? true : false;
            if (index == 0)
                bResult = (byte1 & 1) == 1 ? true : false;
            return bResult;
        }
        public byte I2CMasterSingleRead(byte SlaveAdd, byte Add)
        {
            Byte[] WriteData = new Byte[iDataNum];
            int MaxRbuffersize = 1; // 0x40;
            Byte[] ReadData = new Byte[MaxRbuffersize];

            WriteData[0] = 0x52;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
            WriteData[3] = 0x0;
        
            if (ClassUSBTX(WriteData))
            {
                if (!ClassUSBRX(ref ReadData, MaxRbuffersize))
                    throw new Exception("USB RX Failed !");
            }
            else
                throw new Exception("USB TX Failed !");
            byte temp = ReadData[0];
            return temp;
        }

        public byte[] I2CMasterMultRead(byte SlaveAdd, byte Add, int iLen)
        {
            Byte[] WriteData = new Byte[iDataNum];
            byte length = Convert.ToByte(iLen.ToString());
            int MaxRbuffersize = iLen; // 0x40;
            Byte[] ReadData = new Byte[iLen];

            WriteData[0] = 0x50;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
            WriteData[3] = length;
            
            if (ClassUSBTX(WriteData))
            {
                if (!ClassUSBRX(ref ReadData, MaxRbuffersize))
                    throw new Exception("USB RX Failed !");
            }
            else
                throw new Exception("USB TX Failed !");
            byte[] temp = ReadData;
            return temp;
        }
      
        public byte I2CMasterSingleReadModule(byte SlaveAdd, byte Add)
        {
            Byte[] WriteData = new Byte[iDataNum];
            int MaxRbuffersize = 1; // 0x40;
            Byte[] ReadData = new Byte[MaxRbuffersize];

            WriteData[0] = 0x72;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
           
            if (ClassUSBTX(WriteData))
            {
                if (!ClassUSBRX(ref ReadData, MaxRbuffersize))
                    throw new Exception("USB RX Failed !");
            }
            else
                throw new Exception("USB TX Failed !");
            byte temp = ReadData[0];
            return temp;
        }

        public byte[] I2CMasterMultReadModule(byte SlaveAdd, byte Add, int iLen)
        {
            Byte[] WriteData = new Byte[iDataNum];
            byte length = Convert.ToByte(iLen.ToString());
            int MaxRbuffersize = iLen; // 0x40;
            Byte[] ReadData = new Byte[iLen];

            WriteData[0] = 0x70;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
            WriteData[3] = length;
          
            if (ClassUSBTX(WriteData))
            {
                if (!ClassUSBRX(ref ReadData, MaxRbuffersize))
                    throw new Exception("USB RX Failed !");
            }
            else
                throw new Exception("USB TX Failed !");
            byte[] temp = ReadData;
            return temp;
        }

        public bool I2CMasterSingleWriteModule(byte SlaveAdd, byte Add, byte Data)
        {
            bool bSuccess = false;
            Byte[] WriteData = new Byte[iDataNum];

            WriteData[0] = 0x77;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
            WriteData[3] = Data;

            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
         
        }
        public bool I2CMasterSingleWriteModule(byte SlaveAdd, byte Data)
        {
            bool bSuccess = false;
            Byte[] WriteData = new Byte[3];

            WriteData[0] = 0x78;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Data;
          
            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
         
        }
        public bool I2CMasterMultWriteModule(byte SlaveAdd, byte Add, byte[] Data)
        {
            bool bSuccess = false;
            int iLen = Data.Length;
            Byte[] WriteData = new Byte[iLen + 3];

            WriteData[0] = 0x6D;
            WriteData[1] = SlaveAdd;//slave address
            WriteData[2] = Add;
            for (int i = 0; i < iLen; i++)
            {
                WriteData[3 + i] = Data[i];
            }
        
            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
         
        }
        public bool I2CMasterMultWriteModule(byte SlaveAdd, byte[] Data)
        {
            bool bSuccess = false;
            int iLen = Data.Length;
            Byte[] WriteData = new Byte[iLen + 2];

            WriteData[0] = 0x78;
            WriteData[1] = SlaveAdd;//slave address
            for (int i = 0; i < iLen; i++)
            {
                WriteData[2 + i] = Data[i];
            }
    
            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
         
        }
        public bool I2CMasterSingleWriteModule()
        {
            bool bSuccess = false;
            Byte[] WriteData = new Byte[1];

            WriteData[0] = 0x62;
         
            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
           
        }

        public bool EnterLPModeHardware(bool bLP)
        {
            bool bSuccess = false;
            Byte[] WriteData = new Byte[1];
            WriteData[0] = 0x6C;
            if (bLP)
                WriteData[1] = 0x01;//slave address
            else
                WriteData[1] = 0x00;//slave address
         
            if (ClassUSBTX(WriteData))
                bSuccess = true;
            else
                bSuccess = false;
            return bSuccess;
           
        }

        public void SelectPage(byte PageSelect)
        {
            I2CMasterSingleWriteModule(SlaveAddModule, 0x7F, PageSelect);
        }
        public void SelectPage(byte SlaveAdd, byte PageSelect)
        {
            I2CMasterSingleWriteModule(SlaveAdd, 0x7F, PageSelect);
            System.Threading.Thread.Sleep(100);
        }
      
        public bool EnterEngMod()
        {
            bool bResult = false;
            if (!bEngMode)
            {
                //bResult = ExitEngMod();
                //byte[] array = System.Text.Encoding.ASCII.GetBytes("RIXI");
                I2CMasterSingleWriteModule(SlaveAddModule, 0x7B, 0x49);
                System.Threading.Thread.Sleep(iSleeptime);

                I2CMasterSingleWriteModule(SlaveAddModule, 0x7C, 0x52);
                System.Threading.Thread.Sleep(iSleeptime);

                I2CMasterSingleWriteModule(SlaveAddModule, 0x7D, 0x58);
                System.Threading.Thread.Sleep(iSleeptime);

                bResult = I2CMasterSingleWriteModule(SlaveAddModule, 0x7E, 0x49);
                //SelectPage(0x01);
                System.Threading.Thread.Sleep(iSleeptime);

                if (!bResult)
                    throw new Exception("Enter EngMod Error !");
                string sMOde = GetEngMode();
                if (sMOde.ToLower().Contains("eng"))
                    bEngMode = true;
                else
                {
                    throw new Exception("Enter EngMod Error!");
                    bEngMode = false;
                }
                //bEngMode = bResult;
            }
            return bResult;
        }

        public bool ExitEngMod()
        {
            bool bResult = false;
            int iCount = 0;
            if (bEngMode)
            {
                do
                {
                    //System.Threading.Thread.Sleep(iSleeptime * 2);

                    I2CMasterSingleWriteModule(SlaveAddModule, 0x7B, 0x00);
                    I2CMasterSingleWriteModule(SlaveAddModule, 0x7C, 0x00);
                    I2CMasterSingleWriteModule(SlaveAddModule, 0x7D, 0x00);
                    bResult = I2CMasterSingleWriteModule(SlaveAddModule, 0x7E, 0x00);
                    System.Threading.Thread.Sleep(iSleeptime * 2);

                    string sMOde = GetEngMode();
                    if (sMOde.ToLower().Contains("eng"))
                    {
                        bEngMode = true;
                        throw new Exception("Exit EngMod Error!");
                    }
                    else
                    {
                        bEngMode = false;
                        break;
                    }

                    iCount++;
                } while (!bResult && iCount < 5);
                //bEngMode = !bResult;
                if (!bResult)
                    throw new Exception("Exit EngMod Error !");
            }
            return bResult;
        }

        public string GetEngMode()
        {
            string sType = "";
            SelectPage(0x80);
            byte Result = I2CMasterSingleReadModule(SlaveAddModule, 0x8C);
            if (Result == 0x03)
                sType = "ENG Mod";
            else if (Result == 0x00)
                sType = "Normal Mod";
            else if (Result == 0x02)
                sType = "Customer Mod";
            return sType;
        }

        public bool CheckReady()
        {
            bool bReady = false;
            double totalTimeFind = 0;
            DateTime startTime = System.DateTime.Now;
            byte Data = 1;
            while (true)
            {
                Data = I2CMasterSingleReadModule(SlaveAddModule, 0x85);
                totalTimeFind = (System.DateTime.Now - startTime).TotalMilliseconds;
                if (Data == 0)
                {
                    bReady = true;
                    break;
                }
                if (totalTimeFind > 2000)
                    break;
            }
            return bReady;
        }
        public UInt16 Read_ADC(byte ID)
        {
            EnterEngMod();
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, ID);
            System.Threading.Thread.Sleep(iSleeptime * 2);
            bool bReady = CheckReady();
            if (!bReady)
                throw new Exception("0x85 not ready when Read ADC");
            byte DataHigh = I2CMasterSingleReadModule(SlaveAddModule, 0x86);
            System.Threading.Thread.Sleep(iSleeptime1);
            byte DataLow = I2CMasterSingleReadModule(SlaveAddModule, 0x87);

            UInt16 Result = UInt16.Parse((DataHigh * 256 + DataLow).ToString());

            return Result;
        }
        public UInt16 ReadMCUTempADC()
        {
            UInt16 Result = Read_ADC(0x03);
            return Result;
        }
        public UInt16 ReadMCUVcc()
        {
            UInt16 Result = Read_ADC(0x04);
            return Result;
        }
        public UInt16 ReadBiasADC()
        {
            UInt16 Result = Read_ADC(0x05);
            return Result;
        }
        public UInt16 ReadTxPowerADC()
        {
            UInt16 Result = Read_ADC(0x06);
            return Result;
        }
        public UInt16 ReadRxPowerADC()
        {
            UInt16 Result = Read_ADC(0x07);
            return Result;
        }

        public UInt16 ReadTosaITEC()
        {
            UInt16 Result = Read_ADC(0x08);
            return Result;
        }

        public UInt16 ReadTosaTemperature()
        {
            UInt16 Result = Read_ADC(0x09);
            return Result;
        }

        public UInt16 ReadTxVP()
        {
            UInt16 Result = Read_ADC(0x0A);
            return Result;
        }

        public UInt16 Read_IN015050(UInt32 Add)
        {
            EnterEngMod();
            SelectPage(0x80);
            byte[] AddValue = System.BitConverter.GetBytes(Add);
            UInt16 Result = 0;
            I2CMasterSingleWriteModule(SlaveAddModule, 0x81, AddValue[1]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x82, AddValue[0]);//Low

            I2CMasterSingleWriteModule(SlaveAddModule, 0xA8, AddValue[3]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0xA9, AddValue[2]);//Low

            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x02);
            System.Threading.Thread.Sleep(iSleeptime * 2);
            byte High = I2CMasterSingleReadModule(SlaveAddModule, 0x86);
            byte Low = I2CMasterSingleReadModule(SlaveAddModule, 0x87);
            Result = UInt16.Parse (( High * 256 + Low).ToString ());
            return Result;
        }

        public void Write_IN015050(UInt32 Add, UInt16 Data)
        {
            EnterEngMod();
            byte[] AddValue = System.BitConverter.GetBytes(Add);
            SelectPage(0x80);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x81, AddValue[1]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x82, AddValue[0]);//Low

            I2CMasterSingleWriteModule(SlaveAddModule, 0xA8, AddValue[3]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0xA9, AddValue[2]);//Low
            System.Threading.Thread.Sleep(iSleeptime1);

            AddValue = System.BitConverter.GetBytes(Data);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x83, AddValue[1]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, AddValue[0]);//Low
            System.Threading.Thread.Sleep(iSleeptime * 2);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x01);
        }

        public void WriteDAC(UInt16 Data, byte ID)
        {
            EnterEngMod();
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);

            byte[] AddValue = System.BitConverter.GetBytes(Data);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x83, AddValue[1]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, AddValue[0]);//Low
            System.Threading.Thread.Sleep(iSleeptime * 2);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, ID);
        }
        public void WriteDAC_DSP(Int16 Data, byte ID)
        {
            EnterEngMod();
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);

            byte[] AddValue = System.BitConverter.GetBytes(Data);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x83, AddValue[1]);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, AddValue[0]);//Low
            System.Threading.Thread.Sleep(iSleeptime * 2);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, ID);
        }

        public void SetPreTAG(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x28);
        }

        public void SetMainTAG(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x29);
        }

        public void SetPostTAG(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x2A);
        }

        public void SetSwing(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x2B);
        }
        public void SetInnerEye1(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x2C);
        }
        public void SetInnerEye2(Int16 Data)
        {
            WriteDAC_DSP(Data, 0x2D);
        }

        public void SetTxTEC(UInt16 Data)
        {
            WriteDAC(Data, 0x0B);
        }

        public void SetRxVPD(UInt16 Data)
        {
            WriteDAC(Data, 0x0C);
        }

        public void SetTxVB(UInt16 Data)
        {
            WriteDAC(Data, 0x0D);
        }

        public void SetTxVEA(UInt16 Data)
        {
            WriteDAC(Data, 0x0E);
        }

        public void SetTxVG(UInt16 Data)
        {
            WriteDAC(Data, 0x0F);
        }

        public void SetLDiBias(UInt16 Data)
        {
            WriteDAC(Data, 0x10);
        }

        public void SetRxVGC(UInt16 Data)
        {
            WriteDAC(Data, 0x11);
        }

        public bool ReadGPI(byte ID)
        {
            EnterEngMod();
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, ID);
            System.Threading.Thread.Sleep(iSleeptime * 2);
            byte Result = I2CMasterSingleReadModule(SlaveAddModule, 0x86);
            bool bResult = Result == 0x000 ? false : true;
            return bResult;
        }

        public bool ReadLPMode()
        {
            bool bResult = ReadGPI(0x1A);
            return bResult;
        }

        public bool ReadIntGB()
        {
            bool bResult = ReadGPI(0x1B);
            return bResult;
        }

        public bool ReadModseLN()
        {
            bool bResult = ReadGPI(0x1C);
            return bResult;
        }

        public void SetGPO(bool bStatus, byte ID)
        {
            EnterEngMod();
            byte Data = 0;
            if (bStatus)
                Data = 0x01;
            else
                Data = 0x00;
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x83, 0x00);//High
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, Data);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, ID);
        }

        public void WriteRstnGBL(bool bStatus)
        {
            SetGPO(bStatus,0x12);
        }

        public void WriteInitGBL(bool bStatus)
        {
            SetGPO(bStatus, 0x13);
        }

        public void WriteVGB75En(bool bStatus)
        {
            SetGPO(bStatus, 0x14);
        }

        public void WriteTxTECEn(bool bStatus)
        {
            SetGPO(bStatus, 0x15);
        }

        public void WriteVdrv33En(bool bStatus)
        {
            SetGPO(bStatus, 0x16);
        }

        public void WriteVGB11En(bool bStatus)
        {
            SetGPO(bStatus, 0x17);
        }

        public void WriteVcc33En(bool bStatus)
        {
            SetGPO(bStatus, 0x18);
        }

        public void WriteIntn(bool bStatus)
        {
            SetGPO(bStatus, 0x19);
        }

        public void DDM_Enable()
        {
            EnterEngMod();
            SelectPage(0x80);
          
            I2CMasterSingleWriteModule(SlaveAddModule, 0xAC, 0x00);
            System.Threading.Thread.Sleep(100);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0xAC);
            if (Data != 0x00)
                throw new Exception("Disable DDM Error !");

        }
        public void DDM_Disable()
        {

            EnterEngMod();
            SelectPage(0x80);
           
            I2CMasterSingleWriteModule(SlaveAddModule, 0xAC, 0x01);
            System.Threading.Thread.Sleep(100);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0xAC);
            if (Data != 0x01)
                throw new Exception("Disable DDM Error !");

        }
        public bool DDM_Status()
        {
            EnterEngMod();
            SelectPage(0x80);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0xAC);
            bool bEnable = Data == 0x00 ? true : false;
            return bEnable;
        }

     
        public bool GetLOS(bool bTx, int iChannel)
        {
            bool bResult = false;
            SelectPage(0x00);
            byte Value = I2CMasterSingleReadModule(SlaveAddModule, 0x03);
            if (bTx)
                bResult = GetBit(Value, 4 + iChannel);
            else
                bResult = GetBit(Value, iChannel);
            return bResult;
        }
        public bool[] GetLOS(bool bTx)
        {
            bool[] bResult = new bool[4];
            SelectPage(0x00);
            byte Value = I2CMasterSingleReadModule(SlaveAddModule, 0x03);
            for (int iChannel = 0; iChannel < 4; iChannel++)
            {
                if (bTx)
                    bResult[iChannel] = GetBit(Value, 4 + iChannel);
                else
                    bResult[iChannel] = GetBit(Value, iChannel);
            }
            return bResult;
        }
        public bool GetTxFault(int iChannel)
        {
            bool bResult = false;
            SelectPage(0x00);
            byte Value = I2CMasterSingleReadModule(SlaveAddModule, 0x04);
            bResult = GetBit(Value, iChannel);
            return bResult;
        }
        public bool GetLOL(bool bTx, int iChannel)
        {
            bool bResult = false;
            SelectPage(0x00);
            byte Value = I2CMasterSingleReadModule(SlaveAddModule, 0x05);
            if (bTx)
                bResult = GetBit(Value, 4 + iChannel);
            else
                bResult = GetBit(Value, iChannel);
            return bResult;
        }

        public double GetModuleTemperatureCalc()
        {
            SelectPage(0x80);
            byte DataHigh = I2CMasterSingleReadModule(SlaveAddModule, 0x9E);
            System.Threading.Thread.Sleep(iSleeptime1);
            byte DataLow = I2CMasterSingleReadModule(SlaveAddModule, 0x9F);

            double Result = double.Parse((DataHigh * 256 + DataLow).ToString());

            Result = Math.Round(Result / 256.0, 1);
            return Result;
        }

        public double ReadTemperature_C()
        {
            SelectPage(0x00);
            byte DataHigh = I2CMasterSingleReadModule(SlaveAddModule, 0x16);
            System.Threading.Thread.Sleep(iSleeptime1);
            byte DataLow = I2CMasterSingleReadModule(SlaveAddModule, 0x17);

            //double Result = double.Parse((DataHigh * 256 + DataLow).ToString());
            byte[] data = new byte[2] { DataLow, DataHigh };
            double value = BitConverter.ToInt16(data, 0) / 256.0; ;
            value = Math.Round(value, 3);

            //Result = Math.Round(Result / 256.0, 1);
            return value;
        }

        public double ReadVcc_V()
        {
            SelectPage(0x00);
            byte DataHigh = I2CMasterSingleReadModule(SlaveAddModule, 0x1A);
            System.Threading.Thread.Sleep(iSleeptime1);
            byte DataLow = I2CMasterSingleReadModule(SlaveAddModule, 0x1B);

            double Result = double.Parse((DataHigh * 256 + DataLow).ToString());

            Result = Math.Round(Result * 0.1 / 1000.0, 2);
            return Result;
        }

        public double ReadRxPower_mW(int iChannel)
        {
            SelectPage(0x00);
            byte[] Data = I2CMasterMultReadModule(SlaveAddModule, byte.Parse((0x22 + iChannel * 2).ToString()), 2);
            double Result = double.Parse((Data[0] * 256 + Data[1]).ToString());

            Result = Math.Round(Result * 0.1 / 1000.0, 6);
            return Result;
        }

        public double ReadTxPower_mW(int iChannel)
        {
            SelectPage(0x00);
            byte[] Data = I2CMasterMultReadModule(SlaveAddModule, byte.Parse((0x32 + iChannel * 2).ToString()), 2);
            double Result = double.Parse((Data[0] * 256 + Data[1]).ToString());

            Result = Math.Round(Result * 0.1 / 1000.0, 6);
            return Result;
        }

        public double ReadTxBias_mA(int iChannel)
        {
            SelectPage(0x00);
            byte[] Data = I2CMasterMultReadModule(SlaveAddModule, byte.Parse((0x2A + iChannel * 2).ToString()), 2);
            double Result = double.Parse((Data[0] * 256 + Data[1]).ToString());

            Result = Math.Round(Result * 2 / 1000.0, 1);
            return Result;
        }

        public void MaskingTxLos(int iChannel, bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            Data = set_bit(Data, iChannel + 5, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x64, Data);
        }

        public void MaskingTxLos(bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 5, bMasking);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x64, Data);
        }

        public bool GetMaskingTxLos(int iChannel)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            bool bResult = GetBit(Data, iChannel + 4);
            return bResult;
        }

        public bool[] GetMaskingTxLosAll()
        {
            SelectPage(0x00);
            bool[] bResult = new bool[4];
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            for (int i = 0; i < 4; i++)
                bResult[i] = GetBit(Data, i + 4);
            return bResult;
        }

        public void MaskingRxLos(int iChannel, bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            Data = set_bit(Data, iChannel + 1, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x64, Data);
        }

        public void MaskingRxLos(bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 1, bMasking);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x64, Data);
        }

        public bool GetMaskingRxLos(int iChannel)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            bool bResult = GetBit(Data, iChannel);
            return bResult;
        }

        public bool[] GetMaskingRxLosAll()
        {
            SelectPage(0x00);
            bool[] bResult = new bool[4];
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x64);
            for (int i = 0; i < 4; i++)
                bResult[i] = GetBit(Data, i);
            return bResult;
        }

        public void MaskingTxFault(int iChannel, bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x65);
            Data = set_bit(Data, iChannel + 1, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x65, Data);
        }

        public void MaskingTxFault(bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x65);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 1, bMasking);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x65, Data);
        }

        public bool GetMaskingTxFault(int iChannel)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x65);
            bool bResult = GetBit(Data, iChannel);
            return bResult;
        }

        public bool[] GetMaskingTxFaultAll()
        {
            SelectPage(0x00);
            bool[] bResult = new bool[4];
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x65);
            for (int i = 0; i < 4; i++)
                bResult[i] = GetBit(Data, i);
            return bResult;
        }

        public void MaskingTxLOL(int iChannel, bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x66);
            Data = set_bit(Data, iChannel + 5, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x66, Data);
        }

        public void MaskingTxLOL(bool bMasking)
        {
            SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x66);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 5, bMasking);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x66, Data);
        }
        public void MaskingRxLOL(int iChannel, bool bMasking)
        {
            //SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x66);
            Data = set_bit(Data, iChannel + 1, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x66, Data);
        }

        public void MaskingRxLOL(bool bMasking)
        {
            //SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x66);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 1, bMasking);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x66, Data);
        }

        public void MaskingTemperatureHighAlarm(bool bMasking)
        {
            byte Add = 0x67;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 8, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTemperatureLowAlarm(bool bMasking)
        {
            byte Add = 0x67;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 7, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTemperatureHighWarning(bool bMasking)
        {
            byte Add = 0x67;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 6, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTemperatureLowWarning(bool bMasking)
        {
            byte Add = 0x67;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 5, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingVccHighAlarm(bool bMasking)
        {
            byte Add = 0x68;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 8, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingVccLowAlarm(bool bMasking)
        {
            byte Add = 0x68;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 7, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingVccHighWarning(bool bMasking)
        {
            byte Add = 0x68;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 6, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingVccLowWarning(bool bMasking)
        {
            byte Add = 0x68;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, 5, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingRxPowerHighAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF2;
            if (iChannel > 1)
                Add = 0xF3;
            int iIndex = 8;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 4;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingRxPowerLowAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF2;
            if (iChannel > 1)
                Add = 0xF3;
            int iIndex = 7;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 3;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }
        public void MaskingRxPowerHighWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF2;
            if (iChannel > 1)
                Add = 0xF3;
            int iIndex = 6;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 2;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingRxPowerLowWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF2;
            if (iChannel > 1)
                Add = 0xF3;
            int iIndex = 5;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 1;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxBiasHighAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF4;
            if (iChannel > 1)
                Add = 0xF5;
            int iIndex = 8;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 4;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxBiasLowAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF4;
            if (iChannel > 1)
                Add = 0xF5;
            int iIndex = 7;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 3;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }
        public void MaskingTxBiasHighWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF4;
            if (iChannel > 1)
                Add = 0xF5;
            int iIndex = 6;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 2;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxBiasLowWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF4;
            if (iChannel > 1)
                Add = 0xF5;
            int iIndex = 5;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 1;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxPowerHighAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF6;
            if (iChannel > 1)
                Add = 0xF7;
            int iIndex = 8;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 4;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxPowerLowAlarm(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF6;
            if (iChannel > 1)
                Add = 0xF7;
            int iIndex = 7;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 3;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }
        public void MaskingTxPowerHighWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF6;
            if (iChannel > 1)
                Add = 0xF7;
            int iIndex = 6;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 2;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public void MaskingTxPowerLowWarning(int iChannel, bool bMasking)
        {
            SelectPage(0x03);
            byte Add = 0xF4;
            if (iChannel > 1)
                Add = 0xF5;
            int iIndex = 5;
            if (iChannel == 1 || iChannel == 3)
                iIndex = 1;
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, Add);
            Data = set_bit(Data, iIndex, bMasking);
            I2CMasterSingleWriteModule(SlaveAddModule, Add, Data);
        }

        public bool[] GetDisableTxAll()
        {
            ExitEngMod();
            bool[] bStatus = new bool[4];
            //SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x56);
            for (int i = 0; i < 4; i++)
            {
                bStatus[i] = Get_Bit(Data, i);
            }
            return bStatus;
        }
        public void DisableTxAll(bool bDisable)
        {
            ExitEngMod();
            //SelectPage(0x00);
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x56);
            for (int i = 0; i < 4; i++)
            {
                Data = set_bit(Data, i + 1, bDisable);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0x56, Data);

            bool[] bStatus = GetDisableTxAll();
            for (int i = 0; i < 4; i++)
            {
                if (bStatus[i] != bDisable)
                    throw new Exception("Disabel Tx All ERROR !");
            }
        }
        public void DisableTx(int iChannel, bool bDisable)
        {
            byte Data = I2CMasterSingleReadModule(SlaveAddModule, 0x56);
            Data = set_bit(Data, iChannel + 1, bDisable);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x56, Data);
            byte DataRead = I2CMasterSingleReadModule(SlaveAddModule, 0x56);
            if (Data != DataRead)
                throw new Exception("Disabel Tx  ERROR ! Channel " + iChannel);
        }

        public void SaveEEPROMAPC()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x01);
                System.Threading.Thread.Sleep(1000);
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xE6);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x0);
            }
        }

        public void ClearSaveEEPROMAPC()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xE6);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x0);
            }
        }

        public void SaveEEPROMAPC(int iChannel)
        {
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x01);
            System.Threading.Thread.Sleep(1000);
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xE6);
            if (data == 1)
                I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x0);
        }

        public void ClearSaveEEPROMAPC(int iChannel)
        {
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xE6);
            if (data == 1)
                I2CMasterSingleWriteModule(SlaveAddModule, 0xE6, 0x0);
        }
        public void SaveEEPROMTuningCoeff()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                I2CMasterSingleWriteModule(SlaveAddModule, 0x8D, 0x01);
                System.Threading.Thread.Sleep(1000);
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0x8D);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0x8D, 0x0);
            }
        }

        public void ClearEEPROMTuningCoeff()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0x8D);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0x8D, 0x0);
            }
        }
        public void SaveEEPROMVcc()
        {
            SelectPage(0x80);
            I2CMasterSingleWriteModule(SlaveAddModule, 0xA3, 0x01);
            System.Threading.Thread.Sleep(1000);
        }

        public void ClearSaveEEPROMVcc()
        {
            SelectPage(0x80);
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xA3);
            if (data == 1)
                I2CMasterSingleWriteModule(SlaveAddModule, 0xA3, 0x0);
        }

        public void SaveEEPROMCoef()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x01);
                System.Threading.Thread.Sleep(1000);
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xC8);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x0);
            }
        }

        public void ClearSaveEEPROMCoef()
        {
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xC8);
                if (data == 1)
                    I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x0);
            }
        }

        public void SaveEEPROMCoef(int iChannel)
        {
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x01);
            System.Threading.Thread.Sleep(1000);
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xC8);
            if (data == 1)
                I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x0);
        }

        public void ClearSaveEEPROMCoef(int iChannel)
        {
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0xC8);
            if (data == 1)
                I2CMasterSingleWriteModule(SlaveAddModule, 0xC8, 0x0);
        }

        public void SetAPCLoop(bool bOn)
        {
            EnterEngMod();
            byte Data = 0;
            if (bOn)
                Data = 0x01;
            else
                Data = 0x00;
            for (int i = 0; i < 4; i++)
            {
                SelectPage(byte.Parse((0x81 + i).ToString()));
                I2CMasterSingleWriteModule(SlaveAddModule, 0x81, Data);
                byte DataRead = I2CMasterSingleReadModule(SlaveAddModule, 0x81);
                if (Data != DataRead)
                    throw new Exception("Set APC Loop Error ! ");

            }
        }

        public void SetAPCLoop(int iChannel, bool bOn)
        {
            EnterEngMod();
            byte Data = 0;
            if (bOn)
                Data = 0x01;
            else
                Data = 0x00;
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            I2CMasterSingleWriteModule(SlaveAddModule, 0x81, Data);
            byte DataRead = I2CMasterSingleReadModule(SlaveAddModule, 0x81);
            if (Data != DataRead)
                throw new Exception("Set APC Loop Error ! Channel " + iChannel);

        }

        public bool GetAPCLoop(int iChannel)
        {
            EnterEngMod();
            SelectPage(byte.Parse((0x81 + iChannel).ToString()));
            byte data = I2CMasterSingleReadModule(SlaveAddModule, 0x81);
            bool bOn = false;
            if (data == 0x01)
                bOn = true;
            else if (data == 0x00)
                bOn = false;
            return bOn;
        }

        public double GetTemperatureHighAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x80);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x81);
            byte[] data = new byte[2] { dataL, dataH };
            double value = BitConverter.ToInt16(data, 0) / 256.0; ;

            return value;
        }

        public double GetTemperatureLowAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x82);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x83);

            byte[] data = new byte[2] { dataL, dataH };
            double value = BitConverter.ToInt16(data, 0) / 256.0; ;
            return value;
        }

        public double GetTemperatureHighWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x84);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x85);

            byte[] data = new byte[2] { dataL, dataH };
            double value = BitConverter.ToInt16(data, 0) / 256.0; ;
            return value;
        }

        public double GetTemperatureLowWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x86);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x87);

            byte[] data = new byte[2] { dataL, dataH };
            double value = BitConverter.ToInt16(data, 0) / 256.0; ;
            return value;
        }

        public double GetVccHighAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x90);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x91);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000.0;
            return value;
        }

        public double GetVccLowAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x92);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x93);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000.0;
            return value;
        }

        public double GetVccHighWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x94);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x95);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000.0;
            return value;
        }

        public double GetVccLowWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0x96);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0x97);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000.0;
            return value;
        }

        public double GetRxPowerHighAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xB0);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xB1);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetRxPowerLowAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xB2);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xB3);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetRxPowerHighWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xB4);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xB5);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetRxPowerLowWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xB6);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xB7);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetTxBiasHighAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xB8);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xB9);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 2 / 1000;
            return value;
        }

        public double GetTxBiasLowAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xBA);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xBB);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 2 / 1000;
            return value;
        }

        public double GetTxBiasHighWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xBC);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xBD);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 2 / 1000;
            return value;
        }

        public double GetTxBiasLowWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xBE);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xBF);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 2 / 1000;
            return value;
        }

        public double GetTxPowerHighAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xC0);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xC1);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetTxPowerLowAlarmThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xC2);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xC3);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetTxPowerHighWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xC4);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xC5);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public double GetTxPowerLowWarningThreshold()
        {
            //EnterEngMod();
            SelectPage(0x03);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xC6);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xC7);

            double value = double.Parse((dataH * 256.0 + dataL).ToString()) * 0.1 / 1000;
            value = Convert_mWtodBm(value);
            value = Math.Round(value, 3);
            return value;
        }

        public string ReadVendorName()
        {
            SelectPage(0x00);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0x94, 16);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            return Name;
        }

        public string ReadVendorPN()
        {
            SelectPage(0x00);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0xA8, 16);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            return Name;
        }

        public string ReadVendorRev()
        {
            SelectPage(0x00);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0xB8, 2);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            return Name;
        }

        public double ReadWavelength()
        {
            SelectPage(0x00);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xBA);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xBB);

            double length = (dataH * 256.0 + dataL) * 0.05;
            return length;
        }

        public double ReadWavelengthTolerance()
        {
            SelectPage(0x00);
            byte dataH = I2CMasterSingleReadModule(SlaveAddModule, 0xBC);
            byte dataL = I2CMasterSingleReadModule(SlaveAddModule, 0xBD);

            double length = (dataH * 256.0 + dataL) * 0.005;
            return length;
        }

        public string ReadVendorSN()
        {
            SelectPage(0x00);
            byte[] data = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                data[i] = I2CMasterSingleReadModule(SlaveAddModule, byte.Parse((0xC4 + i).ToString()));
            }
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0xC4, 16);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            return Name;
        }

        public string ReadDate()
        {
            SelectPage(0x00);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0xD4, 6);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            //Name = "20" + Name;
            return Name;
        }
        public string ReadPCBASN()
        {
            SelectPage(0x80);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0xB4, 10);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            //Name = "20" + Name;
            return Name;
        }
        public byte CalcCC_Base_191()
        {
            int sum = 0;
            for (int i = 128; i < 191; i++)
            {
                sum += I2CMasterSingleReadModule(SlaveAddModule, byte.Parse(i.ToString()));
            }
            byte[] checksum = BitConverter.GetBytes(sum);

            I2CMasterSingleWriteModule(SlaveAddModule, 0xBF, checksum[0]);
            return checksum[0];
        }

        public byte CalcCC_Base_223()
        {
            int sum = 0;
            for (int i = 192; i < 223; i++)
            {
                sum += I2CMasterSingleReadModule(SlaveAddModule, byte.Parse(i.ToString()));
            }
            byte[] checksum = BitConverter.GetBytes(sum);
            I2CMasterSingleWriteModule(SlaveAddModule, 0xDF, checksum[0]);
            return checksum[0];
        }
        public byte CalcOnlyCC_Base_191()
        {
            int sum = 0;
            for (int i = 128; i < 191; i++)
            {
                sum += I2CMasterSingleReadModule(SlaveAddModule, byte.Parse(i.ToString()));
            }
            byte[] checksum = BitConverter.GetBytes(sum);
            return checksum[0];
        }

        public byte CalcOnlyCC_Base_223()
        {
            int sum = 0;
            for (int i = 192; i < 223; i++)
            {
                sum += I2CMasterSingleReadModule(SlaveAddModule, byte.Parse(i.ToString()));
            }
            byte[] checksum = BitConverter.GetBytes(sum);
            return checksum[0];
        }
        public void SetVendorName(string Name)
        {
            EnterEngMod();
            SelectPage(0x00);
            int iLen = Name.Length;
            if (iLen > 16)
            {
                Name = Name.Substring(0, 16);
            }
            Name = Name.PadRight(16, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(Name);
            for (int i = 0; i < 16; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0x94 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            CalcCC_Base_191();
        }

        public void SetVendorPN(string PN)
        {
            EnterEngMod();
            SelectPage(0x00);
            int iLen = PN.Length;
            if (iLen > 16)
            {
                PN = PN.Substring(0, 16);
            }
            PN = PN.PadRight(16, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(PN);
            for (int i = 0; i < 16; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0xA8 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            CalcCC_Base_191();
        }

        public void SetVendorRev(string Rev)
        {
            EnterEngMod();
            SelectPage(0x00);
            int iLen = Rev.Length;
            if (iLen > 2)
            {
                Rev = Rev.Substring(0, 2);
            }
            Rev = Rev.PadRight(2, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(Rev);
            for (int i = 0; i < 2; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0xB8 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            CalcCC_Base_191();
        }

        public void SetVendorSN(string SN)
        {
            EnterEngMod();
            SelectPage(0x00);
            int iLen = SN.Length;
            if (iLen > 16)
            {
                SN = SN.Substring(0, 16);
            }
            SN = SN.PadRight(16, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(SN);
            for (int i = 0; i < 16; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0xC4 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            CalcCC_Base_223();
            //byte[] vv = I2CMasterMultReadModule(SlaveAddModule, 0xC4, 16);
        }

        public void SetVendorDate(string Date)
        {
            EnterEngMod();
            SelectPage(0x00);
            int iLen = Date.Length;
            if (iLen > 6)
            {
                Date = Date.Substring(0, 6);
            }
            Date = Date.PadRight(6, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(Date);
            for (int i = 0; i < 6; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0xD4 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            CalcCC_Base_223();
        }
        public void SetPCBASN(string SN)
        {
            EnterEngMod();
            SelectPage(0x80);
            I2CMasterSingleWriteModule(SlaveAddModule, 0xC0, 0x00);
            System.Threading.Thread.Sleep(500);
            int iLen = SN.Length;
            if (iLen > 10)
            {
                SN = SN.Substring(0, 10);
            }
            SN = SN.PadRight(10, ' ');
            byte[] array = System.Text.Encoding.ASCII.GetBytes(SN);
            for (int i = 0; i < 10; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0xB4 + i).ToString()), array[i]);
                System.Threading.Thread.Sleep(iSleeptime1);
            }
            I2CMasterSingleWriteModule(SlaveAddModule, 0xC0, 0x01);
            System.Threading.Thread.Sleep(1000);

            //CalcCC_Base_223();
        }

        public string GetFirmwareVer()
        {
            //EnterEngMod();
            //SelectPage(0x80);
            byte[] Value = I2CMasterMultReadModule(SlaveAddModule, 0x1e, 4);
            string Name = System.Text.Encoding.ASCII.GetString(Value);//
            string Ver = Name.Substring(0, 1) + "." + Value[1] + "." + Value[2] + "." + Value[3];
            return Ver;
        }
        public string GetHostBoardVer()
        {
            Byte[] WriteData = new Byte[iDataNum];
            int MaxRbuffersize = 3; // 0x40;
            Byte[] ReadData = new Byte[3];

            WriteData[0] = 0x56;
            WriteData[1] = SlaveAddModule;//slave address
            WriteData[2] = 0;

            WriteData[3] = 0;
            WriteData[4] = 0;
            //WriteData[5] = Data[2];
            //WriteData[6] = Data[3];
            if (ClassUSBTX(WriteData))
            {
                if (!ClassUSBRX(ref ReadData, MaxRbuffersize))
                    throw new Exception("USB RX Failed !");
            }
            else
                throw new Exception("USB TX Failed !");
            byte[] temp = ReadData;
            string Name = System.Text.Encoding.ASCII.GetString(temp);//

            string Ver = Name.Substring(0, 1) + "." + Name.Substring(1, 1) + "." + Name.Substring(2, 1);
            return Ver;
        }

        public void EnableDCDC(bool bEnable)
        {
            EnterEngMod();
            byte Value = 0;
            if (bEnable)
                Value = 0x01;
            else
                Value = 0x00;
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, Value);
            System.Threading.Thread.Sleep(iSleeptime * 2);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x08);
            double totalTimeFind = 0;
            DateTime startTime = System.DateTime.Now;

        }

        public void EnableRx16(bool bEnable)
        {
            EnterEngMod();
            byte Value = 0;
            if (bEnable)
                Value = 0x01;
            else
                Value = 0x00;
            I2CMasterSingleWriteModule(SlaveAddModule, 0x84, Value);
            System.Threading.Thread.Sleep(iSleeptime * 2);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x09);
            double totalTimeFind = 0;
            DateTime startTime = System.DateTime.Now;

            //while (true)
            //{
            //    byte Dataa = I2CMasterSingleReadModule(SlaveAddModule, 0x89);
            //    totalTimeFind = (System.DateTime.Now - startTime).TotalMilliseconds;
            //    if (Dataa == 0x01)
            //    {
            //        break;
            //    }
            //    if (totalTimeFind > 2000)
            //    {
            //        throw new Exception("Read CDR Error !");
            //    }
            //}
        }

        public void EnterLPMode(bool bLP)
        {
            if (bLP)
                I2CMasterSingleWriteModule(SlaveAddModule, 0x5D, 0x03);
            else
                I2CMasterSingleWriteModule(SlaveAddModule, 0x5D, 0x00);
        }

        public bool CheckReadyDSP(ref string Reason)
        {
            bool bReady = false;
            double totalTimeFind = 0;
            DateTime startTime = System.DateTime.Now;
            byte Data = 1;
            while (true)
            {
                Data = I2CMasterSingleReadModule(SlaveAddModule, 0x85);
                totalTimeFind = (System.DateTime.Now - startTime).TotalMilliseconds;
                if (Data == 0)
                {
                    Data = I2CMasterSingleReadModule(SlaveAddModule, 0x86);
                    if (Data == 3)
                    {
                        bReady = true;
                        break;
                    }
                    else
                    {
                        Reason = "Write Block Fail";
                        break;
                    }
                }
                if (totalTimeFind > 5000)
                {
                    Reason = "Status Fail";
                    break;
                }
            }
            return bReady;
        }

        public bool  IniDSP(ref string Reason)
        {
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x54);
            bool bSuccess = CheckReadyDSP(ref Reason);
            return bSuccess;
        }

        public bool  EndIniDSP(ref string Reason)
        {
            SelectPage(0x80);
            System.Threading.Thread.Sleep(iSleeptime1);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x56);
            System.Threading.Thread.Sleep(3000);
            bool bSuccess = CheckReadyDSP(ref Reason);
            return bSuccess;
        }
        public bool WriteDSP(byte[] Data,bool bLast, ref string Reason)
        {
            SelectPage(0x85);
            UInt16 sum = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                I2CMasterSingleWriteModule(SlaveAddModule, byte.Parse((0x80 + i).ToString()), Data[i]);
                sum += Data[i];
            }
            //System.Threading.Thread.Sleep(iSleeptime1);

            SelectPage(0x80);
            //System.Threading.Thread.Sleep(iSleeptime1);
            byte[] checksum = BitConverter.GetBytes(sum);

            I2CMasterSingleWriteModule(SlaveAddModule, 0x8B, checksum[0]);//LSB
            I2CMasterSingleWriteModule(SlaveAddModule, 0x8A, checksum[1]);//LSB

            I2CMasterSingleWriteModule(SlaveAddModule, 0x83, byte.Parse(Data.Length.ToString()));

            if (bLast )
                I2CMasterSingleWriteModule(SlaveAddModule, 0x84, 0xAA);

            //System.Threading.Thread.Sleep(iSleeptime);
            I2CMasterSingleWriteModule(SlaveAddModule, 0x80, 0x55);

            System.Threading.Thread.Sleep(iSleeptime1);
            bool bSuccess = CheckReadyDSP(ref Reason);

            return bSuccess;
        }
        public static double Convert_mWtodBm(double power_mW)
        {
            // Conversion
            // check for negative power
            if (power_mW <= 0.0) return -10000.0;
            return 10.0 * Math.Log10(power_mW);
        }
    }

}

