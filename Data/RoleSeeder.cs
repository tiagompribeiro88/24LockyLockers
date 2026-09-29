using _24LockyLockers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _24LockyLockers.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        // Create roles
        var roles = new[] { "Admin", "Operator", "Client" };

        foreach (var roleName in roles)
        {
            var roleExists = await context.Roles.AnyAsync(r => r.Name == roleName);
            if (!roleExists)
            {
                await context.Roles.AddAsync(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
                await context.SaveChangesAsync();
            }
        }

        // Create admin user
        var adminEmail = "admin@24lockylockers.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create operator user
        var operatorEmail = "operator@24lockylockers.com";
        if (await userManager.FindByEmailAsync(operatorEmail) == null)
        {
            var operatorUser = new IdentityUser
            {
                UserName = operatorEmail,
                Email = operatorEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(operatorUser, "Operator123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(operatorUser, "Operator");
            }
        }

        // Create client user
        var clientEmail = "client@24lockylockers.com";
        if (await userManager.FindByEmailAsync(clientEmail) == null)
        {
            var clientUser = new IdentityUser
            {
                UserName = clientEmail,
                Email = clientEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(clientUser, "Client123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(clientUser, "Client");
            }
        }

        // Seed locations and lockers
        if (!context.Locations.Any())
        {
            var locations = new List<Location>
            {
                // Portugal
                new Location
                {
                    CountryCode = "PT",
                    CountryName = "Portugal",
                    CityCode = "OPO",
                    CityName = "Porto",
                    LocationName = "Francisco Sá Carneiro Airport",
                    Lockers = GenerateLockers("PT", "OPO", 10)
                },
                new Location
                {
                    CountryCode = "PT",
                    CountryName = "Portugal",
                    CityCode = "LIS",
                    CityName = "Lisbon",
                    LocationName = "Humberto Delgado Airport",
                    Lockers = GenerateLockers("PT", "LIS", 15)
                },
                new Location
                {
                    CountryCode = "PT",
                    CountryName = "Portugal",
                    CityCode = "CPB",  // <-- MUDA DE "CBP" PARA "CPB"
                    CityName = "Coimbra",
                    LocationName = "Coimbra Train Station",
                    Lockers = GenerateLockers("PT", "CPB", 8)
                },
                // Spain
                new Location
                {
                    CountryCode = "ES",
                    CountryName = "Spain",
                    CityCode = "BCN",
                    CityName = "Barcelona",
                    LocationName = "Josep Tarradellas Barcelona-El Prat Airport",
                    Lockers = GenerateLockers("ES", "BCN", 20)
                },
                new Location
                {
                    CountryCode = "ES",
                    CountryName = "Spain",
                    CityCode = "MAD",
                    CityName = "Madrid",
                    LocationName = "Adolfo Suárez Madrid-Barajas Airport",
                    Lockers = GenerateLockers("ES", "MAD", 25)
                },
                new Location
                {
                    CountryCode = "ES",
                    CountryName = "Spain",
                    CityCode = "SCQ",
                    CityName = "Santiago de Compostela",
                    LocationName = "Santiago de Compostela Train Station",
                    Lockers = GenerateLockers("ES", "SCQ", 12)
                }
            };

            context.Locations.AddRange(locations);
            await context.SaveChangesAsync();
        }

        // Seed operators
        if (!context.Operators.Any())
        {
            var operators = new List<Operator>
            {
                new Operator { Name = "John Doe", Email = "john.doe@24lockylockers.com", IsActive = true },
                new Operator { Name = "Jane Smith", Email = "jane.smith@24lockylockers.com", IsActive = true },
                new Operator { Name = "Bob Wilson", Email = "bob.wilson@24lockylockers.com", IsActive = false },
            };

            context.Operators.AddRange(operators);
            await context.SaveChangesAsync();
        }
    }

    private static List<Locker> GenerateLockers(string countryCode, string cityCode, int count)
    {
        var lockers = new List<Locker>();
        for (int i = 1; i <= count; i++)
        {
            lockers.Add(new Locker
            {
                LockerNumber = $"{countryCode}-{cityCode}-{i:D3}", // e.g., PT-OPO-001
                IsAvailable = true,
                CurrentCode = null,
                LastUsed = null
            });
        }
        return lockers;
    }
}