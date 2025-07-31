using API.Models.Dto;

namespace Identity.Interfaces
{
    public interface IIdentityUserService
    {
        public Task<string> LoginAsync(LoginDto dto);
        public Task<string> RegistrateUserAsync(RegistrationDto dto);
    }
}
