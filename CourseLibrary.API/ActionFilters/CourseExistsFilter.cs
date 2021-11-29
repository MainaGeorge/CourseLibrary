using CourseLibrary.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CourseLibrary.API.ActionFilters
{
    public class CourseExistsFilter : IActionFilter
    {
        private readonly ICourseLibraryRepository _repo;

        public CourseExistsFilter(ICourseLibraryRepository repo)
        {
            _repo = repo;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.ActionArguments.ContainsKey("authorId"))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (context.ActionArguments.ContainsKey("courseId"))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var courseId = (Guid)context.ActionArguments["courseId"];
            var authorId = (Guid)context.ActionArguments["authorId"];

            var course = _repo.GetCourse(authorId, courseId);

            if(course is null) {context.Result = new NotFoundResult(); }
            else {context.HttpContext.Items.Add("course", course); }
        }
    }
}
