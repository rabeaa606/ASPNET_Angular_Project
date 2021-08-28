using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class PostRepository : IPostsRepository
    {
        private readonly DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context;
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);
        }

        public async Task<Post> GetPost(int id)
        {
            return await _context.Posts
            .Include(u => u.Creater)
            .Include(c => c.Creater.Photos)
            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<PostDto>> GetUsersPosts(PostParams postsParams)
        {

            var posts = _context.Posts
                       .OrderByDescending(m => m.PostCreated)
                       .AsQueryable();

            posts = postsParams.Core switch
            {
                "My" => posts.Where(m => m.Creater.UserName == postsParams.Username),
                "Others" => posts.Where(m => m.Creater.UserName != postsParams.Username),
            };


            var postsDto = posts.Select(post => new PostDto
            {
                CreaterId = post.CreaterId,
                CreaterUsername = post.Creater.UserName,
                CreaterKnownAs = post.Creater.KnownAs,
                CreaterPhotoUrl = post.Creater.Photos.FirstOrDefault(x => x.IsMain).Url,
                Content = post.Content,
                PostCreated = post.PostCreated,
                Id = post.Id
            });

            return await PagedList<PostDto>.CreateAsync(postsDto,
                      postsParams.PageNumber, postsParams.PageSize);
        }

    }
}