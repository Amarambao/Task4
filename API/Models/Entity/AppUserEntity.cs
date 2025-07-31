using API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace API.Models.Entity
{
    public class AppUserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime LastLoginTime { get; set; }

        public AppUserEntity() { }
        public AppUserEntity(RegistrationDto dto, string normalizeName, string normalizeEmail)
        {
            Id = Guid.NewGuid();
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            UserName = dto.UserName;
            Email = dto.Email;
            LastLoginTime = DateTime.UtcNow;
            AccessFailedCount = 0;
            NormalizedUserName = normalizeName;
            NormalizedEmail = normalizeEmail;
            EmailConfirmed = false;
            ConcurrencyStamp = Guid.NewGuid().ToString();
            PhoneNumber = string.Empty;
            PhoneNumberConfirmed = true;
            TwoFactorEnabled = false;
            IsBlocked = false;
            LockoutEnabled = true;
            LockoutEnd = DateTime.UtcNow;
        }
    }
}
