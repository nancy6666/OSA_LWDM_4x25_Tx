using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
{
   public class CLog
    {
        public void WriteLog(string Action)
        {
            try
            {
                string strLogPath = $"{Directory.GetCurrentDirectory()}\\Log\\";
                string strLogName = System.DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "-");
            
                //判斷是否有這樣的路徑并創建
                if (System.IO.Directory.Exists(strLogPath) == false)
                {
                    System.IO.Directory.CreateDirectory(strLogPath);
                }
                strLogName = strLogPath + strLogName + ".txt";
                ////如果文件不存在，會自動創建
                string strNote = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                strNote += ":  "  + Action ;
                System.IO.StreamWriter file = new System.IO.StreamWriter(strLogName, true);
                file.WriteLine(strNote);
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Write Log error,{ex.Message}");
            }
        }
    }
}
