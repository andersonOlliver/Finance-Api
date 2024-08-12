using Finance.Domain.Abstracts;

namespace Finance.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, FirstName firstName, LastName lastName, Email email, DateTime createdOnUtc) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedOnUtc = createdOnUtc;
    }

    private User() { }

    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public string IdentityId { get; set; }

    public static User Create(FirstName firstName, LastName lastName, Email email, DateTime createdOnUtc)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, createdOnUtc);
        //user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        //user._roles.Add(Role.Registered);

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}
