using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Events.Api.Models
{
    public class MovieEvent
    {
        public MovieEvent()
        {
            Genres = Array.Empty<string>();
            Description = string.Empty;
        }

        [Required]
        [JsonPropertyName("movie_id")]
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [Required]
        public double Rating { get; set; }
        public string[] Genres { get; set; }
        public string Description { get; set; }  
    }
}
