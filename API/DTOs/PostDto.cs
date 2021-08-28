using System;

namespace API.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public int CreaterId { get; set; }
        public string CreaterPhotoUrl { get; set; }
        public string CreaterUsername { get; set; }
        public string CreaterKnownAs { get; set; }
        public string Content { get; set; }
        public DateTime PostCreated { get; set; }
    }
}