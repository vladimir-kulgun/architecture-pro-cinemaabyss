using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Events.Api.Models
{
    public class UserEvent
    {
        [Required]
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
