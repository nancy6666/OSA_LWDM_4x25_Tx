using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GY7501_I2C_Control
{
    class DataManagement
    {
        public List<string> ReadConfigValues(string fileFullPath)
        {
            List<string> lstConfigLine = new List<string>();
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

                    for (int i = 0; i < 4; i++)
                    {
                        if ((rl = sr.ReadLine()) != null)
                        {
                            lstConfigLine.Add(rl);
                        }
                        else
                        {
                            throw new Exception("配置文件格式出错，请确认有4个通道的配置参数");
                        }
                    }
                }
            }
            return lstConfigLine;
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
    }
}
