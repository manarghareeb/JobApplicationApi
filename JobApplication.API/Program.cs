using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace JobApplication.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddAutoMapper(config => { config.AddMaps(typeof(Application.AssemblyReference).Assembly); });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are missing.");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });
            builder.Services.AddAuthorization();

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??=
                        new Microsoft.OpenApi.Models.OpenApiComponents();

                    document.Components.SecuritySchemes ??=
                        new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSecurityScheme>();

                    document.Components.SecuritySchemes["Bearer"] =
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                            Name = "Authorization"
                        };

                    return Task.CompletedTask;
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                await IdentitySeeder.SeedRolesAsync(roleManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
