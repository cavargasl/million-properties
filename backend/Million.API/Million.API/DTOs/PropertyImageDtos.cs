using System.ComponentModel.DataAnnotations;

namespace Million.API.DTOs
{
    public class PropertyImageDto
    {
        public string IdPropertyImage { get; set; } = string.Empty;
        public string IdProperty { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }

    public class CreatePropertyImageDto
    {
        [Required(ErrorMessage = "IdProperty is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "IdProperty is required")]
        public string IdProperty { get; set; } = string.Empty;

        [Required(ErrorMessage = "File is required")]
        [Url(ErrorMessage = "File must be a valid URL")]
        [StringLength(500, ErrorMessage = "File URL cannot exceed 500 characters")]
        public string File { get; set; } = string.Empty;

        public bool Enabled { get; set; } = true;
    }

    public class UpdatePropertyImageDto
    {
        [Required(ErrorMessage = "File is required")]
        [Url(ErrorMessage = "File must be a valid URL")]
        [StringLength(500, ErrorMessage = "File URL cannot exceed 500 characters")]
        public string File { get; set; } = string.Empty;

        public bool Enabled { get; set; }
    }
}
