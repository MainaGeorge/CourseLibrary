using System;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.DTOs
{
    public abstract class AuthorForManipulation
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        public string MainCategory { get; set; }
    }
}
