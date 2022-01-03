using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;
using CourseLibrary.API.RequestParameters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        public ActionResult<IEnumerable<CourseDto>> GetCourses([FromRoute]Guid authorId, 
            [FromQuery]CoursesRequestParameters parameters)
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

        [HttpPatch("{courseId:guid}")]
        public ActionResult PatchCourse(Guid authorId, Guid courseId,
            JsonPatchDocument<CourseForUpdatingDto> patcher)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest();

            var course = _repo.GetCourse(authorId, courseId);
            if(course is null) return NotFound();   

            var toPatch = _mapper.Map<CourseForUpdatingDto>(course);

            patcher.ApplyTo(toPatch, ModelState);

            if (!TryValidateModel(toPatch))
                return ValidationProblem(ModelState);

            _mapper.Map(toPatch, course);

            _repo.UpdateCourse(course);

            _repo.Save();

            return NoContent();
        }

        [HttpDelete("{courseId:guid}")]
        public IActionResult DeleteCourse(Guid courseId, Guid authorId)
        {
            if(!_repo.AuthorExists(authorId)) return BadRequest(); 

            var course = _repo.GetCourse(authorId, courseId);
            if(course is null) return NotFound();

            _repo.DeleteCourse(course);

            _repo.Save();

            return NoContent();
        }
         public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        
    }
}
