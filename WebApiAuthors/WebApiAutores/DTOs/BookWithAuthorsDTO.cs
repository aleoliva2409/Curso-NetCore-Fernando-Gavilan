﻿namespace WebApiAuthors.DTOs
{
    public class BookWithAuthorsDTO : BookDTO
    {
        public List<AuthorDTO> Authors { get; set; }
    }
}
