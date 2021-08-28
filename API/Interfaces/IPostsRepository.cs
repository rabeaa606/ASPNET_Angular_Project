using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IPostsRepository
    {
        Task<PagedList<PostDto>> GetUsersPosts(PostParams postsParams);
        void AddPost(Post post);
        void DeletePost(Post post);
        Task<Post> GetPost(int id);

    }
}