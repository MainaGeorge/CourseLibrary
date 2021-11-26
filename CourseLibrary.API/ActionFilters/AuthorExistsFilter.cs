using System;
using CourseLibrary.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseLibrary.API.ActionFilters
{
    public class AuthorExistsFilter : IActionFilter
    {
        private readonly ICourseLibraryRepository _repo;

        public AuthorExistsFilter(ICourseLibraryRepository repo)
        {
            _repo = repo;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.ContainsKey("authorId"))
            {
                context.Result = new BadRequestResult();
            }
            else
            {
                var authorId = (Guid) context.ActionArguments["authorId"];
                var author = _repo.GetAuthor(authorId);

                if (author is null)
                {
                    context.Result = new NotFoundResult();
                }
                else
                {
                    context.HttpContext.Items.Add("author", author);
                }
            }

             
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
