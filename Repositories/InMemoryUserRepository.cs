using AuthServer.Models;

namespace AuthServer.Repositories
{
    public class InMemoryUserRepository: IUserRepository
    {
        private readonly List<User> _users = new List<User>();
        public User GetUserByUsername(string username)
        {
            var userByUsername = _users.FirstOrDefault(o => o.Username == username);
            return userByUsername;
        }

        public User GetUserByEmail(string email)
        {
            var userByEmail = _users.FirstOrDefault(o => o.Email == email);
            return userByEmail;
        }

        public User CreateUser(User user)
        {
            _users.Add(user);
            return user;
        }
    }
}
