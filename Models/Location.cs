using System.ComponentModel.DataAnnotations;

namespace _24LockyLockers.Models;

public class Location
{
    public int Id { get; set; }

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = ""; // PT or ES

    [Required]
    [StringLength(100)]
    public string CountryName { get; set; } = ""; // Portugal or Spain

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string CityCode { get; set; } = ""; // OPO, LIS, etc.

    [Required]
    [StringLength(100)]
    public string CityName { get; set; } = ""; // Porto, Lisbon, etc.

    [Required]
    [StringLength(200)]
    public string LocationName { get; set; } = ""; // Airport, Train Station, etc.

    public List<Locker> Lockers { get; set; } = new();
}