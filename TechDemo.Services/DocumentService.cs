using System.Buffers.Text;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Mappers;
using TechDemo.DTO.Documents;
using TechDemo.DTO.Grants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using TechDemo.Data.Source;

namespace TechDemo.Services
{
    public interface IDocumentService
    {
        Task<ApiResponse<DocumentTransfer>> GetDocument(AuthenticatedApiRequest<Guid> request);
        Task<ApiResponse<DocumentTransfer>> AddDocument(AuthenticatedApiRequest<AddDocumentRequest> request);
        
        Task<ApiResponse<DocumentTransfer>> UpdateDocument(AuthenticatedApiRequest<UpdateDocumentRequest> request);
        
        Task<ApiResponse> DeleteDocument(AuthenticatedApiRequest<Guid> request);
        
        Task<ApiResponse> GrantUser(AuthenticatedApiRequest<AddDocumentUserRequest> request);
        
        Task<ApiResponse> GrantGroup(AuthenticatedApiRequest<AddDocumentGroupRequest> request);
        
        Task<ApiResponse> RevokeUser(AuthenticatedApiRequest<AddDocumentUserRequest> request);
        
        Task<ApiResponse> RevokeGroup(AuthenticatedApiRequest<AddDocumentGroupRequest> request);
    }

    public class DocumentService : IDocumentService
    {
        private readonly ILogger<IDocumentService> _log;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storage;
        private readonly IDapperContext _context;

        public DocumentService(ILogger<DocumentService> log, IConfiguration configuration, IStorageService storage, IDapperContext context)
        {
            _log = log;
            _configuration = configuration;
            _storage = storage;
            _context = context;
        }

        public async Task<ApiResponse<DocumentTransfer>> GetDocument(AuthenticatedApiRequest<Guid> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {

                var result = await _context.ExecuteReadSingleAsync<Document>(
                    @"CALL proc_get_document_by_id(@user_id, @document_id);",
                    new NpgsqlParameter("@user_id", $"{request.UserId}"),
                    new NpgsqlParameter("@document_id", $"{request.Data}")
                );

                if (result != null)
                {
                    response.Success = true;
                    response.Data = result.ToTransfer();
                }
            }
            catch (Exception e)
            {
                response.Error = "Something went wrong.";
                _log.LogError(e.Message + Environment.NewLine + e.InnerException?.Message);
            }

            return response;
        }

        public async Task<ApiResponse<DocumentTransfer>> AddDocument(AuthenticatedApiRequest<AddDocumentRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                var id = Guid.NewGuid();

                string location = Path.Combine(Path.GetDirectoryName($"{Environment.SpecialFolder.ApplicationData}")!, $"{id}");

                await using (var stream = request.Data.File.OpenReadStream())
                {
                                 
                    using (var md5 = MD5.Create())
                    {
                        md5.Initialize();
                        var hash = await md5.ComputeHashAsync(stream);
                        md5.Clear();

                        var existing = await _context.ExecuteReadSingleAsync<Document>(
                            $@"CALL proc_get_document_by_hash(@user_id, @hash);",
                            new NpgsqlParameter($@"@user_id", request.UserId),
                            new NpgsqlParameter($@"@hash", Convert.ToBase64String(hash))
                        );
                        
                        if (existing != null)
                        {
                            response.Success = true;
                            response.Data = existing.ToTransfer();
                            return response;
                        }
                    
                        //TODO Can gracefully handle a few errors here (storage full, disk failure, etc)
                        await _storage.Upload(location, stream);
                
                        var doc = new Document()
                        {
                            Id = id,
                            Name = request.Data.Name,
                            Category =  request.Data.Category,
                            Description = request.Data.Description,
                            Tags = request.Data.Tags,
                            Location = location,
                            Added = DateTime.UtcNow,
                            Hash = Convert.ToBase64String(hash)
                        };


                        NpgsqlParameter param = new ("@document", doc);
                        param.Value = doc;
                        param.DataTypeName = "c_document";
                        await _context.ExecuteNonQueryAsync($@"CALL proc_create_document(@document);", 
                                param
                            );
                        
                        response.Success = true;
                        response.Data = doc.ToTransfer();
                    }   
                }
            }
            catch (Exception e)
            {
                response.Error = "Something went wrong.";
                _log.LogError(e.Message + Environment.NewLine + e.InnerException?.Message);
            }

