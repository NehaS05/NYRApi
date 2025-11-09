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

			// Seed Vans
			if (!await context.Vans.AnyAsync())
			{
				var vans = new List<Van>
				{
					new Van { DefaultDriverName = "Alice Carter", VanName = "Van Alpha", VanNumber = "VAN-001", IsActive = true },
					new Van { DefaultDriverName = "Bob Evans", VanName = "Van Beta", VanNumber = "VAN-002", IsActive = true }
				};

				await context.Vans.AddRangeAsync(vans);
				await context.SaveChangesAsync();
			}

			// Seed Warehouses
			if (!await context.Warehouses.AnyAsync())
			{
				var warehouses = new List<Warehouse>
				{
					new Warehouse { Name = "NY Central Warehouse", AddressLine1 = "100 Warehouse Rd", City = "New York", State = "NY", ZipCode = "10010", IsActive = true },
					new Warehouse { Name = "LA Distribution Center", AddressLine1 = "200 Logistics Ave", City = "Los Angeles", State = "CA", ZipCode = "90012", IsActive = true },
					new Warehouse { Name = "Chicago Storage Hub", AddressLine1 = "300 Industrial Blvd", City = "Chicago", State = "IL", ZipCode = "60601", IsActive = true },
					new Warehouse { Name = "Houston Mega Center", AddressLine1 = "400 Commerce St", City = "Houston", State = "TX", ZipCode = "77001", IsActive = true },
					new Warehouse { Name = "Phoenix Distribution", AddressLine1 = "500 Desert Way", City = "Phoenix", State = "AZ", ZipCode = "85001", IsActive = true }
				};

				await context.Warehouses.AddRangeAsync(warehouses);
				await context.SaveChangesAsync();
			}

			// Seed Suppliers
			if (!await context.Suppliers.AnyAsync())
			{
				var suppliers = new List<Supplier>
				{
					new Supplier { Name = "Global Medical Supplies", Email = "orders@globalmedical.com", PhoneNumber = "555-1001", Address = "100 Medical Dr, Boston, MA 02101", ContactPerson = "John Smith", IsActive = true },
					new Supplier { Name = "HealthTech Solutions", Email = "sales@healthtech.com", PhoneNumber = "555-1002", Address = "200 Tech Ave, Austin, TX 73301", ContactPerson = "Jane Doe", IsActive = true },
					new Supplier { Name = "MedEquip Direct", Email = "info@medequip.com", PhoneNumber = "555-1003", Address = "300 Equipment Blvd, Denver, CO 80201", ContactPerson = "Mike Johnson", IsActive = true }
				};

				await context.Suppliers.AddRangeAsync(suppliers);
				await context.SaveChangesAsync();
			}

			// Seed Products
			if (!await context.Products.AnyAsync())
			{
				var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Health & Beauty");
				var brand = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Apple");
				var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Global Medical Supplies");

				if (category != null && brand != null && supplier != null)
				{
					var products = new List<Product>
					{
						new Product 
						{ 
							Name = "Pneumatic Walking Boot", 
							Description = "Adjustable pneumatic walking boot for foot injuries", 
							BarcodeSKU = "PWB-001", 
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							Price = 89.99m, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						},
						new Product 
						{ 
							Name = "Knee Brace Support", 
							Description = "Heavy-duty knee brace for sports and recovery", 
							BarcodeSKU = "KBS-002", 
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							Price = 45.50m, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						},
						new Product 
						{ 
							Name = "Wrist Support Band", 
							Description = "Compression wrist support for carpal tunnel relief", 
							BarcodeSKU = "WSB-003", 
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							Price = 25.99m, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						}
					};

					await context.Products.AddRangeAsync(products);
					await context.SaveChangesAsync();
				}
			}

			// Seed Product Variations
			if (!await context.ProductVariations.AnyAsync())
			{
				var products = await context.Products.ToListAsync();
				var productVariations = new List<ProductVariation>();

				foreach (var product in products)
				{
					// Add size variations
					productVariations.AddRange(new List<ProductVariation>
					{
						new ProductVariation { ProductId = product.Id, VariationType = "Size", VariationValue = "Small", SKU = $"{product.BarcodeSKU}-S", PriceAdjustment = 0, StockQuantity = 0, IsActive = true },
						new ProductVariation { ProductId = product.Id, VariationType = "Size", VariationValue = "Medium", SKU = $"{product.BarcodeSKU}-M", PriceAdjustment = 0, StockQuantity = 0, IsActive = true },
						new ProductVariation { ProductId = product.Id, VariationType = "Size", VariationValue = "Large", SKU = $"{product.BarcodeSKU}-L", PriceAdjustment = 5.00m, StockQuantity = 0, IsActive = true },
						new ProductVariation { ProductId = product.Id, VariationType = "Size", VariationValue = "Extra Large", SKU = $"{product.BarcodeSKU}-XL", PriceAdjustment = 10.00m, StockQuantity = 0, IsActive = true }
					});

					// Add color variations for first product
					if (product.Name == "Pneumatic Walking Boot")
					{
						productVariations.AddRange(new List<ProductVariation>
						{
							new ProductVariation { ProductId = product.Id, VariationType = "Color", VariationValue = "Black", SKU = $"{product.BarcodeSKU}-BLK", PriceAdjustment = 0, StockQuantity = 0, IsActive = true },
							new ProductVariation { ProductId = product.Id, VariationType = "Color", VariationValue = "White", SKU = $"{product.BarcodeSKU}-WHT", PriceAdjustment = 0, StockQuantity = 0, IsActive = true },
							new ProductVariation { ProductId = product.Id, VariationType = "Color", VariationValue = "Blue", SKU = $"{product.BarcodeSKU}-BLU", PriceAdjustment = 0, StockQuantity = 0, IsActive = true }
						});
					}
				}

				await context.ProductVariations.AddRangeAsync(productVariations);
				await context.SaveChangesAsync();
			}

			// Seed Warehouse Inventory
			if (!await context.WarehouseInventories.AnyAsync())
			{
				var warehouses = await context.Warehouses.ToListAsync();
				var productVariations = await context.ProductVariations.ToListAsync();

				var warehouseInventories = new List<WarehouseInventory>();

				// Add inventory to first warehouse
				if (warehouses.Any() && productVariations.Any())
				{
					var firstWarehouse = warehouses.First();
					var selectedVariations = productVariations.Take(8).ToList(); // Take first 8 variations

					foreach (var variation in selectedVariations)
					{
						warehouseInventories.Add(new WarehouseInventory
						{
							WarehouseId = firstWarehouse.Id,
							ProductId = variation.ProductId,
							ProductVariationId = variation.Id,
							Quantity = new Random().Next(5, 50),
							Notes = $"Initial stock for {variation.VariationType}: {variation.VariationValue}",
							IsActive = true
						});
					}

					// Add some inventory to second warehouse
					if (warehouses.Count > 1)
					{
						var secondWarehouse = warehouses[1];
						var secondVariations = productVariations.Skip(4).Take(6).ToList();

						foreach (var variation in secondVariations)
						{
							warehouseInventories.Add(new WarehouseInventory
							{
								WarehouseId = secondWarehouse.Id,
								ProductId = variation.ProductId,
								ProductVariationId = variation.Id,
								Quantity = new Random().Next(3, 30),
								Notes = $"Initial stock for {variation.VariationType}: {variation.VariationValue}",
								IsActive = true
							});
						}
					}
				}

				await context.WarehouseInventories.AddRangeAsync(warehouseInventories);
				await context.SaveChangesAsync();
			}

            // Seed Scanners
            if (!await context.Scanners.AnyAsync())
            {
                var locations = await context.Locations.ToListAsync();
                if (locations.Any())
                {
                    var scanners = new List<Scanner>();
                    
                    var firstLocation = locations.First();
                    scanners.Add(new Scanner
                    {
                        SerialNo = "SCAN-001",
                        ScannerName = "Main Office Scanner",
                        ScannerPIN = "1234",
                        LocationId = firstLocation.Id,
                        IsActive = true
                    });

                    if (locations.Count > 1)
                    {
                        var secondLocation = locations.Skip(1).First();
                        scanners.Add(new Scanner
                        {
                            SerialNo = "SCAN-002",
                            ScannerName = "Warehouse Scanner",
                            ScannerPIN = "5678",
                            LocationId = secondLocation.Id,
                            IsActive = true
                        });
                    }

                    if (locations.Count > 2)
                    {
                        var thirdLocation = locations.Skip(2).First();
                        scanners.Add(new Scanner
                        {
                            SerialNo = "SCAN-003",
                            ScannerName = "HQ Scanner",
                            ScannerPIN = "9012",
                            LocationId = thirdLocation.Id,
                            IsActive = true
                        });
                    }

                    await context.Scanners.AddRangeAsync(scanners);
                    await context.SaveChangesAsync();
                }
            }

            // Seed Variations
            if (!await context.Variations.AnyAsync())
            {
                var variations = new List<Variation>
                {
                    new Variation
                    {
                        Name = "Size",
                        ValueType = "Dropdown",
                        IsActive = true,
                        Options = new List<VariationOption>
                        {
                            new VariationOption { Name = "Small", Value = "S", IsActive = true },
                            new VariationOption { Name = "Medium", Value = "M", IsActive = true },
                            new VariationOption { Name = "Large", Value = "L", IsActive = true },
                            new VariationOption { Name = "Extra Large", Value = "XL", IsActive = true },
                            new VariationOption { Name = "XXL", Value = "XXL", IsActive = true }
                        }
                    },
                    new Variation
                    {
                        Name = "Color",
                        ValueType = "Dropdown",
                        IsActive = true,
                        Options = new List<VariationOption>
                        {
                            new VariationOption { Name = "Black", Value = "BLK", IsActive = true },
                            new VariationOption { Name = "White", Value = "WHT", IsActive = true },
                            new VariationOption { Name = "Blue", Value = "BLU", IsActive = true },
                            new VariationOption { Name = "Red", Value = "RED", IsActive = true },
                            new VariationOption { Name = "Green", Value = "GRN", IsActive = true },
                            new VariationOption { Name = "Gray", Value = "GRY", IsActive = true }
                        }
                    },
                    new Variation
                    {
                        Name = "Material",
                        ValueType = "Dropdown",
                        IsActive = true,
                        Options = new List<VariationOption>
                        {
                            new VariationOption { Name = "Cotton", Value = "Cotton", IsActive = true },
                            new VariationOption { Name = "Polyester", Value = "Polyester", IsActive = true },
                            new VariationOption { Name = "Nylon", Value = "Nylon", IsActive = true },
                            new VariationOption { Name = "Leather", Value = "Leather", IsActive = true },
                            new VariationOption { Name = "Plastic", Value = "Plastic", IsActive = true }
                        }
                    },
                    new Variation
                    {
                        Name = "Weight",
                        ValueType = "TextInput",
                        IsActive = true,
                        Options = new List<VariationOption>
                        {
                            new VariationOption { Name = "Weight (kg)", Value = null, IsActive = true }
                        }
                    },
                    new Variation
                    {
                        Name = "Dimensions",
                        ValueType = "TextInput",
                        IsActive = true,
                        Options = new List<VariationOption>
                        {
                            new VariationOption { Name = "Length (cm)", Value = null, IsActive = true },
                            new VariationOption { Name = "Width (cm)", Value = null, IsActive = true },
                            new VariationOption { Name = "Height (cm)", Value = null, IsActive = true }
                        }
                    }
                };

                await context.Variations.AddRangeAsync(variations);
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
