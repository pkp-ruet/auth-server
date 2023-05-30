namespace AuthServer.Services
{
    public class HashService: IHashService
    {
        public string GetPasswordHash(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return hash;
        }

        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
