using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repo;

        public AuthorsController(ICourseLibraryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<Author>> GetAuthors()
        {
            return Ok(_repo.GetAuthors());
        }


        [HttpGet("{authorId:Guid}")]
        public ActionResult<Author> GetAuthorById(Guid authorId)
        {
            var author = _repo.GetAuthor(authorId);

            if (author is null) return NotFound();

            return Ok(author);
        }
    }
}
