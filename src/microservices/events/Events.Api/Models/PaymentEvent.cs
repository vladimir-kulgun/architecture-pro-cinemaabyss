using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Events.Api.Models
{
    public class PaymentEvent
    {
        [Required]
        [JsonPropertyName("payment_id")]
        public int PaymentId { get; set; }
        [Required]
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        [JsonPropertyName("method_type")]
        public string MethodType { get; set; }
    }
}
