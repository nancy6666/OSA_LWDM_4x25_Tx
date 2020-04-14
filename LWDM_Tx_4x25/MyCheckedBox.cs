using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GY7501_I2C_Control
{
   public class MyCheckedBox : CheckBox
    {
        private MyRadioButton myRadioButton1;

        public byte RegAddr { get; set; }
       
        /// <summary>
        /// 该控件在字节中所占的某一位的索引，从0开始计
        /// </summary>
        public int RegBitIndex { get; set; }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            List<byte> dataBuff = new List<byte>();
            var valueRead = GlobalVar.uSB_I2C_Adapter.ReadValue(this.RegAddr);

            var BitValue = Convert.ToInt16(this.Checked);
            //从寄存器读到的值的RegBitIndex那一位，置为0，其余为保持不变
            var initialWithoutRegbitIndex = valueRead[0] & ~(1 << RegBitIndex);
            //initialWithoutRegbitIndex与控件所得的值移位后进行或操作，为了获取控件所代表的这一位的值，且其余位保持现有值。
            var valueToWrite = initialWithoutRegbitIndex | BitValue << RegBitIndex;
            dataBuff.Add(RegAddr);
            dataBuff.Add(Convert.ToByte(valueToWrite));
            GlobalVar.uSB_I2C_Adapter.SetValue(dataBuff.ToArray());
        }

        private void InitializeComponent()
        {
            this.myRadioButton1 = new GY7501_I2C_Control.MyRadioButton();
            this.SuspendLayout();
            // 
            // myRadioButton1
            // 
            this.myRadioButton1.AutoSize = true;
            this.myRadioButton1.Location = new System.Drawing.Point(0, 0);
            this.myRadioButton1.Name = "myRadioButton1";
            this.myRadioButton1.RegAddr = ((byte)(0));
            this.myRadioButton1.RegBitIndex = 0;
            this.myRadioButton1.Size = new System.Drawing.Size(104, 24);
            this.myRadioButton1.TabIndex = 0;
            this.myRadioButton1.TabStop = true;
            this.myRadioButton1.Text = "myRadioButton1";
            this.myRadioButton1.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);

        }
    }
}
