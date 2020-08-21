using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWDM_Tx_4x25
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConfigManagement config = new ConfigManagement();
            Process m_Process = null;
            m_Process = new Process();
            m_Process.StartInfo.FileName = config.DCAN1092Path;
          //  m_Process.Start();
            Thread.Sleep(4000);
            LWDM_Tx_4x25 mainFrame = new LWDM_Tx_4x25();
            Application.Run(mainFrame);
        }
    }
}
