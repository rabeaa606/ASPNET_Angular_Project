using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{

    public class PostHub : Hub
    {

        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _unitOfWork;

        public PostHub(IUnitOfWork unitOfWork, IHubContext<PresenceHub> presenceHub,
          PresenceTracker tracker)
        {
            _unitOfWork = unitOfWork;
            _tracker = tracker;
            _presenceHub = presenceHub;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var postId = Int32.Parse(httpContext.Request.Query["postId"]);
            var Id = Int32.Parse(httpContext.Request.Query["userId"]);//userID

            var postLike = await _unitOfWork.PostLikesRepository.GetPostLike(postId, Id); //check if user liked this post
            if (postLike == null)
            {

                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(Id);
                var post = await _unitOfWork.PostRepository.GetPost(postId);
                user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.UserName);  //for including photos

                if (user == null) throw new HubException("Not found user");
                if (post == null) throw new HubException("Not found post");


                var like = new PostLike
                {
                    LikedUser = user,
                    LikedUserId = Id,
                    SourcePost = post,
                    SourcePostId = postId
                };


                _unitOfWork.PostLikesRepository.AddLike(like);

                if (await _unitOfWork.Complete())
                {

                    var photourl = user.Photos?.FirstOrDefault(p => p.IsMain).Url ?? "";

                    await Clients.Caller.SendAsync("NewLikeDone", new
                    {
                        Id = postId,
                        Username = user.UserName,
                        KnownAs = user.KnownAs,
                        PhotoUrl = photourl,
                    }
                    );

                    var connections = await _tracker.GetConnectionsForUser(post.CreaterUsername);
                    if (connections != null)
                    {
                        await _presenceHub.Clients.Clients(connections).SendAsync("NewPostLikeReceived",
                            new { username = user.UserName, knownAs = user.KnownAs });
                    }

                }
            }
            else
            {
                await Clients.Caller.SendAsync("NewLikeReject", "Post liked Rejected");

            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


    }
}