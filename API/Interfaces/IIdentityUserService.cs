using Identity.Models;

namespace Identity.Interfaces
{
    public interface IIdentityUserService
    {
        public Task<string> LoginAsync(LoginDto dto);
        public Task<string> RegistrateUserAsync(AppUserPostDto dto);
        public Task<IEnumerable<AppUser>> GetAllAsync();
        public Task BlockUsersAsync(IEnumerable<Guid> userIds);
        public Task DeleteUsersAsync(IEnumerable<Guid> userIds);
    }
}
