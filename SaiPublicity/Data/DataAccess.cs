using System.Data;
using Microsoft.Data.SqlClient;

namespace SaiPublicity.Data
{
    public class DataAccess
    {
        protected readonly string _connectionString;

        public DataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be connected");
        }

        public bool IsRecordExist(string strQuery)
        {
            try
            {
                bool rValue = false;

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            rValue = dr.HasRows;
                        }
                    }
                }

                return rValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int ExecuteQueryMVC(string strQuery, List<SqlParameter> parameters = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }

                        return cmd.ExecuteNonQuery(); // ✅ return the number of rows affected
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                throw;
            }
        }


        public int NextId(string tableName, string fieldName)
        {
            try
            {
                int retValue = 1;

                // Using statement ensures proper resource cleanup
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(" + fieldName + ") AS maxNo FROM " + tableName, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        // Using SqlDataReader with a 'using' block ensures it's disposed properly
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    if (dr["maxNo"] != DBNull.Value)
                                    {
                                        retValue = Convert.ToInt32(dr["maxNo"]) + 1;
                                    }
                                    else
                                    {
                                        retValue = 1;
                                    }
                                }
                            }
                            else
                            {
                                retValue = 1;
                            }
                        }
                    }
                }

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
