namespace SaiPublicity.Models
{
    public class VideoModel
    {
        public int VideoId { get; set; }

        public string VideoName { get; set; }

        public string VideoLink { get; set; }

        public DateTime VideoDate { get; set; }


        public string EmbedLink
        {
            get
            {
                if (string.IsNullOrEmpty(VideoLink))
                    return string.Empty;

                string embedUrl = VideoLink.Trim();

                // Case 1 : Normal YouTube watch URL
                if (embedUrl.Contains("watch?v="))
                {
                    embedUrl = embedUrl.Replace("watch?v=", "embed/");
                }
                // Case 2 : youtu.be short link
                else if (embedUrl.Contains("youtu.be/"))
                {
                    var videoId = embedUrl.Split("youtu.be/")[1];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }
                // Case 3 : Shorts URL
                else if (embedUrl.Contains("youtube.com/shorts/"))
                {
                    var videoId = embedUrl.Split("youtube.com/shorts/")[1];
                    embedUrl = $"https://www.youtube.com/embed/{videoId}";
                }

                // Always ensure https://
                if (!embedUrl.StartsWith("http"))
                {
                    embedUrl = "https://" + embedUrl;
                }

                // Enable JavaScript API for play/pause control
                if (!embedUrl.Contains("enablejsapi=1"))
                {
                    embedUrl += embedUrl.Contains("?") ? "&enablejsapi=1" : "?enablejsapi=1";
                }

                return embedUrl;
            }
        }

    }
}
