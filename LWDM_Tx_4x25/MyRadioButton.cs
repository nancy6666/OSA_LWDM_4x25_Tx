using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GY7501_I2C_Control
{
  public  class MyRadioButton:RadioButton
    {
        public byte RegAddr { get; set; }
        /// <summary>
        /// 该控件在字节中所占的某一位的索引，从0开始计
        /// </summary>
        public int RegBitIndex { get; set; }
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
          
            List<byte> dataBuff = new List<byte>();
            var valueRead = GlobalVar.uSB_I2C_Adapter.ReadValue(this.RegAddr);
            var radioValue = this.Checked == false ? 0 : 1;
            var valueToWrite = valueRead[0] | radioValue << RegBitIndex;
            dataBuff.Add(RegAddr);
            dataBuff.Add(Convert.ToByte(valueToWrite));
            GlobalVar.uSB_I2C_Adapter.SetValue(dataBuff.ToArray());
        }
    }
}
