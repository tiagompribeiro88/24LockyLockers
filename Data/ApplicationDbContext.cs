using _24LockyLockers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Locker> Lockers => Set<Locker>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Locker>(entity =>
        {
            entity.HasOne(l => l.Location)
                .WithMany(loc => loc.Lockers)
                .HasForeignKey(l => l.LocationId);
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Locker)
                .WithMany()
                .HasForeignKey(r => r.LockerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}