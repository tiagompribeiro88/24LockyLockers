using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Models;

/// <summary>
/// Represents an operator who manages lockers.
/// </summary>
public class Operator
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}