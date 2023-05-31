using AuthServer.Models;

namespace AuthServer.Repositories
{
    public class InMemoryRefreshTokenRepository: IRefreshTokenRepository
    {
        private readonly List<RefreshToken> refreshTokens = new List<RefreshToken>();
        public void Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid().ToString();
            refreshTokens.Add(refreshToken);
        }

        public RefreshToken GetByToken(string token)
        {
            RefreshToken refreshToken = refreshTokens.FirstOrDefault(o => o.Token == token);
            return refreshToken;
        }

        public void Delete(string id)
        {
            refreshTokens.RemoveAll(o => o.Id==id);
        }
    }
}
