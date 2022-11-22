namespace TechDemo.DTO.Groups
{
    public class AddGroupUserRequest
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }   
}