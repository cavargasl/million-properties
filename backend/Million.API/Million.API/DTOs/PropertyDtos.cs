using System.ComponentModel.DataAnnotations;

namespace Million.API.DTOs
{
    public class PropertyDto
    {
        public string IdProperty { get; set; } = string.Empty;
        public string IdOwner { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; } // Just one image as required
    }

    public class CreatePropertyDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 250 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "CodeInternal is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "CodeInternal must be between 3 and 50 characters")]
        [RegularExpression(@"^[A-Za-z0-9-_]+$", ErrorMessage = "CodeInternal can only contain letters, numbers, hyphens and underscores")]
        public string CodeInternal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [CustomValidation(typeof(CreatePropertyDto), nameof(ValidateYearRules))]
        public int Year { get; set; }

        [Required(ErrorMessage = "IdOwner is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "IdOwner is required")]
        public string IdOwner { get; set; } = string.Empty;

        public static ValidationResult? ValidateYearRules(int year, ValidationContext context)
        {
            var currentYear = DateTime.UtcNow.Year;
            const int minimumYear = 1800;

            if (year < minimumYear)
            {
                return new ValidationResult($"Year cannot be before {minimumYear}.");
            }

            if (year > currentYear + 5)
            {
                return new ValidationResult($"Year cannot be more than 5 years in the future.");
            }

            return ValidationResult.Success;
        }
    }

    public class UpdatePropertyDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 250 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "CodeInternal is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "CodeInternal must be between 3 and 50 characters")]
        [RegularExpression(@"^[A-Za-z0-9-_]+$", ErrorMessage = "CodeInternal can only contain letters, numbers, hyphens and underscores")]
        public string CodeInternal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [CustomValidation(typeof(UpdatePropertyDto), nameof(ValidateYearRules))]
        public int Year { get; set; }

        [Required(ErrorMessage = "IdOwner is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "IdOwner is required")]
        public string IdOwner { get; set; } = string.Empty;

        public static ValidationResult? ValidateYearRules(int year, ValidationContext context)
        {
            var currentYear = DateTime.UtcNow.Year;
            const int minimumYear = 1800;

            if (year < minimumYear)
            {
                return new ValidationResult($"Year cannot be before {minimumYear}.");
            }

            if (year > currentYear + 5)
            {
                return new ValidationResult($"Year cannot be more than 5 years in the future.");
            }

            return ValidationResult.Success;
        }
    }

    public class PropertyDetailDto
    {
        public string IdProperty { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = string.Empty;
        public int Year { get; set; }
        public string IdOwner { get; set; } = string.Empty;
        public OwnerDto? Owner { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
    }
}
