using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWDM_Tx_4x25
{
    public class CDatabase
    {
        public SqlConnection conn;
        public ConfigManagement cfg = new ConfigManagement();
        public CDatabase()
        {
            string connectString = cfg.DBConnectString;
            conn = new SqlConnection(connectString);
        }

        public void SaveTestData(CTestDataCommon testData)
        {
            SqlCommand cmd = new SqlCommand() { Connection = conn, Transaction = conn.BeginTransaction() };

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to open SQL Server connection, {ex.Message}");
            }
            try
            {
                var strSql = $"insert into dbo.tx_lwdm_4x25_test_common (spec_id,sn,operator,test_station,test_start_time,test_stop_time,pf) output inserted.id values({testData.Spec_id}, '{testData.SN}','{testData.Operator}','{SystemInformation.ComputerName}','{testData.Test_Start_Time}','{testData.Test_Stop_Time}','{testData.pf}')";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSql;
                testData.ID=(int) cmd.ExecuteScalar();//返回插入行的id
         
                for (int i = 0; i < testData.lstTestData_Temp.Count; i++)
                {
                    CTestData_Temp data = testData.lstTestData_Temp[i];
                    data.Common_id = testData.ID;
                    strSql = $"insert into dbo.tx_lwdm_4x25_test_common_temp (common_id, temp_out,temp_in, vcc1,vcc2,vcc3,icc1,icc2,icc3,itec,pf) output inserted.id values({data.Common_id}, '{data.Temp_out}','{data.Temp_in}','{data.Vcc1}','{data.Vcc2}','{data.Vcc3}','{data.Icc1}','{data.Icc2}','{data.Icc3}','{data.Itec}','{data.Pf}')";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strSql;
                    //插入一行与温度相关，与通道无关数据，返回id
                    var data_temp_id = (int)cmd.ExecuteScalar();

                    for(int ch=0;ch<data.lstTestData_Channel.Count;ch++)
                    {
                        CTestData_Channel data_Channel = data.lstTestData_Channel[ch];
                        data_Channel.Temp_id = data_temp_id;
                        data_Channel.Channel = ch;
                        strSql = $"insert into dbo.tx_lwdm_4x25_test_common_ret (temp_id, channel,vcpa, veq,vmod,isink,ldd,power,impd,idark,cwl,smsr,jitter_pp,jitter_rms,crossing,fall_time,rise_time,er,mask_margin) values('{data_Channel.Temp_id}','{data_Channel.Channel}','{data_Channel.Vcpa}','{data_Channel.Veq}','{data_Channel.Vmod}','{data_Channel.Isink}','{data_Channel.Ldd}','{data_Channel.Power}','{data_Channel.Impd}','{data_Channel.Power}','{data_Channel.Impd}','{data_Channel.Idark}','{data_Channel.Cwl}','{data_Channel.SMSR}','{data_Channel.Jitter_pp}','{data_Channel.Jitter_rms}','{data_Channel.Crossing}','{data_Channel.Fall_time}','{data_Channel.Rise_time}','{data_Channel.Er}','{data_Channel.Mask_Margin}')";
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                throw new Exception($"Fail to save test data, {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public List<string> GetAllPN()
        {
            List<string> lstPN = new List<string>();
            try
            {
                string sql = string.Format("select distinct pn from dbo.tx_lwdm_4x25_spec");

                DataSet ds = ReadRawSQL(sql);

                DataTable dt = ds.Tables[0];
                if (ds.Tables != null && ds.Tables.Count == 1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lstPN.Add(dt.Rows[i]["pn"].ToString().Trim());
                    }
                }
         
                return lstPN;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to get all PNs, {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public CTestSpec GetTestSpec(string pn)
        {
            CTestSpec testSpec = new CTestSpec();
            testSpec.PN = pn;
            try
            {
                string sql = string.Format($"select * from dbo.tx_lwdm_4x25_spec where pn='{pn}' and temp=25 and version=(select Max(version) from dbo.tx_lwdm_4x25_spec where pn='{pn}'and temp=25)");

                DataSet ds = ReadRawSQL(sql);
                DataTable dt = ds.Tables[0];
                if (ds.Tables != null && ds.Tables.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    testSpec.ID = Convert.ToInt32(row["id"]);
                    testSpec.Cwl_min = Convert.ToDouble(row["cwl_min"]);
                    testSpec.Cwl_max = Convert.ToDouble(row["cwl_max"]);
                    testSpec.SMSR_min = Convert.ToDouble(row["smsr_min"]);
                    testSpec.SMSR_max = Convert.ToDouble(row["smsr_max"]);
                    testSpec.Power_min =Convert.ToDouble(row["power_min"]);
                    testSpec.Power_max= Convert.ToDouble(row["power_max"]);
                    testSpec.Impd_min=  Convert.ToDouble(row["impd_min"]);
                    testSpec.Impd_max = Convert.ToDouble(row["impd_max"]);
                    testSpec.Idark_min = Convert.ToDouble(row["idark_min"]);
                    testSpec.Idark_min = Convert.ToDouble(row["idark_max"]);
                    testSpec.Jitter_pp_min =  Convert.ToDouble(row["jitter_pp_min"]);
                    testSpec.Jitter_pp_max =  Convert.ToDouble(row["jitter_pp_max"]);
                    testSpec.Jitter_rms_min = Convert.ToDouble(row["jitter_rms_min"]);
                    testSpec.Jitter_rms_max = Convert.ToDouble(row["jitter_rms_max"]);
                    testSpec.Crossing_min = Convert.ToDouble(row["crossing_min"]);
                    testSpec.Crossing_max = Convert.ToDouble(row["crossing_max"]);
                    testSpec.Fall_time_min = Convert.ToDouble(row["fall_time_min"]);
                    testSpec.Fall_time_max = Convert.ToDouble(row["fall_time_max"]);
                    testSpec.Rise_time_min = Convert.ToDouble(row["rise_time_min"]);
                    testSpec.Rise_time_max = Convert.ToDouble(row["rise_time_max"]);
                    testSpec.Er_min = Convert.ToDouble(row["er_min"]);
                    testSpec.Er_max = Convert.ToDouble(row["er_max"]);
                    testSpec.Mask_margin_min = Convert.ToDouble(row["mask_margin_min"]);
                    testSpec.Mask_margin_max = Convert.ToDouble(row["mask_margin_max"]);
                    testSpec.Itec_min = Convert.ToDouble(row["itec_min"]);
                    testSpec.Itec_max = Convert.ToDouble(row["itec_max"]);
                    testSpec.Vtec_min = Convert.ToDouble(row["vtec_min"]);
                    testSpec.Vtec_max = Convert.ToDouble(row["vtec_max"]);
                    testSpec.Ptec_min = Convert.ToDouble(row["ptec_min"]);
                    testSpec.Ptec_max = Convert.ToDouble(row["ptec_max"]);
                }
                return testSpec;

            }
            catch (Exception ex)
            {
                throw new Exception($"Fail to get spec for pn{pn}, {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public void SaveMesData(string post_json, string created_by)
        {
            string sql = $"insert into dbo.MES_TEST_DATA (post_json,create_by) values('{post_json}','{created_by}')";
            ExecuteRawSQL(sql);
        }
        #region Private Methods

        private DataSet ReadRawSQL(string Expression)
        {
            // try to open the connection
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to open SQL Server connection, {ex.Message}");
            }

            // try to execute query 
            try
            {
                using (var cmd = new SqlCommand(Expression, conn))
                {

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        private void ExecuteRawSQL(string Expression)
        {
            // try to open the connection
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to open SQL Server connection, {ex.Message}");
            }

            // try to execute query 
            try
            {
                using (var cmd = new SqlCommand(Expression, conn))
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
       
        #endregion
    }


}
