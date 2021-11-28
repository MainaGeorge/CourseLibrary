using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.ActionFilters;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors/{authorId:guid}/courses")]
    [ApiController]
    [ServiceFilter(typeof(AuthorExistsFilter))]
    public class CoursesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICourseLibraryRepository _repo;

        public CoursesController(IMapper mapper, ICourseLibraryRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCourses(Guid authorId)
        {
            var author = HttpContext.Items["author"] as Author;

            if (author!.Id != authorId) return BadRequest();
            
            var courses = _repo.GetCourses(author!.Id);

            if (courses is null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("{courseId:guid}", Name = nameof(GetCourseById))]
        public ActionResult<CourseDto> GetCourseById(Guid authorId,Guid courseId)
        {
            var author = HttpContext.Items["authorId"] as Author;

            if (author!.Id != authorId) return BadRequest();

            var course = _repo.GetCourse(authorId, courseId);

            if (course is null) return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourse(
            [FromRoute] Guid authorId,
            [FromBody] CourseForCreationDto courseForCreationDto)
        {
            var course = _mapper.Map<Course>(courseForCreationDto);
            _repo.AddCourse(authorId, course);
            _repo.Save();

            var courseToReturn = _mapper.Map<CourseDto>(course);

            return CreatedAtRoute(nameof(GetCourseById),
                new {authorId, courseId = course.Id}, courseToReturn);

        }
    }
}
