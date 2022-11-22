namespace TechDemo.DTO.Users
{
    public class UpdateUserRequest : AddUserRequest
    {
        public Guid Id { get; set; }
    }    
}