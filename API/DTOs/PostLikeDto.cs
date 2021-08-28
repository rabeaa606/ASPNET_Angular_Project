namespace API.DTOs
{
    public class PostLikeDto
    {
        public int Id { get; set; }  //post id
        public string Username { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
    }
}