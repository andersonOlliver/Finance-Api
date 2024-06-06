namespace Finance.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByIdAsync(Guid guid, CancellationToken cancellationToken = default);
}
