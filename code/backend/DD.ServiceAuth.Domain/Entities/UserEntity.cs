using AspNetCore.Identity.Mongo.Model;

namespace DD.ServiceAuth.Domain.Entities;

public class UserEntity : MongoUser
{
    public string DisplayName { get; init; } = string.Empty;
}
