using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using TechDemo.Core;
using TechDemo.Core.Constants;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.Services;
using TechDemo.Tests;
using TechDemo.Tests.Common;
using TechDemo.Web;
using Xunit;

namespace TechDemo.Tests
{
    public class GroupTests : IClassFixture<CustomWebApplicationFactory<TechDemo.Web.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public GroupTests(CustomWebApplicationFactory<TechDemo.Web.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
    }   
}