using Identity.Models;

namespace API.Interfaces
{
    public interface IJwtService
    {
        public Task<string> GenerateJwtTokenAsync(AppUser user);
    }
}
