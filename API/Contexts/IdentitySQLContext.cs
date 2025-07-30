using Identity.Configuration;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Contexts
{
    internal class IdentitySQLContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public IdentitySQLContext() { }

        public IdentitySQLContext(DbContextOptions<IdentitySQLContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AppUserConfig());
        }
    }
}
