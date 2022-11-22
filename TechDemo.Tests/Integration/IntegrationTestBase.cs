using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace TechDemo.Tests.Integration
{
    public abstract class IntegrationTestBase  : IClassFixture<CustomWebApplicationFactory<TechDemo.Web.Startup>>
    {
        protected readonly CustomWebApplicationFactory<TechDemo.Web.Startup> ApplicationFactory;
        protected readonly HttpClient HttpClient;
        protected readonly JsonSerializerOptions JsonSerializerOptions;

        protected IntegrationTestBase(CustomWebApplicationFactory<TechDemo.Web.Startup> factory)
        {
            ApplicationFactory = factory;
            HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            
            
        }
    }   
}