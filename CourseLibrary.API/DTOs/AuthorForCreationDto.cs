using System.Collections.Generic;

namespace CourseLibrary.API.DTOs
{
    public class AuthorForCreationDto : AuthorForManipulation
    {
        public ICollection<CourseForCreationDto> Courses { get; set; } 
            = new List<CourseForCreationDto>();
    }
}
