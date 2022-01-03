using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
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
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repo;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository repo, IMapper mapper)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<Author>> GetAuthors(
            [FromQuery] AuthorRequestParameters parameters)
        {
            var authors = _repo.GetAuthors(parameters);
            var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorDtos);
        }

        [HttpGet("{authorId:Guid}", Name = nameof(GetAuthorById))]
        public ActionResult<AuthorDto> GetAuthorById(Guid authorId)
        {
            var author = _repo.GetAuthor(authorId);

            if (author is null) return NotFound();

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(
            [FromBody] AuthorForCreationDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);

            _repo.AddAuthor(author);
            _repo.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(author);

            return CreatedAtRoute(nameof(GetAuthorById),
                new {authorId = authorToReturn.Id}, authorToReturn);
        }

        [HttpPut("{authorId:guid}")]
        public IActionResult UpdateAuthor(Guid authorId, AuthorForUpdatingDto authorForUpdatingDto)
        {
            var author = _repo.GetAuthor(authorId);

            if(author is null) return NotFound();  

            _mapper.Map(authorForUpdatingDto, author);

            _repo.UpdateAuthor(author); 

            _repo.Save();

            return NoContent();
        }

        [HttpPatch("{authorId:guid}")]
        public IActionResult PatchAuthor(Guid authorId, JsonPatchDocument<AuthorForUpdatingDto> patcher)
        {
            var author = _repo.GetAuthor(authorId);

            if(author is null) return NotFound();

            var authorForUpdatingDto = _mapper.Map<AuthorForUpdatingDto>(author);

            patcher.ApplyTo(authorForUpdatingDto, ModelState);

            if(!TryValidateModel(authorForUpdatingDto)) return ValidationProblem(ModelState);

            _mapper.Map(authorForUpdatingDto, author);

            _repo.UpdateAuthor(author);

            _repo.Save();

            return NoContent();

        }

        [HttpDelete("{authorId:guid}")]
        public IActionResult DeleteAuthor(Guid authorId)
        {
            var author = _repo.GetAuthor(authorId);

            if(author is null) return NotFound();   

            _repo.DeleteAuthor(author); 

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
