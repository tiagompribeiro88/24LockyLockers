using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _24LockyLockers.Models;

public class Locker
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^[A-Z]{2}-[A-Z]{3}-\d{3}$", ErrorMessage = "Locker number must be in format XX-XXX-000 (e.g., PT-OPO-001)")]
    public string LockerNumber { get; set; } = ""; // e.g., PT-OPO-001

    public bool IsAvailable { get; set; } = true;

    [StringLength(10)]
    public string? CurrentCode { get; set; }

    public DateTime? LastUsed { get; set; }

    [Required]
    public int LocationId { get; set; }

    [ForeignKey("LocationId")]
    public Location? Location { get; set; }
}