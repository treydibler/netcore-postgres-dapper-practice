using TechDemo.Core.Constants;
using TechDemo.DTO.Groups;
using TechDemo.Services;
using TechDemo.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDemo.Core;

namespace TechDemo.Web.Controllers
{
    [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
    [ApiController]
    public sealed class GroupController : BaseController
    {
        private readonly IGroupService _groups;
        
        public GroupController(ILogger<GroupController> log, IGroupService groups) : base(log)
        {
            _groups = groups;
        }
        
        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<GroupTransfer>))]
        [Route("[controller]/add")]
        public async Task<IActionResult> Add([FromBody] AddGroupRequest request)
        {
            var result = await _groups.AddGroup(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
                
        /// <summary>
        /// Update an existing group.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("[controller]/update")]
        public async Task<IActionResult> Update([FromBody] UpdateGroupRequest request)
        {
            var result = await _groups.UpdateGroup(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);        
        }
        
        
        /// <summary>
        /// Delete an existing group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<GroupTransfer>))]
        [Route("[controller]/{groupId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid groupId)
        {
            var result = await _groups.DeleteGroup(HttpContext.CreateAuthenticatedContext(groupId));
            return Json(result);        
        }

        
        /// <summary>
        /// Add a user to a group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<GroupTransfer>))]

        [Route("[controller]/{groupId:guid}/user")]
        public async Task<IActionResult> AddUser([FromRoute] Guid groupId, [FromBody] AddGroupUserRequest request)
        {
            request.GroupId = groupId;
            var result = await _groups.AddUser(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        /// <summary>
        /// Remove a user from a group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("[controller]/{groupId:guid}/user/{userId:guid}")]
        public async Task<IActionResult> RemoverUser([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var result = await _groups.RemoveUser(HttpContext.CreateAuthenticatedContext(new AddGroupUserRequest()
            {
                GroupId = groupId, UserId = userId
            }));
            return Json(result);
        }
    }    
}