using SaiPublicity.Models;
using Microsoft.Data.SqlClient;

namespace SaiPublicity.Data
{
    public class ProjectDAL : DataAccess
    {
        public ProjectDAL(IConfiguration configuration) : base(configuration) { }

        // Get Category List with Project Count
        public List<ProjectCategoryViewModel> GetProjectCategoryList()
        {
            var catList = new List<ProjectCategoryViewModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        c.ProjectCaegoryId,
                        c.ProjectCategory,
                        COUNT(p.ProjectId) AS TotalProjects
                    FROM ProjectCategory c
                    LEFT JOIN Projects p ON c.ProjectCaegoryId = p.ProjectCategoryId AND p.DelMark = 0
                    WHERE c.DelMark = 0
                    GROUP BY c.ProjectCaegoryId, c.ProjectCategory
                    ORDER BY c.ProjectCategory ASC";

                var cmd = new SqlCommand(query, con);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        catList.Add(new ProjectCategoryViewModel
                        {
                            ProjectCaegoryId = Convert.ToInt32(reader["ProjectCaegoryId"]),
                            ProjectCategory = reader["ProjectCategory"].ToString(),
                            TotalProjects = Convert.ToInt32(reader["TotalProjects"])
                        });
                    }
                }
            }

            return catList;
        }
        public ProjectCategoryViewModel GetProjectCategoryById(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = "SELECT ProjectCaegoryId, ProjectCategory FROM ProjectCategory WHERE ProjectCaegoryId=@Id AND DelMark=0";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ProjectCategoryViewModel
                        {
                            ProjectCaegoryId = Convert.ToInt32(reader["ProjectCaegoryId"]),
                            ProjectCategory = reader["ProjectCategory"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        // Get Projects by Category
        public List<ProjectModel> GetProjectsByCategory(int categoryId)
        {
            var list = new List<ProjectModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ProjectId, ProjectCategoryId, ProjectName, ProjectDate, ProjectImage 
                                 FROM Projects 
                                 WHERE ProjectCategoryId = @CatId AND DelMark = 0
                                 ORDER BY ProjectDate DESC";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CatId", categoryId);

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProjectModel
                        {
                            ProjectId = Convert.ToInt32(reader["ProjectId"]),
                            ProjectCategoryId = Convert.ToInt32(reader["ProjectCategoryId"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            ProjectDate = Convert.ToDateTime(reader["ProjectDate"]),
                            ProjectImage = reader["ProjectImage"]?.ToString()
                        });
                    }
                }
            }

            return list;
        }

        public void AddProject(ProjectModel model)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO Projects (ProjectName, ProjectImage, ProjectCategoryId, DelMark, ProjectDate)
            VALUES (@ProjectName, @ProjectImage, @ProjectCategoryId, 0, @ProjectDate)";

                var cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ProjectName", model.ProjectName);
                cmd.Parameters.AddWithValue("@ProjectImage", (object)model.ProjectImage ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectCategoryId", model.ProjectCategoryId);
                cmd.Parameters.AddWithValue("@ProjectDate", model.ProjectDate);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        // Get a single project
        public ProjectModel GetProjectById(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Projects WHERE ProjectId=@ProjectId";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProjectId", id);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ProjectModel
                        {
                            ProjectId = Convert.ToInt32(reader["ProjectId"]),
                            ProjectCategoryId = Convert.ToInt32(reader["ProjectCategoryId"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            ProjectImage = reader["ProjectImage"]?.ToString(),
                            ProjectDate = Convert.ToDateTime(reader["ProjectDate"]),
                            DelMark = Convert.ToBoolean(reader["DelMark"])
                        };
                    }
                }
            }
            return null;
        }

        // Update a project
        public void UpdateProject(ProjectModel model)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Projects SET 
                        ProjectName=@ProjectName, 
                        ProjectImage=@ProjectImage 
                        WHERE ProjectId=@ProjectId";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProjectName", model.ProjectName);
                cmd.Parameters.AddWithValue("@ProjectImage", (object)model.ProjectImage ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        // Delete Project
        public int DeleteProject(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Projects SET DelMark=1 WHERE ProjectId=@Id";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public List<ProjectViewModel> GetLatestProjectsByCategory()
        {
            var list = new List<ProjectViewModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT p.ProjectId, p.ProjectName, p.ProjectImage, p.ProjectCategoryId, c.ProjectCategory
                    FROM Projects p
                    INNER JOIN ProjectCategory c ON p.ProjectCategoryId = c.ProjectCaegoryId
                    WHERE p.DelMark = 0 AND c.DelMark = 0
                    AND p.ProjectId IN (
                        SELECT TOP 1 ProjectId 
                        FROM Projects 
                        WHERE ProjectCategoryId = p.ProjectCategoryId AND DelMark = 0 
                        ORDER BY ProjectDate DESC
                    )
                    ORDER BY p.ProjectDate DESC";

                var cmd = new SqlCommand(query, con);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProjectViewModel
                        {
                            ProjectId = Convert.ToInt32(reader["ProjectId"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            ProjectImage = reader["ProjectImage"]?.ToString(),
                            ProjectCategoryId = Convert.ToInt32(reader["ProjectCategoryId"]),
                            ProjectCategory = reader["ProjectCategory"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        // -------------------------------
        // Get all projects in a category
        // -------------------------------
        public List<ProjectViewModel> GetAllProjectsByCategory(int categoryId)
        {
            var list = new List<ProjectViewModel>();
            using (var con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT p.ProjectId, p.ProjectName, p.ProjectImage, p.ProjectCategoryId, c.ProjectCategory
                    FROM Projects p
                    INNER JOIN ProjectCategory c ON p.ProjectCategoryId = c.ProjectCaegoryId
                    WHERE p.ProjectCategoryId=@CategoryId AND p.DelMark=0 AND c.DelMark=0
                    ORDER BY p.ProjectDate DESC";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProjectViewModel
                        {
                            ProjectId = Convert.ToInt32(reader["ProjectId"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            ProjectImage = reader["ProjectImage"]?.ToString(),
                            ProjectCategoryId = Convert.ToInt32(reader["ProjectCategoryId"]),
                            ProjectCategory = reader["ProjectCategory"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        public int GetTotalProjectCount()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Projects WHERE DelMark = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
