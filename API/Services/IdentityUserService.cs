using API.Interfaces;
using API.Models.Dto;
using API.Models.Entity;
using Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly SignInManager<AppUserEntity> _signInManager;

        public IdentityUserService(IJwtService jwtService, UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            AppUserEntity? user = null;

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

            if (!passwordCheck.Succeeded)
                throw new Exception("Wrong credentials");

            var token = await _jwtService.GenerateJwtTokenAsync(user!);

            user.LastLoginTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return token;
        }

        public async Task<string> RegistrateUserAsync(RegistrationDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                throw new ArgumentNullException();

            var user = new AppUserEntity(dto, _userManager.NormalizeName(dto.UserName), _userManager.NormalizeEmail(dto.Email));

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                foreach (var err in result.Errors)
                    throw new Exception(err.Description);

            return await _jwtService.GenerateJwtTokenAsync(user);
        }
    }
}
