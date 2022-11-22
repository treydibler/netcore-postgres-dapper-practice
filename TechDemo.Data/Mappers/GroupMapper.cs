using TechDemo.DTO.Groups;
using TechDemo.Data.Entities;

namespace TechDemo.Data.Mappers
{
    public static class GroupMapper
    {
        public static GroupTransfer ToTransfer(this Group group)
        {
            return new GroupTransfer()
            {
                Id = group.Id,
                Name = group.Name
            };
        }
        
        public static List<GroupTransfer> ToTransfer(this List<Group> groups)
        {
            return groups.Select(ToTransfer).ToList();
        }
    }   
}