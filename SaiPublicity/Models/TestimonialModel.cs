using System.ComponentModel.DataAnnotations;

namespace SaiPublicity.Models
{
    public class TestimonialModel
    {
        public int TestId { get; set; }

        [Required]
        public DateTime TestDate { get; set; }

        [Required]
        public string TestName { get; set; }

        [Required]
        public string TestLocation { get; set; }

        [Required]
        public string TestDesc { get; set; }
        public IFormFile? ImageFile { get; set; }

        public string? TestimProfile { get; set; }  
        public bool RemoveImage { get; set; }


    }
}
