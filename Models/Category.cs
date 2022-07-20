using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pfm.Models;

public class Category
{
    [Required]
    public string? code { get; set; }
    [JsonPropertyName("parent-code")]
    public string? parent_code { get; set; }
    [Required]
    public string? name { get; set; }
}