using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using System;

namespace API.Controllers
{
    [Authorize]

    public class PostLikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostLikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("users-liked-posts/{postId}")]
        public async Task<ActionResult<IEnumerable<PostLikeDto>>> GetUsersLikedPost(int postId)
        {

            return Ok(await _unitOfWork.PostLikesRepository.GetUsersLikedPost(postId));
        }

        [HttpGet("user-likes")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsLikedByUser([FromQuery] PostLikesParams postLikesParams)
        {
            var posts = await _unitOfWork.PostLikesRepository.GetPostsLikedByUser(postLikesParams);

            Response.AddPaginationHeader(posts.CurrentPage,
                posts.PageSize, posts.TotalCount, posts.TotalPages);

            return Ok(posts);
        }

    }
}