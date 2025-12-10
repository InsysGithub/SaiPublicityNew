namespace SaiPublicity.Models
{
    public class ProjectPageViewModel
    {
        public int CategoryId { get; set; }
        public ProjectCategoryViewModel Category { get; set; }
        public List<ProjectModel> Projects { get; set; }
    }
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectImage { get; set; }
        public int ProjectCategoryId { get; set; }
        public string ProjectCategory { get; set; }
        public string ProjectSlug =>
    !string.IsNullOrEmpty(ProjectName)
        ? string.Join("-", ProjectName.ToLower().Split(" "))
            .Replace("@", "")
            .Replace("#", "")
            .Replace("$", "")
            .Replace("%", "")
            .Replace("&", "")
            .Replace("*", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("/", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "")
        : "";
    }

}
