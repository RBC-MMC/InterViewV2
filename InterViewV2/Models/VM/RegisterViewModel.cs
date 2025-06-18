using System.ComponentModel.DataAnnotations;

namespace InterViewV2.Models.VM
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
