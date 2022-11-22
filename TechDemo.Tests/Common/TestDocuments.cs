using System.Text.Json.Nodes;
using TechDemo.Core;
using TechDemo.Data.Entities;

namespace TechDemo.Tests.Common
{
    public static class TestDocuments
    {
        //TODO Can be extended into a document factory; json file, etc.
        public static Document GOOD_DOCUMENT() => new()
        {
            Id = Guid.NewGuid(),
            Name = "GoodDocument.pdf",
            Category = DocumentCategory.InverseCramer,
            Description = "a good document description",
            Tags = new JsonObject()
            {
                ["TestName"] = "TestValue"
            }.ToJsonString()
        };
        
        public static Document BAD_DOCUMENT() => new()
        {
            Id = Guid.Empty,
            Name = "BadDocument.pdf",
            Category = DocumentCategory.Cramer,
            Description = string.Empty,
            Tags = null!
        };
    }   
}