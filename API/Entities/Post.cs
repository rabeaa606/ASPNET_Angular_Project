using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public AppUser Creater { get; set; }
        public int CreaterId { get; set; }
        public string CreaterUsername { get; set; }
        public string Content { get; set; }
        public DateTime PostCreated { get; set; } = DateTime.UtcNow;
        public ICollection<PostLike> LikedByUsers { get; set; }


    }
}