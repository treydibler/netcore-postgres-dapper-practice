using TechDemo.Core;
using TechDemo.Core.Constants;
using TechDemo.DTO.Users;
using TechDemo.Services;
using TechDemo.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechDemo.Web.Controllers
{
    [ApiController]
    public sealed class UserController : BaseController
    {
        private readonly IUserService _users;
        
        public UserController(ILogger<BaseController> log, IUserService users) : base(log)
        {
            _users = users;
        }
        
        
        //TODO
        //Could wire in the default _userManager or
        // _signInManager for better control over password resets, user management, etc
        /// <summary>
        /// Authenticate and receive a JWT to access the API.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("[controller]/sign-in")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<string>))]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var response = await _users.SignIn(request);
            return Json(response);
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]   
        [HttpPost]
        [Route("[controller]/add")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<UserTransfer>))]
        public async Task<IActionResult> Add([FromBody] AddUserRequest request)
        {
            var authenticated = HttpContext.CreateAuthenticatedContext(request);
            return Json(await _users.Add(authenticated));
        }
        
        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]   
        [HttpDelete]
        [Route("[controller]/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<UserTransfer>))]
        public async Task<IActionResult> Delete([FromBody] Guid userId)
        {
            var authenticated = HttpContext.CreateAuthenticatedContext(userId);
            return Json(await _users.Delete(authenticated));
        }
        
        
        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<UserTransfer>))]
        [HttpPut]
        [Route("[controller]/update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest user)
        {
            var authenticated = HttpContext.CreateAuthenticatedContext(user);
            return Json(await _users.Update(authenticated));
        }
        
        
        //TODO Add pagination
        /// <summary>
        /// List all users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<UserTransfer>))]
        [HttpPut]
        [Route("[controller]/list")]
        public async Task<IActionResult> List()
        {
            var authenticated = HttpContext.CreateAuthenticatedContext();
            return Json(await _users.List(authenticated));
        }
    }   
}