using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;

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
        public ActionResult<IEnumerable<Author>> GetAuthors()
        {
            var authors = _repo.GetAuthors();
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
    }
}