            return response;        }

        public async Task<ApiResponse<DocumentTransfer>> UpdateDocument(AuthenticatedApiRequest<UpdateDocumentRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                string location = Path.Combine(
                    _configuration["LocalStoragePath"] ?? throw new ApplicationException("no configured storage location"),
                    $"{request.Data.Id}"
                );

                await using (var stream = request.Data.File.OpenReadStream())
                {
                    using (var md5 = MD5.Create())
                    {
                        md5.Initialize();
                        var hash = await md5.ComputeHashAsync(stream);
                        md5.Clear();
                        

                        var existing = await _context.ExecuteReadSingleAsync<Document>(
                            @"CALL proc_get_document_by_hash(@user_id, @hash);",
                            new NpgsqlParameter("@user_id", request.UserId),
                            new NpgsqlParameter("@hash", hash)
                        );
                        
                        
                        if (existing != null)
                        {
                            response.Success = true;
                            response.Data = existing.ToTransfer();
                            return response;
                        }
                    
                        //TODO Can gracefully handle a few errors here (storage full, disk failure, etc)
                        await _storage.Upload(location, stream);
                
                        var doc = new Document()
                        {
                            Id = request.Data.Id,
                            Name = request.Data.Name,
                            Category = request.Data.Category,
                            Description = request.Data.Description,
                            Tags = request.Data.Tags,
                            Location = location,
                            Hash = Convert.ToBase64String(hash)
                        };
                

                        NpgsqlParameter param = new ("@document", doc);
                        param.Value = doc;
                        param.DataTypeName = "c_document";
                        
                        await _context.ExecuteNonQueryAsync("CALL proc_update_document(@user_id, @document);",
                            new NpgsqlParameter("@user_id", request.UserId),
                            param
                        );
                        
                        response.Success = true;
                        response.Data = doc.ToTransfer();
                    }
                
                    return response;   
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to update document.";
            }

            return response;
        }
        

        public async Task<ApiResponse> DeleteDocument(AuthenticatedApiRequest<Guid> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync(
                    "CALL proc_delete_document(@user_id, @document_id);",
                    new NpgsqlParameter("@user_id", request.UserId), 
                    new NpgsqlParameter("@document_id", request.Data)
                );
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to delete document.";
            }

            return response;
        }

        public async Task<ApiResponse> GrantUser(AuthenticatedApiRequest<AddDocumentUserRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync(
                    "CALL proc_grant_document_user(@user_id, @document_id);",
                    new NpgsqlParameter("@user_id", request.Data.UserId), 
                    new NpgsqlParameter("@document_id", request.Data.DocumentId)
                );
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to grant document access to user";
            }
            

            return response;
        }

        public async Task<ApiResponse> GrantGroup(AuthenticatedApiRequest<AddDocumentGroupRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync(
                    "CALL proc_grant_document_group(@group_id, @document_id);",
                    new NpgsqlParameter("@group_id", request.Data.GroupId), 
                    new NpgsqlParameter("@document_id", request.Data.DocumentId)
                );                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to grant group access to document.";
            }
            return response;
        }

        public async Task<ApiResponse> RevokeUser(AuthenticatedApiRequest<AddDocumentUserRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync(
                    "CALL proc_revoke_document_user(@user_id, @document_id);",
                    new NpgsqlParameter("@user_id", request.Data.UserId), 
                    new NpgsqlParameter("@document_id", request.Data.DocumentId)
                );
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to revoke user access to document.";
            }
            
            return response;
        }

        public async Task<ApiResponse> RevokeGroup(AuthenticatedApiRequest<AddDocumentGroupRequest> request)
        {
            var response = new ApiResponse<DocumentTransfer>();

            try
            {
                await _context.ExecuteNonQueryAsync(
                    "CALL proc_revoke_document_group(@group_id, @document_id);",
                    new NpgsqlParameter("@group_id", request.Data.GroupId), 
                    new NpgsqlParameter("@document_id", request.Data.DocumentId)
                );
                
                response.Success = true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                response.Error = "Failed to revoke group access to document.";
            }
            
            return response;

        }
    }
}