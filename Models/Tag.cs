using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int BlogId { get; set; }
        public string BlogUserId { get; set; }
        
        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string Text { get; set; }

        public virtual Post Post { get; set; }
        public virtual BlogUser BlogUser { get; set; }
        public virtual Blog Blog { get; set; }
    }
}