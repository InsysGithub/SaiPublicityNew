using System.ComponentModel.DataAnnotations;

namespace SaiPublicity.Models
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage ="Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Password is required.")]
        public string UserPassword { get; set; }
    }
}
