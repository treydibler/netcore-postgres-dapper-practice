using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Users;
using TechDemo.Services;
using TechDemo.Tests.Common;
using Xunit;

namespace TechDemo.Tests.Extensions
{
    public static class AuthenticationHelper
    {
        public static HttpClient WithUserAuthentication(this HttpClient client, User user)
        {
            var response = GetMockedAuthentication(user).GetAwaiter().GetResult();
            Assert.True(response.Success);
            client.DefaultRequestHeaders.Add(@"Authorization", $"Bearer {response.Data}");
            return client;
        }

        public static async Task<ApiResponse<string>> GetMockedAuthentication(User user)
        {
            var context = new Mock<IDapperContext>();
            var hasher = new Mock<IPasswordHasher<User>>();


            context.Setup(x => 
                    x.ExecuteReadSingleAsync<User>("CALL proc_find_user_by_username(@username);", 
                        It.IsAny<NpgsqlParameter>()))
                .ReturnsAsync(user);
            
            
            hasher.Setup(x => 
                    x.VerifyHashedPassword(
                        It.IsAny<User>(), 
                        It.IsAny<string>(), 
                        It.IsAny<string>()
                    )
                )
                .Returns(PasswordVerificationResult.Success);
            
            var service = new UserService(
                new Mock<ILogger<UserService>>().Object, 
                hasher.Object,
                ApplicationConfiguration.Instance, 
                context.Object
            );

            return await service.SignIn(new SignInRequest()
            {
                Username = user.Username,
                Password = user.Credentials!
            });
        }
    }    
}

