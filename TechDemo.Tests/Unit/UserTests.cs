using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using TechDemo.Core;
using TechDemo.Core.Constants;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Users;
using TechDemo.Services;
using TechDemo.Tests.Common;
using TechDemo.Tests.Extensions;
using TechDemo.Web;
using Xunit;

namespace TechDemo.Tests.Unit
{
    public class UserTests
    {
        [Fact]
        public async Task can_authenticate_good_user()
        {
            var context = new Mock<IDapperContext>();
            var hasher = new Mock<IPasswordHasher<User>>();

            var user = TestUsers.GOOD_USER();

            var result = await AuthenticationHelper.GetMockedAuthentication(user);
        
            Assert.True(string.IsNullOrEmpty(result.Error));
            Assert.False(string.IsNullOrEmpty(result.Data));
            Assert.True(result.Success);
        }
    }
}
