using Core.Entities;
using Core.Interfaces;
using Infrastructure.Middleware;
using Infrastructure.OperationFilter;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Infrastructure.Helper
{
    public static class APIExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(
                    options =>
                    {
                        // Keep property names as-is (PascalCase)
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;

                        // Optional: Ignore null values in responses
                        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

                        // Optional: Handle enums as strings
                        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    }
                );
            var appSettings = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
            var userDetiails = configuration.GetSection("UserDetails");
            services.Configure<List<UserDetails>>(userDetiails);
            var reportDetails = configuration.GetSection("ReportDetails");
            services.Configure<ReportDetails>(reportDetails);
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            var authenticationSettings = appSettings.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(authenticationSettings.JwtSecret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,

                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var expiryTime = context.SecurityToken.ValidTo;
                        if (expiryTime < DateTime.UtcNow)
                        {
                            context.Fail("Token expired");
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<OptionalHeaderOperationFilter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter token: Bearer {your_token}"
                });
            });
            #region Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenServices, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISSRSService, SSRSService>();
            services.AddScoped<ISSRSClientFactory, SSRSClientFactory>();
            services.AddSingleton<IUploadQueue, UploadQueue>();
            services.AddSingleton<IUploadJobQueue, UploadJobQueue>();
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }
        public static void WebApplicationConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ResponseMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
