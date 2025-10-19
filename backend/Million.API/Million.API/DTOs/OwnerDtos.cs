using Million.API.Domain;
using System.ComponentModel.DataAnnotations;

namespace Million.API.DTOs
{
    public class OwnerDto
    {
        public string IdOwner { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class CreateOwnerDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Url(ErrorMessage = "Photo must be a valid URL")]
        public string? Photo { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateOwnerDto), nameof(ValidateBirthdayRules))]
        public DateTime Birthday { get; set; }

        public static ValidationResult? ValidateBirthdayRules(DateTime birthday, ValidationContext context)
        {
            // Validar que no sea la fecha por defecto (puede indicar que no se proporcionó)
            if (birthday == default(DateTime) || birthday.Year < 1900)
            {
                return new ValidationResult("Birthday must be a valid date.");
            }

            // Validar que no esté en el futuro
            if (birthday >= DateTime.UtcNow)
            {
                return new ValidationResult("Birthday cannot be in the future.");
            }

            // Calcular edad correctamente
            var today = DateTime.UtcNow;
            var age = today.Year - birthday.Year;
            
            // Ajustar si aún no ha cumplido años este año
            if (birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            // Validar edad mínima
            if (age < 18)
            {
                return new ValidationResult("Owner must be at least 18 years old.");
            }

            // Validar que la edad sea razonable (máximo 150 años)
            if (age > 150)
            {
                return new ValidationResult("Birthday date is not valid.");
            }

            return ValidationResult.Success;
        }
    }

    public class UpdateOwnerDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Url(ErrorMessage = "Photo must be a valid URL")]
        public string? Photo { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdateOwnerDto), nameof(ValidateBirthdayRules))]
        public DateTime Birthday { get; set; }

        public static ValidationResult? ValidateBirthdayRules(DateTime birthday, ValidationContext context)
        {
            // Validar que no sea la fecha por defecto (puede indicar que no se proporcionó)
            if (birthday == default(DateTime) || birthday.Year < 1900)
            {
                return new ValidationResult("Birthday must be a valid date.");
            }

            // Validar que no esté en el futuro
            if (birthday >= DateTime.UtcNow)
            {
                return new ValidationResult("Birthday cannot be in the future.");
            }

            // Calcular edad correctamente
            var today = DateTime.UtcNow;
            var age = today.Year - birthday.Year;
            
            // Ajustar si aún no ha cumplido años este año
            if (birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            // Validar edad mínima
            if (age < 18)
            {
                return new ValidationResult("Owner must be at least 18 years old.");
            }

            // Validar que la edad sea razonable (máximo 150 años)
            if (age > 150)
            {
                return new ValidationResult("Birthday date is not valid.");
            }

            return ValidationResult.Success;
        }
    }
}
