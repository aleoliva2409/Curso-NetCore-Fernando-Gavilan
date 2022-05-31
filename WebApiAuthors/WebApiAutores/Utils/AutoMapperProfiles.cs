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
            CreateMap<BookCreateDTO, Book>();
            CreateMap<Book, BookDTO>();
            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
    }
}
