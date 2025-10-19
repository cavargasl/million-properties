using System.ComponentModel.DataAnnotations;

namespace Million.API.DTOs
{
    public class PropertyTraceDto
    {
        public string IdPropertyTrace { get; set; } = string.Empty;
        public string IdProperty { get; set; } = string.Empty;
        public DateTime DateSale { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
    }

    public class CreatePropertyTraceDto
    {
        [Required(ErrorMessage = "IdProperty is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "IdProperty is required")]
        public string IdProperty { get; set; } = string.Empty;

        [Required(ErrorMessage = "DateSale is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreatePropertyTraceDto), nameof(ValidateDateSaleRules))]
        public DateTime DateSale { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Value is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Tax is required")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Tax must be 0 or greater")]
        [DataType(DataType.Currency)]
        [CustomValidation(typeof(CreatePropertyTraceDto), nameof(ValidateTaxRules))]
        public decimal Tax { get; set; }

        public static ValidationResult? ValidateDateSaleRules(DateTime dateSale, ValidationContext context)
        {
            // Validar que no sea la fecha por defecto
            if (dateSale == default(DateTime) || dateSale.Year < 1900)
            {
                return new ValidationResult("DateSale must be a valid date.");
            }

            // Permitir fechas futuras para ventas planificadas, pero con límite razonable
            var maxFutureDate = DateTime.UtcNow.AddYears(5);
            if (dateSale > maxFutureDate)
            {
                return new ValidationResult("DateSale cannot be more than 5 years in the future.");
            }

            // Validar que no sea demasiado antigua (más de 200 años)
            var minDate = DateTime.UtcNow.AddYears(-200);
            if (dateSale < minDate)
            {
                return new ValidationResult("DateSale cannot be more than 200 years in the past.");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateTaxRules(decimal tax, ValidationContext context)
        {
            var instance = context.ObjectInstance as CreatePropertyTraceDto;
            if (instance == null)
            {
                return ValidationResult.Success;
            }

            // Validar que el impuesto no sea mayor al 100% del valor
            if (tax > instance.Value)
            {
                return new ValidationResult("Tax cannot exceed the property value.");
            }

            // Validar porcentaje razonable (máximo 50% del valor)
            if (tax > instance.Value * 0.5m)
            {
                return new ValidationResult("Tax seems unusually high (more than 50% of value). Please verify.");
            }

            return ValidationResult.Success;
        }
    }

    public class UpdatePropertyTraceDto
    {
        [Required(ErrorMessage = "DateSale is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdatePropertyTraceDto), nameof(ValidateDateSaleRules))]
        public DateTime DateSale { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Value is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Tax is required")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Tax must be 0 or greater")]
        [DataType(DataType.Currency)]
        [CustomValidation(typeof(UpdatePropertyTraceDto), nameof(ValidateTaxRules))]
        public decimal Tax { get; set; }

        public static ValidationResult? ValidateDateSaleRules(DateTime dateSale, ValidationContext context)
        {
            // Validar que no sea la fecha por defecto
            if (dateSale == default(DateTime) || dateSale.Year < 1900)
            {
                return new ValidationResult("DateSale must be a valid date.");
            }

            // Permitir fechas futuras para ventas planificadas, pero con límite razonable
            var maxFutureDate = DateTime.UtcNow.AddYears(5);
            if (dateSale > maxFutureDate)
            {
                return new ValidationResult("DateSale cannot be more than 5 years in the future.");
            }

            // Validar que no sea demasiado antigua (más de 200 años)
            var minDate = DateTime.UtcNow.AddYears(-200);
            if (dateSale < minDate)
            {
                return new ValidationResult("DateSale cannot be more than 200 years in the past.");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateTaxRules(decimal tax, ValidationContext context)
        {
            var instance = context.ObjectInstance as UpdatePropertyTraceDto;
            if (instance == null)
            {
                return ValidationResult.Success;
            }

            // Validar que el impuesto no sea mayor al 100% del valor
            if (tax > instance.Value)
            {
                return new ValidationResult("Tax cannot exceed the property value.");
            }

            // Validar porcentaje razonable (máximo 50% del valor)
            if (tax > instance.Value * 0.5m)
            {
                return new ValidationResult("Tax seems unusually high (more than 50% of value). Please verify.");
            }

            return ValidationResult.Success;
        }
    }
}
