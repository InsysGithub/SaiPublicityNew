using SaiPublicity.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SaiPublicity.Data
{
    public class AdminDAL : DataAccess
    {
        public AdminDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public bool IsAdminValid(AdminLoginModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string query = "SELECT 1 FROM MasterAdmin WHERE DelMark = 0 AND UserName = @UserName AND UserPassword = @UserPassword ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserName", model.UserName);
                        cmd.Parameters.AddWithValue("@UserPassword", model.UserPassword);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            return dr.HasRows;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetAdminPassword(string username)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT UserPassword FROM MasterAdmin WHERE UserName = @UserName AND DelMark = 0";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        public bool UpdateAdminPassword(string username, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "UPDATE MasterAdmin SET UserPassword = @NewPassword WHERE UserName = @UserName AND DelMark = 0";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
