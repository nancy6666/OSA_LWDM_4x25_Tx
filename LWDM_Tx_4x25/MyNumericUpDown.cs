using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GY7501_I2C_Control
{
  public  class MyNumericUpDown:NumericUpDown
    {
        public byte RegAddr { get; set; }
        /// <summary>
        /// 需要写入的寄存器个数
        /// </summary>
        public int RegNum { get; set; }
        /// <summary>
        /// 若为true 则表明写入寄存器的值需要由多个控件合成，需要回读寄存器的值
        /// </summary>             
        public bool RegReadBackStatus { get; set; }
        /// <summary>
        /// 控件的数据在要写入寄存器的字节中所占位数
        /// </summary>
        public int RegDataBitNum { get; set; }

        private byte oldValue { get; set; }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
           
            List<byte> dataBuff = new List<byte>();
            if(e.KeyChar== (char)Keys.Enter)
            {
                var valueToWrite = Convert.ToByte(this.Value);
                if (RegNum == 2)
                {
                    byte[] byteArray = BitConverter.GetBytes((ushort)(this.Value));
                    dataBuff.Add(RegAddr);
                    dataBuff.Add(byteArray[1]);
                    dataBuff.Add(byteArray[0]);
                    GlobalVar.uSB_I2C_Adapter.SetValue(dataBuff.ToArray());
                }
                if(RegReadBackStatus)//回读寄存器的值,取data以外的后几位，并与当前控件里的data值合成
                {
                    var valueRead = GlobalVar.uSB_I2C_Adapter.ReadValue(this.RegAddr);
                    var moveBit = 8 - RegDataBitNum;
                    var initialValueBits = new BitArray(valueRead);
                    var initialValue=0;
                    for (int i=0;i<moveBit;i++)
                    {
                        byte bit =Convert.ToByte( initialValueBits.Get(moveBit - 1 - i));
                        initialValue = initialValue << 1 | bit;
                    }
                    var value = valueToWrite << moveBit | initialValue ;
                    dataBuff.Add(RegAddr);
                    dataBuff.Add(Convert.ToByte(value));
                    GlobalVar.uSB_I2C_Adapter.SetValue(dataBuff.ToArray());
                }
                else
                {
                    dataBuff.Add(RegAddr);
                    dataBuff.Add(valueToWrite);
                    GlobalVar.uSB_I2C_Adapter.SetValue(dataBuff.ToArray());
                }

                oldValue = valueToWrite;
                this.BackColor = Color.White;
                this.Select(0, this.Text.Length);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if(this.Text!=oldValue.ToString())
            {
                this.BackColor = Color.LightGray;
            }
            else
            {
                this.BackColor=Color.White;
            }
        }
    }
}
