using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace TechDemo.Data.Entities
{
    [PgName("group_user")]
    public class GroupUser
    {
        [Key]
        [PgName("id")]
        public Guid Id { get; set; }
        
        [PgName("group_id")]
        public Guid GroupId { get; set; }
        
        [PgName("user_id")]
        public Guid UserId { get; set; }
    }    
}