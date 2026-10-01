using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

internal class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    //public override void Add(User user)
    //{
    //    foreach (var role in user.Roles)
    //    {
    //        DbContext.Attach(role);
    //    }

    //    DbContext.Add(user);
    //}

    public Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<User>().FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
    }
}
