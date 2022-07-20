using System.ComponentModel.DataAnnotations;

namespace pfm.Models;

public class Category
{
    [Required]
    public string? code { get; set; }
    public string? parent_code { get; set; }
    [Required]
    public string? name { get; set; }
}