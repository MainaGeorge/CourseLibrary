using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.DTOs
{
    public class AuthorForCreationDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        public string MainCategory { get; set; }

        public ICollection<CourseForCreationDto> Courses { get; set; } 
            = new List<CourseForCreationDto>();
    }
}
