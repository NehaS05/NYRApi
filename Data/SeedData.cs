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
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						},
						new Product 
						{ 
							Name = "Knee Brace Support", 
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						},
						new Product 
						{ 
							Name = "Wrist Support Band", 
							CategoryId = category.Id, 
							BrandId = brand.Id, 
							SupplierId = supplier.Id, 
							ShowInCatalogue = true, 
							IsUniversal = false, 
							IsActive = true 
						}
					};

					await context.Products.AddRangeAsync(products);
					await context.SaveChangesAsync();

					// Create default variants for the products with migrated data
					var variants = new List<ProductVariant>
					{
						new ProductVariant
						{
							ProductId = products[0].Id,
							VariantName = "Default",
							Description = "Adjustable pneumatic walking boot for foot injuries",
							BarcodeSKU = "PWB-001",
							Price = 89.99m,
							IsEnabled = true,
							IsActive = true
						},
						new ProductVariant
						{
							ProductId = products[1].Id,
							VariantName = "Default",
							Description = "Heavy-duty knee brace for sports and recovery",
							BarcodeSKU = "KBS-002",
							Price = 45.50m,
							IsEnabled = true,
							IsActive = true
						},
						new ProductVariant
						{
							ProductId = products[2].Id,
							VariantName = "Default",
							Description = "Compression wrist support for carpal tunnel relief",
							BarcodeSKU = "WSB-003",
							Price = 25.99m,
							IsEnabled = true,
							IsActive = true
						}
					};

					await context.ProductVariants.AddRangeAsync(variants);
					await context.SaveChangesAsync();
				}
			}

			// Note: ProductVariation seeding removed - using ProductVariant system instead

			// Seed Warehouse Inventory
			if (!await context.WarehouseInventories.AnyAsync())
			{
				var warehouses = await context.Warehouses.ToListAsync();
				var products = await context.Products.ToListAsync();

				var warehouseInventories = new List<WarehouseInventory>();

				// Add inventory to first warehouse (without variants for now)
				if (warehouses.Any() && products.Any())
				{
					var firstWarehouse = warehouses.First();
					var selectedProducts = products.Where(p => p.IsUniversal).Take(5).ToList();

					foreach (var product in selectedProducts)
					{
						warehouseInventories.Add(new WarehouseInventory
						{
							WarehouseId = firstWarehouse.Id,
							ProductId = product.Id,
							ProductVariantId = null,
							Quantity = new Random().Next(10, 100),
							Notes = $"Initial stock for {product.Name}",
							IsActive = true
						});
					}

					// Add some inventory to second warehouse
					if (warehouses.Count > 1)
					{
						var secondWarehouse = warehouses[1];
						var secondProducts = products.Where(p => p.IsUniversal).Skip(2).Take(4).ToList();

						foreach (var product in secondProducts)
						{
							warehouseInventories.Add(new WarehouseInventory
							{
								WarehouseId = secondWarehouse.Id,
								ProductId = product.Id,
								ProductVariantId = null,
								Quantity = new Random().Next(5, 50),
								Notes = $"Initial stock for {product.Name}",
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

            // Seed Transfer Inventories
            // Note: Temporarily disabled - needs to be updated to use ProductVariant system
            /*
            if (!await context.TransferInventories.AnyAsync())
            {
                var customers = await context.Customers.ToListAsync();
                var locations = await context.Locations.ToListAsync();
                var products = await context.Products.ToListAsync();

                if (customers.Any() && locations.Any() && products.Any())
                {
                    var transferInventories = new List<TransferInventory>();

                    // Transfer 1: Acme Main Office
                    var acmeCustomer = customers.FirstOrDefault(c => c.CompanyName == "Acme Corporation");
                    var acmeMainOffice = locations.FirstOrDefault(l => l.LocationName == "Acme Main Office");
                    
                    if (acmeCustomer != null && acmeMainOffice != null)
                    {
                        var transfer1 = new TransferInventory
                        {
                            CustomerId = acmeCustomer.Id,
                            LocationId = acmeMainOffice.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-10),
                            IsActive = true,
                            Items = new List<TransferInventoryItem>()
                        };

                        // Add items to transfer 1
                        var product1 = products.FirstOrDefault(p => p.Name == "Pneumatic Walking Boot");
                        if (product1 != null)
                        {
                            // Note: Removed ProductVariation lookups - using universal products
                            transfer1.Items.Add(new TransferInventoryItem
                            {
                                ProductId = product1.Id,
                                ProductVariantId = null,
                                Quantity = 15
                            });

                            transfer1.Items.Add(new TransferInventoryItem
                            {
                                ProductId = product1.Id,
                                ProductVariantId = null,
                                Quantity = 10
                            });

                            transfer1.Items.Add(new TransferInventoryItem
                            {
                                ProductId = product1.Id,
                                ProductVariantId = null,
                                Quantity = 20
                            });
                        }

                        var product2 = products.FirstOrDefault(p => p.Name == "Knee Brace Support");
                        if (product2 != null)
                        {
                            // Variation lookup removed
                                transfer1.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 8
                                });
                            }
                        }

                        transferInventories.Add(transfer1);
                    }

                    // Transfer 2: Acme Warehouse
                    var acmeWarehouse = locations.FirstOrDefault(l => l.LocationName == "Acme Warehouse");
                    
                    if (acmeCustomer != null && acmeWarehouse != null)
                    {
                        var transfer2 = new TransferInventory
                        {
                            CustomerId = acmeCustomer.Id,
                            LocationId = acmeWarehouse.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-5),
                            IsActive = true,
                            Items = new List<TransferInventoryItem>()
                        };

                        // Add items to transfer 2
                        var product2 = products.FirstOrDefault(p => p.Name == "Knee Brace Support");
                        if (product2 != null)
                        {
                            // Variation lookup removed
                                transfer2.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 25
                                });
                            }

                            // Variation lookup removed
                                transfer2.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 18
                                });
                            }
                        }

                        var product3 = products.FirstOrDefault(p => p.Name == "Wrist Support Band");
                        if (product3 != null)
                        {
                            // Variation lookup removed
                                transfer2.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 30
                                });
                            }

                            // Variation lookup removed
                                transfer2.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 22
                                });
                            }
                        }

                        transferInventories.Add(transfer2);
                    }

                    // Transfer 3: Beta Headquarters
                    var betaCustomer = customers.FirstOrDefault(c => c.CompanyName == "Beta Industries");
                    var betaHQ = locations.FirstOrDefault(l => l.LocationName == "Beta Headquarters");
                    
                    if (betaCustomer != null && betaHQ != null)
                    {
                        var transfer3 = new TransferInventory
                        {
                            CustomerId = betaCustomer.Id,
                            LocationId = betaHQ.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-3),
                            IsActive = true,
                            Items = new List<TransferInventoryItem>()
                        };

                        // Add items to transfer 3
                        var product1 = products.FirstOrDefault(p => p.Name == "Pneumatic Walking Boot");
                        if (product1 != null)
                        {
                            // Variation lookup removed
                                transfer3.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 12
                                });
                            }

                            // Variation lookup removed
                                transfer3.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 16
                                });
                            }
                        }

                        var product3 = products.FirstOrDefault(p => p.Name == "Wrist Support Band");
                        if (product3 != null)
                        {
                            // Variation lookup removed
                                transfer3.Items.Add(new TransferInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 14
                                });
                            }
                        }

                        transferInventories.Add(transfer3);
                    }

                    if (transferInventories.Any())
                    {
                        await context.TransferInventories.AddRangeAsync(transferInventories);
                        await context.SaveChangesAsync();
                    }
                }
            }
            */

            // Seed Van Inventories
            // Note: Temporarily disabled - needs to be updated to use ProductVariant system
            /*
            if (!await context.VanInventories.AnyAsync())
            {
                var vans = await context.Vans.ToListAsync();
                var locations = await context.Locations.ToListAsync();
                var products = await context.Products.ToListAsync();
                var productVariations = await context.ProductVariations.ToListAsync();

                if (vans.Any() && locations.Any() && products.Any() && productVariations.Any())
                {
                    var vanInventories = new List<VanInventory>();

                    // Van Inventory 1: Van Alpha to Acme Main Office
                    var vanAlpha = vans.FirstOrDefault(v => v.VanNumber == "VAN-001");
                    var acmeMainOffice = locations.FirstOrDefault(l => l.LocationName == "Acme Main Office");
                    
                    if (vanAlpha != null && acmeMainOffice != null)
                    {
                        var vanInventory1 = new VanInventory
                        {
                            VanId = vanAlpha.Id,
                            LocationId = acmeMainOffice.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-7),
                            IsActive = true,
                            Items = new List<VanInventoryItem>()
                        };

                        // Add items to van inventory 1
                        var product1 = products.FirstOrDefault(p => p.Name == "Pneumatic Walking Boot");
                        if (product1 != null)
                        {
                            // Variation lookup removed
                                vanInventory1.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 10
                                });
                            }

                            // Variation lookup removed
                                vanInventory1.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 8
                                });
                            }

                            // Variation lookup removed
                                vanInventory1.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 15
                                });
                            }
                        }

                        var product2 = products.FirstOrDefault(p => p.Name == "Knee Brace Support");
                        if (product2 != null)
                        {
                            // Variation lookup removed
                                vanInventory1.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 12
                                });
                            }
                        }

                        vanInventories.Add(vanInventory1);
                    }

                    // Van Inventory 2: Van Beta to Acme Warehouse
                    var vanBeta = vans.FirstOrDefault(v => v.VanNumber == "VAN-002");
                    var acmeWarehouse = locations.FirstOrDefault(l => l.LocationName == "Acme Warehouse");
                    
                    if (vanBeta != null && acmeWarehouse != null)
                    {
                        var vanInventory2 = new VanInventory
                        {
                            VanId = vanBeta.Id,
                            LocationId = acmeWarehouse.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-4),
                            IsActive = true,
                            Items = new List<VanInventoryItem>()
                        };

                        // Add items to van inventory 2
                        var product2 = products.FirstOrDefault(p => p.Name == "Knee Brace Support");
                        if (product2 != null)
                        {
                            // Variation lookup removed
                                vanInventory2.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 20
                                });
                            }

                            // Variation lookup removed
                                vanInventory2.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product2.Id,
                                    ProductVariantId = null,
                                    Quantity = 15
                                });
                            }
                        }

                        var product3 = products.FirstOrDefault(p => p.Name == "Wrist Support Band");
                        if (product3 != null)
                        {
                            // Variation lookup removed
                                vanInventory2.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 25
                                });
                            }

                            // Variation lookup removed
                                vanInventory2.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 18
                                });
                            }
                        }

                        vanInventories.Add(vanInventory2);
                    }

                    // Van Inventory 3: Van Alpha to Beta Headquarters
                    var betaHQ = locations.FirstOrDefault(l => l.LocationName == "Beta Headquarters");
                    
                    if (vanAlpha != null && betaHQ != null)
                    {
                        var vanInventory3 = new VanInventory
                        {
                            VanId = vanAlpha.Id,
                            LocationId = betaHQ.Id,
                            TransferDate = DateTime.UtcNow.AddDays(-2),
                            IsActive = true,
                            Items = new List<VanInventoryItem>()
                        };

                        // Add items to van inventory 3
                        var product1 = products.FirstOrDefault(p => p.Name == "Pneumatic Walking Boot");
                        if (product1 != null)
                        {
                            // Variation lookup removed
                                vanInventory3.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 9
                                });
                            }

                            // Variation lookup removed
                                vanInventory3.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product1.Id,
                                    ProductVariantId = null,
                                    Quantity = 14
                                });
                            }
                        }

                        var product3 = products.FirstOrDefault(p => p.Name == "Wrist Support Band");
                        if (product3 != null)
                        {
                            // Variation lookup removed
                                vanInventory3.Items.Add(new VanInventoryItem
                                {
                                    ProductId = product3.Id,
                                    ProductVariantId = null,
                                    Quantity = 11
                                });
                            }
                        }

                        vanInventories.Add(vanInventory3);
                    }

                    if (vanInventories.Any())
                    {
                        await context.VanInventories.AddRangeAsync(vanInventories);
                        await context.SaveChangesAsync();
                    }
                }
            }
            */

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

