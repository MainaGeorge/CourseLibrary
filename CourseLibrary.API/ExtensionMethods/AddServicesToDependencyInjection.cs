using CourseLibrary.API.Contracts;
using CourseLibrary.API.Data;
using CourseLibrary.API.Mappings;
using CourseLibrary.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
                    .AddXmlDataContractSerializerFormatters();
                
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
            services.AddAutoMapper(p => p.AddProfile(new MappingProfile()));
            return services;
        }
    }
}
