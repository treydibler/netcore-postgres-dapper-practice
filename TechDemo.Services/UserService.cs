using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.DTO.Users;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using TechDemo.Data.Mappers;
using TechDemo.Data.Source;

namespace TechDemo.Services
{
    public class PasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password)
        {
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(password, 
                user.Id.ToByteArray(),
                KeyDerivationPrf.HMACSHA256, 
                100_000, 
                256));
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
    
    public interface IUserService
    {
        Task<ApiResponse<string>> SignIn(SignInRequest request);
        Task<ApiResponse<UserTransfer>> Add(AuthenticatedApiRequest<AddUserRequest> request);
        Task<ApiResponse<UserTransfer>> Update(AuthenticatedApiRequest<UpdateUserRequest> request);
        Task<ApiResponse> Delete(AuthenticatedApiRequest<Guid> request);
        Task<ApiResponse<List<UserTransfer>>> List(AuthenticatedApiRequest request);
    }
    
    
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _log;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IConfiguration _configuration;
        private readonly IDapperContext _context;

        public static readonly TokenValidationParameters TOKEN_VALIDATION_PARAMETERS = new()
        {
            ClockSkew = TimeSpan.FromSeconds(5),
            
            //TODO set these true for production deployment; can DI from config but for the sake of demo this is easier (albeit insecure)
            // ValidateAudience = true,
            // ValidateLifetime = true,
            // ValidateIssuer = true,
            // ValidateTokenReplay = true,
            // RequireExpirationTime = true,
            RequireSignedTokens = true,
        };

        public UserService(ILogger<UserService> log, IPasswordHasher<User> hasher, IConfiguration configuration, IDapperContext context)
        {
            _log = log;
            _hasher = hasher;
            _configuration = configuration;
            _context = context;
        }
        
        //TODO A proper IdP or separate SSO service (oidc callback to /sign-in  ) would be better
        public async Task<ApiResponse<string>> SignIn(SignInRequest request)
        {
            var response = new ApiResponse<string>();

            var user = await _context.ExecuteReadSingleAsync<User>("CALL proc_find_user_by_username(@username);",
                    new NpgsqlParameter("@username", request.Username)
                );

            if (user == null || user.Credentials == null)
            {
                response.Error = "User not found.";
            }
            else
            {
                
                
                var result = _hasher.VerifyHashedPassword(user, user.Credentials, request.Password);

                if ((int) result > 0)
                {
                    
                    var algorithm = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                    algorithm.ImportECPrivateKey(Convert.FromBase64String(_configuration["Jwt"]!), out _);
                    var handler = new JsonWebTokenHandler();
                    var descriptor = new SecurityTokenDescriptor()
                    {
                        SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(algorithm), "ES256", "RS256" ),
                        Claims = new Dictionary<string, object>
                        (
                            new List<KeyValuePair<string, object>>(new []
                        {
                            new KeyValuePair<string, object>(ClaimTypes.Role, user.Role),
                            new KeyValuePair<string, object>(ClaimTypes.NameIdentifier, user.Id)
                        }))
                    };
                    response.Data = handler.CreateToken(descriptor);
                    response.Success = true;
                }
                else
                {
                    response.Error = "Invalid credentials.";
                }
            }

            return response;
        }
        
        public async Task<ApiResponse<UserTransfer>> Add(AuthenticatedApiRequest<AddUserRequest> request)
        {
            var response = new ApiResponse<UserTransfer>();

            try
            {
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Username = request.Data.Username,
                    Role = request.Data.Role
                };
                
                string password = _hasher.HashPassword(user,  request.Data.Password);
                user.Credentials = password;
                
                var result = await _context.ExecuteReadSingleAsync<User>("CALL proc_create_user(@user);", 
                        new NpgsqlParameter("@user", user)
                    );
                
                response.Success = true;
                response.Data = result?.ToTransfer();
            }
            /*
             * TODO can extend flow with better exception handling or more in depth defensive coding
             * (exception driven code can be pretty punishing) to handle failure (ie user exists, db exception, etc)
            */ 
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to add user.";
            }

            return response;
        }

        public async Task<ApiResponse<UserTransfer>> Update(AuthenticatedApiRequest<UpdateUserRequest> request)
        {
            var response = new ApiResponse<UserTransfer>();

            try
            {
                var existing = await _context.ExecuteReadSingleAsync<User>(
                    "CALL proc_find_user_by_id(@user_id);", 
                        new NpgsqlParameter("@user_id", request.Data)
                );

                if (existing == null)
                {
                    response.Error = "User not found.";
                    return response;
                }
                else
                {
                    var user = new User()
                    {
                        Id = existing.Id,
                        Credentials = existing.Credentials,
                        Role = request.Data.Role,
                    };
                
                    await _context.ExecuteNonQueryAsync("CALL proc_update_user(@user);", 
                        new NpgsqlParameter("@user", user)
                    );
                    response.Success = true;
                }
            }
            /*
             * TODO can extend flow with better exception handling or more in depth defensive coding
             * (exception driven code can be pretty punishing) to handle failure (ie user exists, db exception, etc)
            */ 
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to update user.";
            }

            return response;        
        }

        public async Task<ApiResponse> Delete(AuthenticatedApiRequest<Guid> request)
        {
            var response = new ApiResponse<UserTransfer>();

            try
            {
                
                await _context.ExecuteNonQueryAsync("CALL proc_delete_user_by_id(@user_id);", 
                        new NpgsqlParameter("@user_id", request.Data)
                );
                response.Success = true;
            }
            /*
             * TODO can extend flow with better exception handling or more in depth defensive coding
             * (exception driven code can be pretty punishing) to handle failure (ie user exists, db exception, etc)
            */ 
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to delete user.";
            }

            return response;              
        }

        public async Task<ApiResponse<List<UserTransfer>>> List(AuthenticatedApiRequest request)
        {
            var response = new ApiResponse<List<UserTransfer>>();

            try
            {
                var users = await _context.ExecuteReaderAsync<User>($@"CALL proc_list_users();");

                response.Success = true;
                response.Data = users.ToTransfer();
            }
            /*
             * TODO can extend flow with better exception handling or more in depth defensive coding
             * (exception driven code can be pretty punishing) to handle failure (ie user exists, db exception, etc)
            */ 
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to list users.";
            }

            return response;         
        }
    }   
}