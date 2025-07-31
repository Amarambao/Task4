using API.Interfaces;
using API.Models.Entity;
using API.Services;
using Identity.Contexts;
using Identity.Interfaces;
using Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Identity.Settings
{
    public class DI
    {
        public static async Task Add(WebApplicationBuilder builder)
        {
            AddContext(builder.Services, builder.Configuration);
            IdentityPaswordSettings(builder.Services);
            AddService(builder.Services);
            AddSettings(builder.Services, builder.Configuration);
            AppBuild(builder);
        }

        private static void AddContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentitySQLContext>(
                    options => options.UseNpgsql(configuration.GetConnectionString("IdentityDatabase")));

            services.AddIdentity<AppUserEntity, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<IdentitySQLContext>()
                .AddApiEndpoints();
        }

        private static void IdentityPaswordSettings(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            });
        }

        private static void AddService(IServiceCollection services)
        {
            services.AddScoped<IIdentityUserService, IdentityUserService>();
            services.AddScoped<IUserOperationsService, UserOperationsService>();
            services.AddScoped<IJwtService, JwtService>();
        }

        private static void AddSettings(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddCookiePolicy(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                        ValidateLifetime = true,
                    };
                });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JSON Web Token based security",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        private static void AppBuild(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            /*if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }*/
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(builder => builder
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
