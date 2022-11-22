using System.Diagnostics;
using System.Net;
using TechDemo.Core.Constants;
using TechDemo.DTO.Documents;
using TechDemo.DTO.Grants;
using TechDemo.DTO.Groups;
using TechDemo.Services;
using TechDemo.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Net.Http.Headers;
using TechDemo.Core;

namespace TechDemo.Web.Controllers
{
    /// <summary>
    /// /Documents/
    /// </summary>
    [ApiController]
    public sealed class DocumentController : BaseController
    {
        private readonly IDocumentService _documents;
        
        public DocumentController(ILogger<DocumentController> log, IDocumentService documents) : base(log)
        {
            _documents = documents;
        }

        
        /// <summary>
        /// Upload a new document.
        /// </summary>
        /// <param name="document">The document and associated metadata to be added.</param>
        [HttpPost]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}, " +
                           $"{AuthorizationRole.Manager}")]
        [Route("[controller]/upload")]
        [RequestSizeLimit(52428800)] //TODO Increase from 50mB
        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<DocumentTransfer>))]
        public async Task<IActionResult> Upload([FromForm] AddDocumentRequest document)
        {
            Debug.WriteLine("model: " + ModelState.IsValid);
            
            var x =  ModelState.Values.SelectMany(v => v.Errors);
            x.ToList().ForEach(y => Debug.WriteLine(y.ErrorMessage));
            var result = await _documents.AddDocument(HttpContext.CreateAuthenticatedContext(document));
            return Json(result);
        }

        
        /// <summary>
        /// GET an application/octet-stream for the specified file.
        /// </summary>
        /// <param name="documentId"></param>
        [HttpGet]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}, " +
                           $"{AuthorizationRole.User}, " +
                           $"{AuthorizationRole.Manager}")]
        [Route("[controller]/{documentId:guid}/download")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(FileStreamResult))]
        public async Task<IActionResult> Download([FromRoute] Guid documentId)
        {
            var result = await _documents.GetDocument(HttpContext.CreateAuthenticatedContext(documentId));
            
            if (result.Success)
            {
                await using (var handle = System.IO.File.OpenRead(result.Data!.Location))
                {
                    return File(handle, "application/octet-stream", result.Data.Name);   
                }
            }
            else
            {
                return StatusCode(404);   
            }
        }
        
        /// <summary>
        /// Get the associated metadata for a file
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}, " +
                           $"{AuthorizationRole.User}, " +
                           $"{AuthorizationRole.Manager}")]
        [Route("/[controller]/{documentId:guid}/meta")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(FileStreamResult))]
        public async Task<IActionResult> Meta([FromRoute] Guid documentId)
        {
            var result = await _documents.GetDocument(HttpContext.CreateAuthenticatedContext(documentId));
            return Json(result);
        }

        /// <summary>
        /// Grant a user access to a document.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Route("[controller]/{documentId:guid}/user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse))]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        public async Task<IActionResult> GrantUser([FromRoute] Guid documentId, [FromBody] AddDocumentUserRequest request)
        {
            request.DocumentId = documentId;
            var result = await _documents.GrantUser(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        
        /// <summary>
        /// Revoke a user's access to a document
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [Route("[controller]/{documentId:guid}/user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        public async Task<IActionResult> RevokeUser([FromRoute] Guid documentId, [FromBody] AddDocumentUserRequest request)
        {
            request.DocumentId = documentId;
            var result = await _documents.RevokeUser(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        /// <summary>
        /// Grant a group access to a document.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ApiResponse<DocumentTransfer>))]

        [Route("[controller]/{documentId:guid}/group")]
        public async Task<IActionResult> GrantGroup([FromRoute] Guid documentId, [FromBody] AddDocumentGroupRequest request)
        {
            request.DocumentId = documentId;
            var result = await _documents.GrantGroup(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        /// <summary>
        /// Revoke a group's access to a document.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}")]
        [Route("[controller]/{documentId:guid}/group")]
        public async Task<IActionResult> RevokeGroup([FromRoute] Guid documentId, [FromBody] AddDocumentGroupRequest request)
        {
            request.DocumentId = documentId;
            var result = await _documents.RevokeGroup(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        /// <summary>
        /// Update the metadata or file for an existing document.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}, " +
                           $"{AuthorizationRole.User}, " +
                           $"{AuthorizationRole.Manager}")]
        [Route("[controller]/{documentId:guid}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(FileStreamResult))]
        public async Task<IActionResult> UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentRequest request)
        {
            request.Id = documentId;
            var result = await _documents.UpdateDocument(HttpContext.CreateAuthenticatedContext(request));
            return Json(result);
        }
        
        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [Authorize(Roles = $"{AuthorizationRole.Administrator}, " +
                           $"{AuthorizationRole.User}, " +
                           $"{AuthorizationRole.Manager}")]
        [Route("[controller]/{documentId:guid}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(FileStreamResult))]
        public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
        {
            
            var result = await _documents.DeleteDocument(HttpContext.CreateAuthenticatedContext(documentId));
            return Json(result);

        }
        
    }
}