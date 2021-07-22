using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace BlogProject.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string BlogUserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        [DataType(DataType.Date)] public DateTime Created { get; set; }
        [DataType(DataType.Date)] public DateTime? Updated { get; set; }
        [DataType(DataType.Date)] public DateTime? Moderated { get; set; }
        [DataType(DataType.Date)] public DateTime? Deleted { get; set; }

        [Display(Name = "Blog Image")] public byte[] ImageData { get; set; }
        [Display(Name = "Image Type")] public string ContentType { get; set; }
        [NotMapped] public IFormFile Image { get; set; }
        
        public virtual BlogUser BlogUser { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}