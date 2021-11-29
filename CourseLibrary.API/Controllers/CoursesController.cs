using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.ActionFilters;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;
using CourseLibrary.API.RequestParameters;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors/{authorId:guid}/courses")]
    [ApiController]
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
        public ActionResult<IEnumerable<CourseDto>> GetCourses(Guid authorId, 
            CoursesRequestParameters parameters)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest();

            var courses = _repo.GetCourses(authorId, parameters);

            if (courses is null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("{courseId:guid}", Name = nameof(GetCourseById))]
        public ActionResult<CourseDto> GetCourseById(Guid authorId,Guid courseId)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest();

            var course = _repo.GetCourse(authorId, courseId);

            if (course is null) return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourse(
            [FromRoute] Guid authorId,
            [FromBody] CourseForCreationDto courseForCreationDto)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest();

            var course = _mapper.Map<Course>(courseForCreationDto);
            _repo.AddCourse(authorId, course);
            _repo.Save();

            var courseToReturn = _mapper.Map<CourseDto>(course);

            return CreatedAtRoute(nameof(GetCourseById),
                new {authorId, courseId = course.Id}, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourse(Guid authorId, Guid courseId, CourseForUpdatingDto courseForUpdatingDto)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest();

            var course = _repo.GetCourse(authorId, courseId);

            if(course is null) return NotFound();

            _mapper.Map(courseForUpdatingDto, course);

            _repo.UpdateCourse(course);

            _repo.Save();

            return NoContent(); 
        }
        
    }
}
