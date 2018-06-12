using System.ComponentModel.DataAnnotations;

namespace SpaceMe.Models
{
    public class NewAdmin
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}