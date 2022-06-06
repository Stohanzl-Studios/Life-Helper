using System.ComponentModel.DataAnnotations;

namespace LifeHelper.Forms
{
    public class LoginFormModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password too short", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
