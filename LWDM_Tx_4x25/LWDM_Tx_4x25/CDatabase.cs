using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFTest_Tx_PAM4_50G
{
    public class CDatabase
    {
        public SqlConnection conn;
        public ConfigManagement cfg = new ConfigManagement();
        SqlCommand cmd = new SqlCommand();
        public CDatabase()
        {
            string connectString = cfg.DBConnectString;
            conn = new SqlConnection(connectString);
        }

        public void SaveTestData(CTestDataCommon testData)
        {
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
                cmd.Connection = conn;
                cmd.Transaction = conn.BeginTransaction();
                var strSql = $"insert into dbo.tx_rftest_pam4_common(spec_id,sn,operator,test_station,test_date,vcc1,vcc2,ppg_data_rate,ppg_channel,ppg_pattern,pre_cursor, main_cusor, post_cusor, inner_1, inner_2, tx_tec, tx_vb, tx_vea, tx_vg, ld_bias,pf) values({testData.Spec_id}, '{testData.SN}','{testData.Operator}','{SystemInformation.ComputerName}','{testData.Test_Date}','{testData.Vcc1}','{testData.Vcc2}','{testData.Ppg_data_rate}','{testData.Ppg_channel}','{testData.Ppg_pattern}','{testData.Pre_cursor}','{testData.Main_cursor}','{testData.Post_cursor}','{testData.Inner_1}','{testData.Inner_2}' ,'{testData.TxTEC}','{testData.TxVB}','{testData.TxVEA}','{testData.TxVG}','{testData.LDBias}','{testData.Pf}')";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSql;
                cmd.ExecuteNonQuery();

                strSql = $"select max(id) as id from dbo.tx_rftest_pam4_common";
                cmd.CommandText = strSql;
              
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    testData.ID = Convert.ToInt32(reader["id"]);
                }
                reader.Close();
                for (int i = 0; i < testData.lstTestData.Count; i++)
                {
                    CTestData data = testData.lstTestData[i];
                    data.Common_id = testData.ID;
                    strSql = $"insert into dbo.tx_rftest_pam4_ret(common_id, temp_case, test_start_time, test_end_time,io,ea_voltage,temp_coc,cwl,smsr,tdecq, outer_er, outer_oma,aop,linearity,pf) values({data.Common_id}, '{data.Temp_case}','{data.Test_start_time}','{data.Test_end_time}','{data.Io}','{data.EA_Voltage}','{data.Temp_Coc}','{data.CWL}','{data.SMSR}','{data.TDECQ}','{data.OuterER}','{data.OuterOMA}','{data.Lanch_power}','{data.Linearity}','{testData.Pf}')";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strSql;
                    cmd.ExecuteNonQuery();
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
                string sql = string.Format("select distinct pn from dbo.spec_tx_rftest_pam4");

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
                string sql = string.Format($"select * from dbo.spec_tx_rftest_pam4 where pn='{pn}'and id=(select Max(id) from dbo.spec_tx_rftest_pam4 where pn='{pn}')");

                DataSet ds = ReadRawSQL(sql);
                DataTable dt = ds.Tables[0];
                if (ds.Tables != null && ds.Tables.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    testSpec.ID = Convert.ToInt32(row["id"]);
                    testSpec.CWL_Min = Convert.ToDouble(row["cwl_min"]);
                    testSpec.CWL_Max = Convert.ToDouble(row["cwl_max"]);
                    testSpec.SMSR_Min = Convert.ToDouble(row["smsr_min"]);
                    testSpec.SMSR_Max = Convert.ToDouble(row["smsr_max"]);
                    testSpec.TDECQ_Min =Convert.ToDouble(row["tdecq_min"]);
                    testSpec.TDECQ_Max= Convert.ToDouble(row["tdecq_max"]);
                    testSpec.Outer_OMA_Min= Convert.ToDouble(row["outer_oma_min"]);
                    testSpec.Outer_OMA_Max = Convert.ToDouble(row["outer_oma_max"]);
                    testSpec.Outer_ER_Min = Convert.ToDouble(row["outer_er_min"]);
                    testSpec.Outer_ER_Max = Convert.ToDouble(row["outer_er_max"]);
                    testSpec.Lanch_Power_Min = Convert.ToDouble(row["lanch_power_min"]);
                    testSpec.Lanch_Power_Max = Convert.ToDouble(row["lanch_power_max"]);
                    testSpec.Linearity_Min = Convert.ToDouble(row["linearity_min"]);
                    testSpec.Linearity_Max = Convert.ToDouble(row["linearity_max"]);
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
