namespace WebApiAuthors.DTOs
{
    public class DatoHATEOAS
    {
        public string Link { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }

        public DatoHATEOAS(string link, string description, string method)
        {
            Link = link;
            Description = description;
            Method = method;
        }
    }
}
