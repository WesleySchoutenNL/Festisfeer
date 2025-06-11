using System.ComponentModel.DataAnnotations;

namespace Festisfeer.Presentation.Models
{
    public class LoginViewModel
    {
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
