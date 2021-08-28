using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]

    public class PostsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpGet("get-posts")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetUsersPosts([FromQuery] PostParams PostsParams)
        {
            var username = User.GetUsername();
            PostsParams.Username = username;

            var posts = await _unitOfWork.PostRepository.GetUsersPosts(PostsParams);

            Response.AddPaginationHeader(posts.CurrentPage,
                 posts.PageSize, posts.TotalCount, posts.TotalPages);

            return Ok(posts);

        }


        [HttpDelete("delete-post/{postId}")]
        public async Task<ActionResult> DeletePost(int postId)
        {
            var username = User.GetUsername();

            var post = await _unitOfWork.PostRepository.GetPost(postId);
            if (post == null)
                return BadRequest("Post Not Exist !!");

            if (post.CreaterUsername != username)
                return Unauthorized();

            _unitOfWork.PostRepository.DeletePost(post);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the post");
        }

        [HttpPost("add-post/{content}")]
        public async Task<ActionResult<UserDto>> AddPost(string content)
        {
            var username = User.GetUsername();
            var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            var newpost = new Post
            {
                CreaterUsername = postededUser.UserName.ToLower(),
                CreaterId = postededUser.Id,
                Creater = postededUser,
                Content = content
            };


            if (newpost.Creater.UserName != username)
                return Unauthorized();



            _unitOfWork.PostRepository.AddPost(newpost);


            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to add post");
        }


    }
}