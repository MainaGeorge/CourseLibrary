using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.DTOs
{
    public class CourseForCreationDto
    {
        [Required]
        public string Title{ get; set; }

        [Required]
        public string Description { get; set; }
    }
}
