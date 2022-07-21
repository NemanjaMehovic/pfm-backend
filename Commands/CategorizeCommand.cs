using System.ComponentModel.DataAnnotations;

namespace pfm.Commands;

public class CategorizeCommand
{
    [Required]
    public string catcode { get; set; }
}