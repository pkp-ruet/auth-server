namespace AuthServer.Services
{
    public interface IHashService
    {
        string GetPasswordHash(string password);
        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
