using System.ComponentModel.DataAnnotations;

namespace WebAPIDemo.Models.Validations
{
    public class ColorValidationAttribute : ValidationAttribute
    {
        private static readonly string[] ValidColors =
        {
            "red", "blue", "green", "yellow", "black", "white", "orange", "purple",
            "pink", "brown", "grey", "cyan", "magenta", "lime", "teal", "violet",
            "indigo", "turquoise", "gold", "silver", "beige", "maroon", "navy",
            "olive", "peach", "lavender", "coral", "ivory", "plum", "khaki", "mint",
            "salmon", "chocolate", "crimson", "sienna", "fuchsia", "periwinkle",
            "emerald", "charcoal", "rose", "mustard", "apricot", "amber"

        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string color && ValidColors.Contains(color.ToLower()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Enter a valid color name.");
            }
        }
    }
}
