namespace TechDemo.DTO.Grants
{
    public class AddDocumentUserRequest
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
    }   
}