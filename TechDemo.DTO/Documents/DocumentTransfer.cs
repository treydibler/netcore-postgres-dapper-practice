using TechDemo.Core;

namespace TechDemo.DTO.Documents
{
    public class DocumentTransfer
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime Added { get; set; }
        
        public string Tags = "{}";
    }
}