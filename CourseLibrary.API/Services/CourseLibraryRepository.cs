using System;
using System.Collections.Generic;
using System.Linq;
using CourseLibrary.API.Contracts;
using CourseLibrary.API.Data;
using CourseLibrary.API.Entities;
using CourseLibrary.API.RequestParameters;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository
    {
        private readonly CourseLibraryContext _context;

        public CourseLibraryRepository(CourseLibraryContext context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course); 
        }         

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }
  
        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses.FirstOrDefault(c => c.AuthorId == authorId && c.Id == courseId);
        }

        public IEnumerable<Course> GetCourses(Guid authorId, CoursesRequestParameters parameters)
        {
            if (parameters is null) throw new ArgumentNullException(nameof(parameters));
            if (authorId == Guid.Empty) throw new ArgumentNullException(nameof(authorId));

            var courses = _context.Courses.Where(a => a.AuthorId == authorId) as IQueryable<Course>;

            return courses
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();

        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();
            if (author.Courses.Any())
            {
                foreach (var course in author.Courses)
                {
                    course.Id = Guid.NewGuid();
                }
            }
            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }
        
        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors(AuthorRequestParameters parameters)
        {
            if (parameters is null) throw new ArgumentNullException(nameof(parameters));

            var authors = _context.Authors as IQueryable<Author>;

            authors = FilterByMainCategory(parameters, authors);

            authors = FilterByFirstAndLastName(authors, parameters.FirstName, parameters.LastName);

            return authors
                .Skip((parameters.PageNumber-1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList<Author>();
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               // dispose resources when needed
            }
        }

        private static IQueryable<Author> FilterByFirstAndLastName(IQueryable<Author> authors,
            string firstName, string lastName)
        {
            switch (string.IsNullOrWhiteSpace(firstName))
            {
                case false when !string.IsNullOrWhiteSpace(lastName):
                    authors = authors.Where(a => string.Equals(a.FirstName, firstName.Trim(),
                        StringComparison.CurrentCultureIgnoreCase));
                    break;
                case false:
                    authors = authors.Where(a => string.Equals(a.FirstName, firstName,
                        StringComparison.CurrentCultureIgnoreCase));
                    break;
                default:
                {
                    if (!string.IsNullOrWhiteSpace(lastName))
                    {
                        authors = authors.Where(a => string.Equals(a.LastName, lastName,
                            StringComparison.CurrentCultureIgnoreCase));
                    }
                    break;
                }
            }

            return authors;
        }

        private static IQueryable<Author> FilterByMainCategory(AuthorRequestParameters parameters, IQueryable<Author> authors)
        {
            if (!string.IsNullOrWhiteSpace(parameters.MainCategory))
                authors = authors.Where(a => a.MainCategory.ToLower()
                    .Contains(parameters.MainCategory.ToLower()));

            return authors;
        }
    }
}

