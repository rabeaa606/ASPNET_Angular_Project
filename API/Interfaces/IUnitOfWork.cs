using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        IPostsRepository PostRepository { get; }

        IPostLikesRepository PostLikesRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}