using AutoMapper;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AuthorCreateDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorWithBooksDTO>().ForMember(authorDto => authorDto.Books, opt => opt.MapFrom(MapAuthorDtoBooks));

            CreateMap<BookCreateDTO, Book>().ForMember(book => book.AuthorsBooks, opt => opt.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookDTO>();
            CreateMap<Book, BookWithAuthorsDTO>().ForMember(bookDto => bookDto.Authors, opt => opt.MapFrom(MapBookDtoAuthors));
            CreateMap<BookPatchDTO, Book>().ReverseMap();

            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();

        }

        private List<AuthorBook> MapAuthorsBooks(BookCreateDTO bookCreateDto, Book book)
        {
            var result = new List<AuthorBook>();

            if (bookCreateDto.AuthorsIds == null)
            {
                return result;
            }

            foreach (var authorId in bookCreateDto.AuthorsIds)
            {
                result.Add(new AuthorBook() { AuthorId = authorId });
            }

            return result;
        }

        private List<AuthorDTO> MapBookDtoAuthors(Book book, BookDTO bookDto)
        {
            var result = new List<AuthorDTO>();

            if (book.AuthorsBooks == null)
            {
                return result;
            }

            foreach (var authorBook in book.AuthorsBooks)
            {
                result.Add(new AuthorDTO()
                {
                    Id = authorBook.AuthorId,
                    Name = authorBook.Author.Name,
                });
            }

            return result;
        }
        private List<BookDTO> MapAuthorDtoBooks(Author author, AuthorDTO authorDto)
        {
            var result = new List<BookDTO>();


            if (author.AuthorsBooks == null)
            {
                return result;
            }

            foreach (var authorBook in author.AuthorsBooks)
            {
                result.Add(new BookDTO()
                {
                    Id = authorBook.BookId,
                    Title = authorBook.Book.Title,
                });
            }

            return result;
        }
    }
}
