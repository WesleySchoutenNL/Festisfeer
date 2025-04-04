using System.ComponentModel.DataAnnotations;

namespace Festisfeer.Presentation.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
            
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }

}
