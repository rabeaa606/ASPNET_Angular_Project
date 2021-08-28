using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace API.Data
{
    public class PostLikesRepository : IPostLikesRepository
    {
        private readonly DataContext _context;
        public PostLikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<PostLike> GetPostLike(int sourcePostId, int likedUserId)
        {
            return await _context.PostsLikes.FindAsync(sourcePostId, likedUserId);
        }

        public async Task<PagedList<PostDto>> GetPostsLikedByUser(PostLikesParams postLikesParams)
        {


            var posts = _context.Posts
             .Where(m => m.Creater.UserName != postLikesParams.Username)
             .OrderByDescending(m => m.PostCreated)
            .AsQueryable();

            var postslikes = _context.PostsLikes.AsQueryable();

            postslikes = postslikes.Where(like => like.LikedUserId == postLikesParams.UserId);

            posts = postslikes.Select(like => like.SourcePost)
            .Where(u => u.CreaterId != postLikesParams.UserId)
             .OrderByDescending(m => m.PostCreated);


            var likedPosts = posts.Select(post => new PostDto
            {
                CreaterUsername = post.CreaterUsername,
                CreaterKnownAs = post.Creater.KnownAs,
                CreaterPhotoUrl = post.Creater.Photos.FirstOrDefault(p => p.IsMain).Url,
                CreaterId = post.CreaterId,
                Content = post.Content,
                PostCreated = post.PostCreated,
                Id = post.Id
            });

            return await PagedList<PostDto>.CreateAsync(likedPosts,
                postLikesParams.PageNumber, postLikesParams.PageSize);
        }
        public async Task<IEnumerable<PostLikeDto>> GetUsersLikedPost(int postId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var postslikes = _context.PostsLikes.AsQueryable();


            postslikes = postslikes.Where(like => like.SourcePostId == postId);
            users = postslikes.Select(like => like.LikedUser);


            var PostLikers = await users.Select(user => new PostLikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                Id = user.Id
            }).ToListAsync();

            return PostLikers;

        }

        public void AddLike(PostLike postLike)
        {
            _context.PostsLikes.Add(postLike);
        }
    }
}