using AuthServer.Models;

namespace AuthServer.Repositories
{
    public interface IRefreshTokenRepository
    {
        void Create(RefreshToken refreshToken);
        RefreshToken GetByToken(string token);
        void Delete(string id);
        void DeleteAll(string userId);
    }
}
