using System.Text.Json.Serialization;
using TechDemo.Core.Constants;
using TechDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Users;


namespace TechDemo.Web;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            
            options.IncludeXmlComments(string.Format(@"{0}/TechDemo.Web.xml", System.AppDomain.CurrentDomain.BaseDirectory));
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TechDemo.Web",
                Description = "A simple .NET Postgres backend",
                Contact = new OpenApiContact()
                {
                    Email = "treydibler@gmail.com"
                },
            });
        });

        
        //TODO This would be much better bound to an external SSO/IDP Service
        //This will do for the sake of demonstration.
        services.AddAuthorization(options =>
        {
            string scheme = "jwt";
            
            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.AuthenticationSchemes = new string[] { scheme };

            policyBuilder.RequireAuthenticatedUser();

            options.AddPolicy(AuthorizationRole.User, policy =>
            {
                policy.AuthenticationSchemes = new string[] { scheme };
                policy.RequireRole(AuthorizationRole.User);
            });

            options.AddPolicy(AuthorizationRole.Manager, policy =>
            {
                policy.AuthenticationSchemes = new string[] { scheme };
                policy.RequireRole(AuthorizationRole.Manager);
            });
                
            options.AddPolicy(AuthorizationRole.Administrator, policy =>
            {
                policy.AuthenticationSchemes = new string[] { scheme };
                policy.RequireRole(AuthorizationRole.Administrator);
            });

            options.DefaultPolicy = policyBuilder.Build();
        });

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "jwt";
            options.DefaultSignInScheme = "jwt";
            options.DefaultSignOutScheme = "jwt";
            options.DefaultAuthenticateScheme = "jwt";
            options.DefaultChallengeScheme = "jwt";
        }).AddJwtBearer("jwt", options =>
        {
#if DEBUG
            options.IncludeErrorDetails = true;
#endif
            options.Authority = configuration["Jwt:Authority"];
            options.Audience = configuration["Jwt:ClientId"];
            
            options.TokenValidationParameters = UserService.TOKEN_VALIDATION_PARAMETERS;
            options.SaveToken = true;

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub/")))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };            
        });

        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 100_000;
        });

        services.AddControllers();
        

        RegisterCustom(services, configuration);
    }
    
    public static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        
#if DEBUG
        app.UseDeveloperExceptionPage();
#else   
        app.UseExceptionHandler("/Error");
#endif            
            

#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif

#if RELEASE        
        app.UseHsts();
#endif

        app.UseForwardedHeaders();
        
        app.MapSwagger();
        
        app.MapControllers();
        
        app.UseAuthentication();
        
        app.UseAuthorization();
        
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
        });
    }

    public static void RegisterCustom(IServiceCollection services, IConfiguration configuration)
    {
        //Web
        services.AddScoped<IPasswordHasher<User>, PasswordHasher>();
        
        //Data

        //Services
        services.AddSingleton<IStorageService, LocalStorageService>();
        services.AddTransient<IDocumentService, DocumentService>();
        services.AddTransient<IGroupService, GroupService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IDapperContext, PostgresContext>();

    }
}