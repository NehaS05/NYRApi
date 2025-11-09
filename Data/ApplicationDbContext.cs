using Microsoft.EntityFrameworkCore;
using NYR.API.Models.Entities;

namespace NYR.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
		public DbSet<Van> Vans { get; set; }
		public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }
        public DbSet<DriverAvailability> DriverAvailabilities { get; set; }
        public DbSet<Scanner> Scanners { get; set; }
        public DbSet<Variation> Variations { get; set; }
        public DbSet<VariationOption> VariationOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Location entity
            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Brand entity
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Supplier entity
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // Configure ProductVariation entity
            modelBuilder.Entity<ProductVariation>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.PriceAdjustment).HasPrecision(18, 2);
            });

			// Configure Van entity
			modelBuilder.Entity<Van>(entity =>
			{
				entity.HasIndex(e => e.VanNumber).IsUnique();
				entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
			});

			// Configure Warehouse entity
			modelBuilder.Entity<Warehouse>(entity =>
			{
				entity.HasIndex(e => e.Name);
				entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
			});

            // Configure WarehouseInventory entity
            modelBuilder.Entity<WarehouseInventory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => new { e.WarehouseId, e.ProductVariationId }).IsUnique();
            });

            // Configure DriverAvailability entity
            modelBuilder.Entity<DriverAvailability>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Scanner entity
            modelBuilder.Entity<Scanner>(entity =>
            {
                entity.HasIndex(e => e.SerialNo).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Variation entity
            modelBuilder.Entity<Variation>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure VariationOption entity
            modelBuilder.Entity<VariationOption>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Location)
                .WithMany(l => l.Users)
                .HasForeignKey(u => u.LocationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Customer)
                .WithMany(c => c.Locations)
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Product relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductVariation>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variations)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DriverAvailability>()
                .HasOne(d => d.User)
                .WithMany(u => u.DriverAvailabilities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure WarehouseInventory relationships
            modelBuilder.Entity<WarehouseInventory>()
                .HasOne(wi => wi.Warehouse)
                .WithMany()
                .HasForeignKey(wi => wi.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseInventory>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseInventory>()
                .HasOne(wi => wi.ProductVariation)
                .WithMany()
                .HasForeignKey(wi => wi.ProductVariationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Scanner relationships
            modelBuilder.Entity<Scanner>()
                .HasOne(s => s.Location)
                .WithMany()
                .HasForeignKey(s => s.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Variation relationships
            modelBuilder.Entity<VariationOption>()
                .HasOne(vo => vo.Variation)
                .WithMany(v => v.Options)
                .HasForeignKey(vo => vo.VariationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
