using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository
{
    Task CreatePostAsync(PostEntity post);
    Task UpdatePostAsync(PostEntity post);
    Task DeletePostAsync(Guid postId);


    Task<PostEntity> GetPostByIdAsync(Guid postId);
    Task<List<PostEntity>> ListAllAsync();

    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListByLikesAsync(int numberOfLikes);

    Task<List<PostEntity>> ListWithCommentsAsync();


}
