using API.Models.Entity;

namespace API.Interfaces
{
    public interface IJwtService
    {
        public Task<string> GenerateJwtTokenAsync(AppUserEntity user);
    }
}
