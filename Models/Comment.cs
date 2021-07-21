using System;
using System.ComponentModel.DataAnnotations;
using BlogProject.Enums;

namespace BlogProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string BlogUserId { get; set; }
        public string ModeratorId { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string Body { get; set; }

        [Required, Display(Name="Moderated Comment")]
        [StringLength(300, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string ModeratedBody { get; set; }
        public ModerationType ModerationType { get; set; }
        
        [StringLength(300, ErrorMessage = "The {0} must be greater than {2} but less than {1} characters long.", MinimumLength = 2)]
        public string EditedBody { get; set; }

        [DataType(DataType.Date), Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date), Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        [DataType(DataType.Date)] public DateTime? Moderated { get; set; }
        [DataType(DataType.Date)] public DateTime? Deleted { get; set; }

        public virtual Post Post { get; set; }
        public virtual BlogUser BlogUser { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}