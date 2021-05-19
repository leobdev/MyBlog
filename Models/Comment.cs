using MyBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public string AuthorId { get; set; }

        public string ModeratorId { get; set; }

        public string Body { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Moderated { get; set; }

        public string ModeratedBody { get; set; }

        public ModerationType ModerationType { get; set; }

        public virtual Post Post { get; set; }

        public virtual BlogUser Author { get; set; }

        public virtual BlogUser Moderator { get; set; }

        [NotMapped]
        public virtual Post Slug { get; set; }

    }
}
