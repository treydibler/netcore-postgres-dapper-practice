using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Newtonsoft.Json.Linq;
using Npgsql;
using TechDemo.Core;
using TechDemo.Data.Entities;
using TechDemo.Data.Source;
using TechDemo.DTO.Documents;
using TechDemo.Tests.Common;
using TechDemo.Tests.Extensions;
using TechDemo.Tests.Integration;
using TechDemo.Web;
using Xunit;

namespace TechDemo.Tests.Integration
{
    public class DocumentTests : IntegrationTestBase
    {
        public DocumentTests(CustomWebApplicationFactory<Startup> factory) : base(factory) { }
        

        [Fact]
        public async Task must_authenticate_to_upload()
        {
            var response = await HttpClient.PostAsync("/Document/upload", new MultipartFormDataContent());
            Assert.Contains(response.StatusCode, new HttpStatusCode[]
            {
                HttpStatusCode.Unauthorized, 
                HttpStatusCode.Forbidden
            });
        }

        [Fact]
        public async Task can_upload_good_document()
        {
            var doc = TestDocuments.GOOD_DOCUMENT();

            var response = await HttpClient
                .WithUserAuthentication(
                    TestUsers.GOOD_MANAGER()
                ).PostAsync("/Document/upload", 
                    CreateDocumentForm(doc, Guid.NewGuid().ToByteArray()));

            
            Assert.True(response.IsSuccessStatusCode);
            Assert.Contains(response.StatusCode, new []
            {
                HttpStatusCode.OK
            });

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<DocumentTransfer>>();
            Assert.False(result == null);
            Assert.False(result.Data?.Id == Guid.Empty);
            Assert.True(result.Data!.Added < DateTime.UtcNow);
        }


        [Fact]
        public async Task role_fail_upload_document()
        {
            var doc = TestDocuments.GOOD_DOCUMENT();
            
            var response = await HttpClient
                .WithUserAuthentication(
                    TestUsers.GOOD_USER()
                ).PostAsync("/Document/upload", 
                    CreateDocumentForm(doc, Guid.NewGuid().ToByteArray()));

        }
        
        [Fact]
        public async Task fail_upload_bad_document()
        {
            var doc = TestDocuments.BAD_DOCUMENT();
            
            var response = await HttpClient
                .WithUserAuthentication(
                    TestUsers.GOOD_MANAGER()
                        ).PostAsync("/Document/upload", 
                    CreateDocumentForm(doc, Guid.NewGuid().ToByteArray()));


            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        private static MultipartFormDataContent CreateDocumentForm(Document document, byte[] data)
        {
            MultipartFormDataContent content = new ();
                content.Add(new StreamContent(new MemoryStream(data)), $"file", $"{document.Name}");
                content.Add(new StringContent($"{document.Name}"), nameof(document.Name));
                content.Add(new StringContent($"{document.Description}"), nameof(document.Description));
                content.Add(new StringContent($"{document.Category}"), nameof(document.Category));
                content.Add(new StringContent($"{document.Tags}"), nameof(document.Tags));
            return content;
        }

    }
}

