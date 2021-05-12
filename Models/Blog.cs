using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Update Date")]
        public DateTime? Updated { get; set; }

        public byte[] ImageData { get; set; }

        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Choose Blog Image")]
        public IFormFile ImageFile { get; set; }

        //Navigational Properties
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }

}
