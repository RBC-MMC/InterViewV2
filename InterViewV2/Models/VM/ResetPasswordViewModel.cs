using System.ComponentModel.DataAnnotations;

namespace InterViewV2.Models.VM
{
    public class ResetPasswordViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
