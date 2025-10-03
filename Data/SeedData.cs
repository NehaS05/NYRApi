using Microsoft.EntityFrameworkCore;
using NYR.API.Models.Entities;
using System.Security.Cryptography;

namespace NYR.API.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin", Description = "System Administrator", IsActive = true },
                    new Role { Name = "Customer", Description = "Customer User", IsActive = true },
                    new Role { Name = "Staff", Description = "Customer Staff", IsActive = true },
                    new Role { Name = "Driver", Description = "Driver User", IsActive = true }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // Seed Customers
            if (!await context.Customers.AnyAsync())
            {
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        CompanyName = "Acme Corporation",
                        DBA = "Acme Corp",
                        AccountNumber = "ACME001",
                        AddressLine1 = "123 Main Street",
                        City = "New York",
                        State = "NY",
                        ZipCode = "10001",
                        ContactName = "John Smith",
                        JobTitle = "CEO",
                        BusinessPhone = "555-0101",
                        MobilePhone = "555-0102",
                        Email = "john.smith@acme.com",
                        Website = "www.acme.com",
                        IsActive = true
                    },
                    new Customer
                    {
                        CompanyName = "Beta Industries",
                        DBA = "Beta Inc",
                        AccountNumber = "BETA001",
                        AddressLine1 = "456 Oak Avenue",
                        AddressLine2 = "Suite 200",
                        City = "Los Angeles",
                        State = "CA",
                        ZipCode = "90210",
                        ContactName = "Jane Doe",
                        JobTitle = "Operations Manager",
                        BusinessPhone = "555-0201",
                        MobilePhone = "555-0202",
                        Email = "jane.doe@beta.com",
                        Website = "www.beta.com",
                        IsActive = true
                    }
                };

                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();
            }

            // Seed Locations
            if (!await context.Locations.AnyAsync())
            {
                var customers = await context.Customers.ToListAsync();
                var acmeCustomer = customers.FirstOrDefault(c => c.CompanyName == "Acme Corporation");
                var betaCustomer = customers.FirstOrDefault(c => c.CompanyName == "Beta Industries");

                var locations = new List<Location>();

                if (acmeCustomer != null)
                {
                    locations.Add(new Location
                    {
                        CustomerId = acmeCustomer.Id,
                        LocationName = "Acme Main Office",
                        AddressLine1 = "123 Main Street",
                        City = "New York",
                        State = "NY",
                        ZipCode = "10001",
                        ContactPerson = "John Smith",
                        Title = "CEO",
                        LocationPhone = "555-0101",
                        Email = "main@acme.com",
                        IsActive = true
                    });

                    locations.Add(new Location
                    {
                        CustomerId = acmeCustomer.Id,
                        LocationName = "Acme Warehouse",
                        AddressLine1 = "789 Industrial Blvd",
                        City = "Brooklyn",
                        State = "NY",
                        ZipCode = "11201",
                        ContactPerson = "Mike Johnson",
                        Title = "Warehouse Manager",
                        LocationPhone = "555-0103",
                        Email = "warehouse@acme.com",
                        IsActive = true
                    });
                }

                if (betaCustomer != null)
                {
                    locations.Add(new Location
                    {
                        CustomerId = betaCustomer.Id,
                        LocationName = "Beta Headquarters",
                        AddressLine1 = "456 Oak Avenue",
                        AddressLine2 = "Suite 200",
                        City = "Los Angeles",
                        State = "CA",
                        ZipCode = "90210",
                        ContactPerson = "Jane Doe",
                        Title = "Operations Manager",
                        LocationPhone = "555-0201",
                        Email = "hq@beta.com",
                        IsActive = true
                    });
                }

                if (locations.Any())
                {
                    await context.Locations.AddRangeAsync(locations);
                    await context.SaveChangesAsync();
                }
            }

            // Seed Categories (Master Data)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true },
                    new Category { Name = "Clothing", Description = "Apparel and fashion items", IsActive = true },
                    new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
                    new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear", IsActive = true },
                    new Category { Name = "Books & Media", Description = "Books, movies, and media", IsActive = true },
                    new Category { Name = "Health & Beauty", Description = "Health and beauty products", IsActive = true },
                    new Category { Name = "Automotive", Description = "Automotive parts and accessories", IsActive = true },
                    new Category { Name = "Toys & Games", Description = "Toys and gaming products", IsActive = true }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed Brands (Master Data)
            if (!await context.Brands.AnyAsync())
            {
                var brands = new List<Brand>
                {
                    new Brand { Name = "Apple", Description = "Technology company", IsActive = true },
                    new Brand { Name = "Samsung", Description = "Electronics and technology", IsActive = true },
                    new Brand { Name = "Nike", Description = "Athletic footwear and apparel", IsActive = true },
                    new Brand { Name = "Adidas", Description = "Sports and lifestyle brand", IsActive = true },
                    new Brand { Name = "Sony", Description = "Electronics and entertainment", IsActive = true },
                    new Brand { Name = "Microsoft", Description = "Technology and software", IsActive = true },
                    new Brand { Name = "LG", Description = "Electronics and home appliances", IsActive = true },
                    new Brand { Name = "Canon", Description = "Cameras and imaging equipment", IsActive = true },
                    new Brand { Name = "Dell", Description = "Computer technology", IsActive = true },
                    new Brand { Name = "HP", Description = "Technology and printing solutions", IsActive = true }
                };

                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }

            // Seed Admin User
            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    var adminUser = new User
                    {
                        Name = "System Administrator",
                        Email = "admin@nyr.com",
                        PhoneNumber = "555-0000",
                        PasswordHash = HashPassword("Admin123!"),
                        RoleId = adminRole.Id,
                        IsActive = true
                    };

                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
