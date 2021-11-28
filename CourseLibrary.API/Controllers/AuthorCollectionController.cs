using AutoMapper;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;
using CourseLibrary.API.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authorcollection")]
    [ApiController]
    public class AuthorCollectionController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repo;
        private readonly IMapper _mapper;

        public AuthorCollectionController(ICourseLibraryRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name = nameof(GetCollectionOfAuthors))]
        public ActionResult<IEnumerable<AuthorDto>> GetCollectionOfAuthors(
            [FromRoute]
            [ModelBinder(typeof(GetIdsFromRouteModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null) return BadRequest();

            var authors = _repo.GetAuthors(ids);

            if (authors.Count() != ids.Count()) return NotFound();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsToReturn);
        }

        public ActionResult<IEnumerable<AuthorDto>> CreateCollectionOfAuthors(
            [FromBody] IEnumerable<AuthorForCreationDto> authorsForCreation)
        {
            var authors = _mapper.Map<IEnumerable<Author>>(authorsForCreation);

            foreach (var author in authors) _repo.AddAuthor(author);
            _repo.Save();

            var ids = string.Join(",", authors.Select(a => a.Id));

            var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);

            return CreatedAtRoute(nameof(GetCollectionOfAuthors),
                new { ids }, authorDtos);
        }
    }
}
