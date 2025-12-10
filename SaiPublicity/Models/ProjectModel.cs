namespace SaiPublicity.Models
{
    public class ProjectModel
    {
        public int ProjectId { get; set; }
        public int ProjectCategoryId { get; set; }
        public string ProjectName { get; set; }
        public DateTime ProjectDate { get; set; }
        public string ProjectImage { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool DelMark { get; set; }
        
    }
}
