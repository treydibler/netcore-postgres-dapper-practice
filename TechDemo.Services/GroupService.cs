using System.Net;
using Microsoft.Extensions.Logging;
using Npgsql;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Groups;

namespace TechDemo.Services
{
    public interface IGroupService
    {
        Task<ApiResponse<GroupTransfer>> AddGroup(ApiRequest<AddGroupRequest> request);
        
        Task<ApiResponse<GroupTransfer>> UpdateGroup(ApiRequest<UpdateGroupRequest> request);
        
        Task<ApiResponse<GroupTransfer>> DeleteGroup(ApiRequest<Guid> request);
        
        Task<ApiResponse> AddUser(ApiRequest<AddGroupUserRequest> request);
        
        Task<ApiResponse> RemoveUser(ApiRequest<AddGroupUserRequest> request);
        
    }
    
    public class GroupService : IGroupService
    {
        private readonly ILogger<GroupService> _log;
        private readonly IDapperContext _context;

        public GroupService(ILogger<GroupService> log, IDapperContext context)
        {
            _log = log;
            _context = context;
        }

        public async Task<ApiResponse<GroupTransfer>> AddGroup(ApiRequest<AddGroupRequest> request)
        {
            var response = new ApiResponse<GroupTransfer>();

            try
            {
                var group = new Group()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Data.Name
                };
                
                await _context.ExecuteNonQueryAsync("CALL proc_create_group(@group);", 
                        new NpgsqlParameter("@group", group)
                );

                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to create group.";
            }
            
            return response;
        }

        public async Task<ApiResponse<GroupTransfer>> UpdateGroup(ApiRequest<UpdateGroupRequest> request)
        {
            var response = new ApiResponse<GroupTransfer>();

            try
            {
                var group = new Group()
                {
                    Id = request.Data.Id,
                    Name = request.Data.Name
                };
                
                await _context.ExecuteNonQueryAsync("CALL proc_update_group(@group);", 
                    new NpgsqlParameter("@group", group)
                );

                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to update group.";
            }
            
            return response;        }

        public async Task<ApiResponse<GroupTransfer>> DeleteGroup(ApiRequest<Guid> request)
        {
            var response = new ApiResponse<GroupTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync("CALL proc_delete_group(@group_id);", 
                    new NpgsqlParameter("@group_id", request.Data)
                );
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to delete group.";
            }

            return response;
        }

        public async Task<ApiResponse> AddUser(ApiRequest<AddGroupUserRequest> request)
        {
            var response = new ApiResponse();

            try
            {
                NpgsqlParameter param = new ("@group_user", new GroupUser()
                {
                    Id = Guid.NewGuid(),
                    GroupId = request.Data.GroupId,
                    UserId = request.Data.UserId
                });
                param.Value = param;
                param.DataTypeName = "c_group_user";
                
                await _context.ExecuteNonQueryAsync("CALL proc_create_group_user(@group_user);", 
                    new NpgsqlParameter("@group_user", request.Data.GroupId)
                );
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to add user to group.";
            }
            return response;
        }
        
        public async Task<ApiResponse> RemoveUser(ApiRequest<AddGroupUserRequest> request)
        {
            var response = new ApiResponse();

            try
            {
                
                await _context.ExecuteNonQueryAsync("CALL proc_delete_group_user(@group_id, @user_id);", 
                    new NpgsqlParameter("@group_id", request.Data.GroupId),
                    new NpgsqlParameter("@user_id", request.Data.UserId)
                );
                
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to remove user from group.";            
            }
            
            return response;
        }
    }   
}