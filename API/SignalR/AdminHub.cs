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
    public class AdminHub : Hub
    {
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _unitOfWork;

        public AdminHub(IUnitOfWork unitOfWork, IHubContext<PresenceHub> presenceHub,
          PresenceTracker tracker)
        {
            _unitOfWork = unitOfWork;
            _tracker = tracker;
            _presenceHub = presenceHub;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var Id = Int32.Parse(httpContext.Request.Query["postId"]);
            var post = await _unitOfWork.PostRepository.GetPost(Id);
            if (post == null) throw new HubException("Not found post");


            if (post != null)
            {
                _unitOfWork.PostRepository.DeletePost(post);


                if (await _unitOfWork.Complete())
                {
                    await Clients.Caller.SendAsync("PostDelted", new { });

                    var connections = await _tracker.GetConnectionsForUser(post.CreaterUsername);
                    if (connections != null)
                    {
                        await _presenceHub.Clients.Clients(connections).SendAsync("AdminDeletedPost",
                               new { });
                    }

                }
            }
            else
            {
                await Clients.Caller.SendAsync("PostDeltedReject", new { });

            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
