using System.ComponentModel.DataAnnotations;

namespace LifeHelper.Forms
{
    public class RegisterFormModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [StringLength(15, ErrorMessage = "Username too long", MinimumLength = 1)]
        public string Username { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password too short", MinimumLength = 6)]
        public string Password { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Passwords do not match")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string VerifyPassword { get; set; } = "";
    }
}