using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlogProject.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string Text { get; set; }
        public virtual Post Post { get; set; }
    }
}