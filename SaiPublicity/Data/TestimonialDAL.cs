using SaiPublicity.Models;
using Microsoft.Data.SqlClient;

namespace SaiPublicity.Data
{
    public class TestimonialDAL : DataAccess
    {
        public TestimonialDAL(IConfiguration configuration) : base(configuration)
        {
        }

        // Get All Testimonials
        public List<TestimonialModel> GetAllTestim()
        {
            var testimList = new List<TestimonialModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                var query = "SELECT TestId, TestDate, TestName, TestLocation, TestDesc, TestimProfile FROM Testimonials WHERE DelMark = 0";
                var cmd = new SqlCommand(query, con);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        testimList.Add(new TestimonialModel
                        {
                            TestId = reader["TestId"] != DBNull.Value ? Convert.ToInt32(reader["TestId"]) : 0,
                            TestName = reader["TestName"]?.ToString() ?? string.Empty,
                            TestLocation = reader["TestLocation"]?.ToString() ?? string.Empty,
                            TestDate = reader["TestDate"] != DBNull.Value ? Convert.ToDateTime(reader["TestDate"]) : DateTime.MinValue,
                            TestDesc = reader["TestDesc"]?.ToString() ?? string.Empty,
                            TestimProfile = reader["TestimProfile"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return testimList;
        }

        // Create a New Testimonial
        public void AddTestim(TestimonialModel testim)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Testimonials (TestId, TestName, TestDate, TestDesc, TestLocation, TestimProfile, DelMark) " +
                               "VALUES (@TestId, @TestName, @TestDate, @TestDesc, @TestLocation, @TestimProfile, 0)";
                var cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@TestId", testim.TestId);
                cmd.Parameters.AddWithValue("@TestName", testim.TestName);
                cmd.Parameters.AddWithValue("@TestDate", testim.TestDate);
                cmd.Parameters.AddWithValue("@TestDesc", testim.TestDesc);
                cmd.Parameters.AddWithValue("@TestLocation", testim.TestLocation);
                cmd.Parameters.AddWithValue("@TestimProfile", testim.TestimProfile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DelMark", 0); // or 1 if deleted

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        // Get Testimonial By ID
        public TestimonialModel GetTestimById(int testId)
        {
            TestimonialModel testim = null;

            using (var con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Testimonials WHERE TestId = @TestId AND DelMark = 0";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TestId", testId);

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        testim = new TestimonialModel
                        {
                            TestId = Convert.ToInt32(reader["TestId"]),
                            TestDate = Convert.ToDateTime(reader["TestDate"]),
                            TestName = reader["TestName"].ToString(),
                            TestLocation = reader["TestLocation"].ToString(),
                            TestDesc = reader["TestDesc"].ToString(),
                            TestimProfile = reader["TestimProfile"].ToString()
                        };
                    }
                }
            }
            return testim;
        }

        // Update Testimonial
        public void UpdateTestim(TestimonialModel testim)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Testimonials 
                         SET TestName = @TestName, TestLocation = @TestLocation, 
                             TestDate = @TestDate, TestDesc = @TestDesc, 
                             TestimProfile = @TestimProfile 
                         WHERE TestId = @TestId";
                var cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@TestId", testim.TestId);
                cmd.Parameters.AddWithValue("@TestName", testim.TestName);
                cmd.Parameters.AddWithValue("@TestLocation", testim.TestLocation);
                cmd.Parameters.AddWithValue("@TestDate", testim.TestDate);
                cmd.Parameters.AddWithValue("@TestDesc", testim.TestDesc);
                cmd.Parameters.AddWithValue("@TestimProfile", testim.TestimProfile ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        // Soft Delete Testimonial
        public void SoftDeleteTestim(int TestId)
        {
            string query = "UPDATE Testimonials SET DelMark = 1 WHERE TestId = @TestId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@TestId", TestId)
            };

            int result = ExecuteQueryMVC(query, parameters);

            if (result == 0)
            {
                throw new Exception("Testimonial not deleted. Please check if the provided TestId exists.");
            }
        }

        public int GetTotalTestimonialsCount()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Testimonials WHERE DelMark = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

    }
}
