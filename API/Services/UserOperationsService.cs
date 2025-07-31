using API.Interfaces;
using API.Models.Dto;
using API.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserOperationsService : IUserOperationsService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        public UserOperationsService(UserManager<AppUserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<AppUserGetDto>> GetAllAsync()
        {
            var result = await _userManager.Users.AsNoTracking().ToArrayAsync();

            return result.Select(user => new AppUserGetDto(user));
        }

        public async Task<bool> CheckUserStatus(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(z => z.Id == userId);

            if (user is not null)
                return user.IsBlocked;

            return false;
        }

        public async Task ChangeUsersStatusAsync(ChangeUsersStatusDto dto)
        {
            var users = await _userManager.Users.Where(z => dto.UserIds.Contains(z.Id)).ToListAsync();

            foreach (var user in users)
            {
                if (user.IsBlocked == dto.RequestedStatus)
                    continue;

                user.IsBlocked = dto.RequestedStatus;

                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteUsersAsync(IEnumerable<Guid> userIds)
        {
            var users = await _userManager.Users.Where(z => userIds.Contains(z.Id)).ToListAsync();

            foreach (var user in users)
                await _userManager.DeleteAsync(user);
        }

#if DEBUG
        public async Task Generate100TestUsersAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                var dto = new RegistrationDto()
                {
                    FirstName = $"Name{i}",
                    LastName = $"LastName{i}",
                    UserName = $"USERNAME{i}",
                    Email = $"{i}@test.com",
                    Password = "1"
                };
                var user = new AppUserEntity(dto, _userManager.NormalizeName(dto.UserName), _userManager.NormalizeEmail(dto.Email));

                await _userManager.CreateAsync(user, dto.Password);
            }
        }
#endif
    }
}
