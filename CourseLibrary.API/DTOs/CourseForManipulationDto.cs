using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.DTOs
{
    public abstract class CourseForManipulationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title{ get; set; }

        [Required]
        [MaxLength(1500, ErrorMessage="the description can not exceed 1500 characters")]
        public string Description { get; set; }
    }
}
