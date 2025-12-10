using SaiPublicity.Models;
using Microsoft.Data.SqlClient;

namespace SaiPublicity.Data
{
    public class VideoDAL : DataAccess
    {
        public VideoDAL(IConfiguration configuration) : base(configuration)
        {
        }

        // Get All Video
        public List<VideoModel> GetAllVideo()
        {
            var videoList = new List<VideoModel>();

            using (var con = new SqlConnection(_connectionString))
            {
                var query = "SELECT VideoId, VideoDate, VideoName, VideoLink FROM VideoLinks WHERE DelMark = 0";
                var cmd = new SqlCommand(query, con);
                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        videoList.Add(new VideoModel
                        {
                            VideoId = reader["VideoId"] != DBNull.Value ? Convert.ToInt32(reader["VideoId"]) : 0,
                            VideoName = reader["VideoName"]?.ToString() ?? string.Empty,
                            VideoDate = reader["VideoDate"] != DBNull.Value ? Convert.ToDateTime(reader["VideoDate"]) : DateTime.MinValue,
                            VideoLink = reader["VideoLink"]?.ToString() ?? string.Empty,
                        });
                    }
                }
            }
            return videoList;
        }

        // Create a Video
        public void AddVideo(VideoModel video)
        {
            int nextId = NextId("VideoLinks", "VideoId");

            string query = @"INSERT INTO VideoLinks 
            (VideoId, VideoDate, VideoName, VideoLink, DelMark) 
            VALUES (@VideoId, @VideoDate, @VideoName, @VideoLink, 0)";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@VideoId", nextId),
        new SqlParameter("@VideoDate", DateTime.Now), 
        new SqlParameter("@VideoName", video.VideoName),
        new SqlParameter("@VideoLink", video.VideoLink),
    };

            int result = ExecuteQueryMVC(query, parameters);

            if (result == 0)
            {
                throw new Exception("Video not inserted into database.");
            }
        }

        // Update video
        public void UpdateVideo(VideoModel video)
        {
            string query = @"UPDATE VideoLinks 
                     SET VideoName = @VideoName, 
                         VideoDate = @VideoDate, 
                         VideoLink = @VideoLink
                     WHERE VideoId = @VideoId";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@VideoId", video.VideoId),
        new SqlParameter("@VideoDate", DateTime.Now),
        new SqlParameter("@VideoName", video.VideoName),
        new SqlParameter("@VideoLink", video.VideoLink),
    };

            int result = ExecuteQueryMVC(query, parameters);

            if (result == 0)
            {
                throw new Exception("Video not updated. Please check if the provided VideoId exists.");
            }
        }



        // Get Single video by ID
        public VideoModel? GetVideoById(int id)
        {
            string query = "SELECT * FROM VideoLinks WHERE VideoId = @VideoId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@VideoId", id)
            };

            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new VideoModel
                            {
                                VideoId = Convert.ToInt32(reader["VideoId"]),
                                VideoName = reader["VideoName"].ToString(),
                                VideoDate = reader["VideoDate"] != DBNull.Value ? Convert.ToDateTime(reader["VideoDate"]) : DateTime.MinValue,
                                VideoLink = reader["VideoLink"]?.ToString() ?? string.Empty,
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }


        // Delete video
        public void SoftDeleteVideo(int VideoId)
        {
            string query = "UPDATE VideoLinks SET DelMark = 1 WHERE VideoId = @VideoId";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@VideoId", VideoId)
    };

            int result = ExecuteQueryMVC(query, parameters);

            if (result == 0)
            {
                throw new Exception("Video not deleted. Please check if the provided VideoId exists.");
            }
        }

        public int GetTotalVideoCount()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM VideoLinks WHERE DelMark = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }


    }
}
