using System.ComponentModel.DataAnnotations;

namespace InterViewV2.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Wrong Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Wrong Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
