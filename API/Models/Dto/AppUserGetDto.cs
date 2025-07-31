using API.Models.Entity;

namespace API.Models.Dto
{
    public class AppUserGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsBlocked { get; set; }

        public AppUserGetDto() { }

        public AppUserGetDto(AppUserEntity user)
        {
            Id = user.Id;
            Name = $"{user.LastName} {user.FirstName}";
            UserName = user.UserName;
            Email = user.Email;
            LastLoginTime = user.LastLoginTime;
            IsBlocked = user.IsBlocked;
        }
    }
}
