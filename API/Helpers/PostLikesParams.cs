namespace API.Helpers
{
    public class PostLikesParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Username { get; set; }

    }
}