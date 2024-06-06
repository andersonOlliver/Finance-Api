using Finance.Domain.Users;

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
}
