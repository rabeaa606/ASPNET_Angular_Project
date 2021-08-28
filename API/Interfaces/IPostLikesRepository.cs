using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IPostLikesRepository
    {
        void AddLike(PostLike postLike);
        Task<PostLike> GetPostLike(int sourcePostId, int likedUserId);
        Task<PagedList<PostDto>> GetPostsLikedByUser(PostLikesParams postLikesParams);
        Task<IEnumerable<PostLikeDto>> GetUsersLikedPost(int postId);

    }
}