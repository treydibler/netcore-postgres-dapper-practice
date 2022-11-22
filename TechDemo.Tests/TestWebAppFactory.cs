using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using TechDemo.Data.Source;
using TechDemo.Services;
using TechDemo.Tests.Common;

namespace TechDemo.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(ApplicationConfiguration.Dictionary);
        });
        
        

        builder.ConfigureServices(services =>
        {
            services.AddTransient<IDapperContext, PostgresContext>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "test";
                options.DefaultSignInScheme = "test";
                options.DefaultSignOutScheme = "test";
                options.DefaultAuthenticateScheme = "test";
                options.DefaultChallengeScheme = "test";
            }).AddJwtBearer("test", options =>
            {

                var algorithm = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                algorithm.ImportECPrivateKey(Convert.FromBase64String(ApplicationConfiguration.Dictionary["Jwt"]!), out _);
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = false,
                    ValidateActor = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateSignatureLast = false,
                    ValidateIssuer = false,
                    ValidateTokenReplay = false,
                    ValidateWithLKG = false,
                    IssuerSigningKey = new ECDsaSecurityKey(algorithm)
                };

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
        });

        builder.ConfigureLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Trace);
            config.AddConsole();
            config.AddDebug();
        });
        
        return base.CreateHost(builder);
    }
}
