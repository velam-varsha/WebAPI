using System.ComponentModel.DataAnnotations;
using WebAPIDemo.Models.Validations;

namespace WebAPIDemo.Models
{
    public class Shirt
    {
        [Required]  // used for model validation
        public int ShirtId { get; set; }
        [Required]
        public string? Brand { get; set; }
        
        public string? Description { get; set; }
        [Required]
        [ColorValidation]
        public string? Color { get; set; }

        [Shirt_EnsureCorrectSizingAttribute]
        public int? Size { get; set; }
        [Required]
        public string? Gender { get; set; }
        public double? Price { get; set; }

        // for second version
        public bool ValidateDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }
    }
}
