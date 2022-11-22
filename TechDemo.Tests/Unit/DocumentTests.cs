using System.Security.Principal;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Documents;
using TechDemo.DTO.Users;
using TechDemo.Services;
using TechDemo.Tests.Common;
using Xunit;

namespace TechDemo.Tests.Unit
{
    public class DocumentTests
    {
        [Fact]
        public async Task add_and_exists_by_hash()
        {
            var context = new Mock<IDapperContext>();
            var hasher = new Mock<IPasswordHasher<User>>();
            var storage = new Mock<IStorageService>();

            context.Setup(x => x.ExecuteReadSingleAsync<User>("CALL proc_find_user_by_username(@user_id);", It.IsAny<NpgsqlParameter>()))
                .ReturnsAsync(TestUsers.GOOD_USER());

            context.Setup(x => x.ExecuteReadSingleAsync<Document>("CALL proc_get_document_by_id(@user_id, @document_id);", It.IsAny<NpgsqlParameter>()))
                .ReturnsAsync(TestDocuments.GOOD_DOCUMENT);

            

            hasher.Setup(x => 
                    x.VerifyHashedPassword(
                        It.IsAny<User>(), 
                        It.IsAny<string>(), 
                        It.IsAny<string>()
                    )
                )
                .Returns(PasswordVerificationResult.Success);

            var service = new DocumentService(
                new Mock<ILogger<DocumentService>>().Object,
                ApplicationConfiguration.Instance,
                storage.Object,
                context.Object
            );

            var user = TestUsers.GOOD_MANAGER();
            var document = TestDocuments.GOOD_DOCUMENT();
            byte[] test = Guid.NewGuid().ToByteArray();

            var result = await service.AddDocument(
                new AuthenticatedApiRequest<AddDocumentRequest>(
                    new AddDocumentRequest()
                    {
                            File = new FormFile(new MemoryStream(test), 0, test.Length, "file", document.Name),
                            Category = document.Category,
                            Description = document.Description,
                            Name = document.Name,
                            Tags = document.Tags
                    }
            ));
        
            Assert.True(string.IsNullOrEmpty(result.Error));
            Assert.True(result.Success);
            Assert.True(result?.Data?.Id != Guid.Empty);



            var existing = await service.GetDocument(
                new AuthenticatedApiRequest<Guid>(document.Id)
                {
                  UserId  = user.Id
                }
            );
            
            
            Assert.True(existing.Data!.Id == document.Id);
        }
    }   
}