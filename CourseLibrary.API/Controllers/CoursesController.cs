using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;

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
        public ActionResult<IEnumerable<CourseDto>> GetCourses(Guid authorId)
        {
            if (!_repo.AuthorExists(authorId)) return NotFound();

            var courses = _repo.GetCourses(authorId);

            if (courses is null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("/{courseId:guid}")]
        public ActionResult<CourseDto> GetCourseById(Guid authorId,Guid courseId)
        {
            if (!_repo.AuthorExists(authorId)) return NotFound();

            var course = _repo.GetCourse(authorId, courseId);

            if (course is null) return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }
    }
}
