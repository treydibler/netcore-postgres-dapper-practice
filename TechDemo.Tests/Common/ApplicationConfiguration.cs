using Microsoft.Extensions.Configuration;

namespace TechDemo.Tests.Common
{
    public static class ApplicationConfiguration
    {
        public readonly static Dictionary<string, string?> Dictionary = new()
        {
            {
                "Postgres:ConnectionString", 
                "User ID=sa;Password=postgres;Host=localhost;Port=5432;Database=postgres;Pooling=true;"
            },
            {
                "Default:Username", 
                "admin123"
            },
            { 
                "Default:Password", 
                "default123" 
            },
            { 
                "LocalStoragePath", 
                "~/Documents/" 
            },
            {
                "Jwt",
                "MHcCAQEEIK0oqUJ4G4OqJ8+tVxi0ygV9MAgYFm8BOGbpAhbnWoPKoAoGCCqGSM49\nAwEHoUQDQgAEbgjyKuYjDoNfMMSbcdcgwlPZQqKATpkRtMEgHc34SmiQtFTwqpTA\nUz/yp7Wfx/Zmh2PxEK/5q1R2sUVG+P5xNw=="
            }
        };

        public static IConfiguration Instance => new ConfigurationBuilder()
            .AddInMemoryCollection(
                    Dictionary
                )
                .Build();
    }
}

