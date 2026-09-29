using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _24LockyLockers.Models;

/// <summary>
/// Represents a locker rental made by a client.
/// </summary>
public class Rental
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    [Required]
    public int LockerId { get; set; }

    [ForeignKey("LockerId")]
    public Locker? Locker { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(10)]
    public string AccessCode { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; } = true;
}