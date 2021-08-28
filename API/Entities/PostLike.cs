namespace API.Entities
{
    public class PostLike
    {
        public Post SourcePost { get; set; }
        public int SourcePostId { get; set; }

        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}