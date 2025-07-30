using API.Interfaces;
using Identity.Interfaces;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IdentityUserService(IJwtService jwtService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            AppUser? user = null;
            
            if (dto.EmailOrUserName.Contains('@'))
                user = await _userManager.FindByEmailAsync(dto.EmailOrUserName);
            else
                user = await _userManager.FindByNameAsync(dto.EmailOrUserName);

            var passwordCheck = new SignInResult();
            
            if (user is not null)
            {
                if (user.IsBlocked)
                    throw new Exception("This user is blocked");

                passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            }

            if (user == null || !passwordCheck.Succeeded)
                throw new Exception("Wrong credentials");

            return await _jwtService.GenerateJwtTokenAsync(user!);
        }

        public async Task<string> RegistrateUserAsync(AppUserPostDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                throw new ArgumentNullException();

            var user = new AppUser()
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                LastLoginTime = DateTime.UtcNow,
                AccessFailedCount = 0,
                NormalizedUserName = _userManager.NormalizeName(dto.UserName),
                NormalizedEmail = _userManager.NormalizeEmail(dto.Email),
                EmailConfirmed = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = string.Empty,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                IsBlocked = false,
                LockoutEnabled = true,
                LockoutEnd = DateTime.UtcNow,
            };

            try
            {
                await _userManager.CreateAsync(user, dto.Password);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return await _jwtService.GenerateJwtTokenAsync(user);
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            var result = new AppUser[] { };
            try
            {
                result = await _userManager.Users.AsNoTracking().ToArrayAsync();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return result;
        }

        public async Task BlockUsersAsync(IEnumerable<Guid> userIds)
        {
            var users = _userManager.Users.Where(z => userIds.Contains(z.Id));

            foreach (var user in users)
            {
                user.IsBlocked = true;

                await _userManager.UpdateAsync(user);

                //remove jwt
            }
        }

        public async Task DeleteUsersAsync(IEnumerable<Guid> userIds)
        {
            var users = _userManager.Users.Where(z => userIds.Contains(z.Id));

            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);

                //remove jwt
            }
        }
    }
}
