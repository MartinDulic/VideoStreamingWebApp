using System.ComponentModel.DataAnnotations;

namespace DAL.Responses
{
    public class VideoResponse
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int TotalTime { get; set; }
        public string? StreamingUrl { get; set; }
        public string? Genre { get; set; }
        public string? Tags { get; set; }
    }
}
