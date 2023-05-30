using AuthServer.Models;

namespace AuthServer.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);
        User CreateUser(User user);

    }
}
