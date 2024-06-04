using System.ComponentModel.DataAnnotations;

namespace WebAPIDemo.Models.Validations
{
    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute  // we want to create an attribute Shirt_EnsureCorrectSizingAttribute so as to use this attribute to decorate the size property. So to make sure that Shirt_EnsureCorrectSizingAttribute is an attribute, then we should derive it from attribute class named ValidationAttribute
    {
        // now we should override the IsValid method of the parent class
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var shirt = validationContext.ObjectInstance as Shirt;
            if(shirt != null && !string.IsNullOrWhiteSpace(shirt.Gender))
            {
                if(shirt.Gender.Equals("men", StringComparison.OrdinalIgnoreCase) && shirt.Size < 8)
                {
                    return new ValidationResult("For men's shirts, the size has to be greater or equal to 8.");
                }
                else if (shirt.Gender.Equals("women", StringComparison.OrdinalIgnoreCase) && shirt.Size < 6)
                {
                    return new ValidationResult("For women's shirts, the size has to be greater or equal to 6.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
