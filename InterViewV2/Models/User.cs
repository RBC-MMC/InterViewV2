using Microsoft.AspNetCore.Identity;

namespace InterViewV2.Models
{
    public class User : IdentityUser<int>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department Department { get; set; }
        public Guid? PositionId { get; set; }
        public Position Position { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
