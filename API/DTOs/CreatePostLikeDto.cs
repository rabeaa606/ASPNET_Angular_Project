namespace API.DTOs
{
    public class CreatePostLikeDto
    {
        public int SourcePostId { get; set; }
        public int LikedUserId { get; set; }
    }
}