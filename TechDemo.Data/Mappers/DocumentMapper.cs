using TechDemo.DTO.Documents;
using TechDemo.Data.Entities;

namespace TechDemo.Data.Mappers
{
    public class DocumentMap
    {
    
    }

    public static class DocumentMapper
    {
        public static DocumentTransfer ToTransfer(this Document document)
        {
            return new DocumentTransfer()
            {
                Id = document.Id,
                Location = document.Location,
                Added = document.Added,
                Category =  document.Category.ToString(),
                Description = document.Description,
                Name = document.Name,
                Tags = document.Tags
            };
        } 
    }   
}