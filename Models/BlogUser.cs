using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters.", MinimumLength = 2) ]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters.", MinimumLength = 2) ]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters.", MinimumLength = 2) ]
        public string DisplayName { get; set; }

        public byte[] Image { get; set; }
        public string ContentType { get; set; }

        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }

        [NotMapped] public string FullName => $"{FirstName} {LastName}";

        public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}