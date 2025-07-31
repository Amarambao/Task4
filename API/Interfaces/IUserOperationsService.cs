using API.Models.Dto;

namespace API.Interfaces
{
    public interface IUserOperationsService
    {
        public Task<IEnumerable<AppUserGetDto>> GetAllAsync();
        public Task<bool> CheckUserStatus(Guid userId);
        public Task ChangeUsersStatusAsync(ChangeUsersStatusDto dto);
        public Task DeleteUsersAsync(IEnumerable<Guid> userIds);
#if DEBUG
        public Task Generate100TestUsersAsync();
#endif
    }
}
