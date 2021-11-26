using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CourseLibrary.API.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title{ get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [JsonIgnore]
        public Author Author { get; set; }

        [ForeignKey(nameof(Author))]
        public Guid AuthorId { get; set; }  
    }
}