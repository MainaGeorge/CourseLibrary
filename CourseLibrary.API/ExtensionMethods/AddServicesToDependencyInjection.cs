using CourseLibrary.API.Contracts;
using CourseLibrary.API.Data;
using CourseLibrary.API.Mappings;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace CourseLibrary.API.ExtensionMethods
{
    public static class AddServicesToDependencyInjection
    {
        public static IServiceCollection AddConfiguredControllers(this IServiceCollection services)
        {
            services.AddControllers(op =>
                        {
                            op.ReturnHttpNotAcceptable = true;
                        })
                    .AddXmlDataContractSerializerFormatters()
                    .ConfigureApiBehaviorOptions(setupAction =>
                    {
                        setupAction.InvalidModelStateResponseFactory
                        = context =>
                            {
                            var problemsDetailsFactory = context.HttpContext.RequestServices
                                .GetRequiredService<ProblemDetailsFactory>();
                            var problemDetails = problemsDetailsFactory
                                .CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                            //add additional info not added by default
                            problemDetails.Detail = "See the error fields for details";
                            problemDetails.Instance = context.HttpContext.Request.Path;

                            //find out which status code to use
                            var actionExecutingContext =
                                context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                            if (context.ModelState.ErrorCount > 0 &&
                                actionExecutingContext?.ActionArguments.Count
                                 == context.ActionDescriptor.Parameters.Count)
                            {
                                problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                                problemDetails.Title = "one or more validation errors occurred";

                                return new UnprocessableEntityObjectResult(problemDetails)
                                {
                                    ContentTypes = {"application/problem+json"}
                                };

                            }
                            problemDetails.Status = StatusCodes.Status400BadRequest;
                            problemDetails.Title = "one or more errors occurred";

                            return new BadRequestObjectResult(problemDetails)
                            {
                                ContentTypes = {"application/problem+json"}
                            };
                        };
                    });
            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<CourseLibraryContext>(opts =>
            {
                opts.UseSqlServer(config.GetConnectionString("Default"));
            });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentationTools(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CourseLibrary.API", Version = "v1" });
            });

            return services;
        }

        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
