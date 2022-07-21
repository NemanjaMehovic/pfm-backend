using System.ComponentModel.DataAnnotations;
using pfm.Models;

namespace pfm.Commands;

public class SplitCommand
{
    [Required]
    public IEnumerable<Split> splits { get; set; }
}