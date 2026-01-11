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
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
		public DbSet<Van> Vans { get; set; }
		public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }
        public DbSet<DriverAvailability> DriverAvailabilities { get; set; }
        public DbSet<Scanner> Scanners { get; set; }
        public DbSet<Variation> Variations { get; set; }
        public DbSet<VariationOption> VariationOptions { get; set; }
        public DbSet<TransferInventory> TransferInventories { get; set; }
        public DbSet<TransferInventoryItem> TransferInventoryItems { get; set; }
        public DbSet<RequestSupply> RequestSupplies { get; set; }
        public DbSet<RequestSupplyItem> RequestSupplyItems { get; set; }
        public DbSet<VanInventory> VanInventories { get; set; }
        public DbSet<VanInventoryItem> VanInventoryItems { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<LocationInventoryData> LocationInventoryData { get; set; }
        public DbSet<LocationOutwardInventory> LocationOutwardInventories { get; set; }
        public DbSet<LocationUnlistedInventory> LocationUnlistedInventories { get; set; }
        public DbSet<RestockRequest> RestockRequests { get; set; }
        public DbSet<RestockRequestItem> RestockRequestItems { get; set; }
        public DbSet<FollowupRequest> FollowupRequests { get; set; }

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
                entity.HasIndex(e => new { e.WarehouseId, e.ProductId, e.ProductVariantId }).IsUnique();
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

            modelBuilder.Entity<Location>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Configure ProductVariant relationships
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ProductVariantAttribute relationships
            modelBuilder.Entity<ProductVariantAttribute>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pva => pva.ProductVariant)
                .WithMany(pv => pv.Attributes)
                .HasForeignKey(pva => pva.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pva => pva.Variation)
                .WithMany()
                .HasForeignKey(pva => pva.VariationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pva => pva.VariationOption)
                .WithMany()
                .HasForeignKey(pva => pva.VariationOptionId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasOne(wi => wi.ProductVariant)
                .WithMany()
                .HasForeignKey(wi => wi.ProductVariantId)
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

            // Configure TransferInventory entity
            modelBuilder.Entity<TransferInventory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure TransferInventoryItem entity
            modelBuilder.Entity<TransferInventoryItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure TransferInventory relationships
            modelBuilder.Entity<TransferInventory>()
                .HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferInventory>()
                .HasOne(t => t.Location)
                .WithMany()
                .HasForeignKey(t => t.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferInventoryItem>()
                .HasOne(ti => ti.TransferInventory)
                .WithMany(t => t.Items)
                .HasForeignKey(ti => ti.TransferInventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransferInventoryItem>()
                .HasOne(ti => ti.Product)
                .WithMany()
                .HasForeignKey(ti => ti.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferInventoryItem>()
                .HasOne(ti => ti.ProductVariant)
                .WithMany()
                .HasForeignKey(ti => ti.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RequestSupply entity
            modelBuilder.Entity<RequestSupply>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure RequestSupplyItem entity
            modelBuilder.Entity<RequestSupplyItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure RequestSupply relationships
            modelBuilder.Entity<RequestSupply>()
                .HasOne(rs => rs.Product)
                .WithMany()
                .HasForeignKey(rs => rs.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestSupply>()
                .HasOne(rs => rs.Supplier)
                .WithMany()
                .HasForeignKey(rs => rs.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestSupplyItem>()
                .HasOne(rsi => rsi.RequestSupply)
                .WithMany(rs => rs.Items)
                .HasForeignKey(rsi => rsi.RequestSupplyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestSupplyItem>()
                .HasOne(rsi => rsi.Variation)
                .WithMany()
                .HasForeignKey(rsi => rsi.VariationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure VanInventory entity
            modelBuilder.Entity<VanInventory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure VanInventoryItem entity
            modelBuilder.Entity<VanInventoryItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure VanInventory relationships
            modelBuilder.Entity<VanInventory>()
                .HasOne(vi => vi.Van)
                .WithMany()
                .HasForeignKey(vi => vi.VanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VanInventory>()
                .HasOne(vi => vi.Location)
                .WithMany()
                .HasForeignKey(vi => vi.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VanInventoryItem>()
                .HasOne(vii => vii.VanInventory)
                .WithMany(vi => vi.Items)
                .HasForeignKey(vii => vii.VanInventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VanInventoryItem>()
                .HasOne(vii => vii.Product)
                .WithMany()
                .HasForeignKey(vii => vii.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VanInventoryItem>()
                .HasOne(vii => vii.ProductVariant)
                .WithMany()
                .HasForeignKey(vii => vii.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Routes entity
            modelBuilder.Entity<Routes>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure RouteStop entity
            modelBuilder.Entity<RouteStop>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Routes relationships
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStops)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Location)
                .WithMany()
                .HasForeignKey(rs => rs.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Customer)
                .WithMany()
                .HasForeignKey(rs => rs.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.RestockRequest)
                .WithMany()
                .HasForeignKey(rs => rs.RestockRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.FollowupRequest)
                .WithMany()
                .HasForeignKey(rs => rs.FollowupRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure LocationInventoryData entity
            modelBuilder.Entity<LocationInventoryData>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure LocationInventoryData relationships
            modelBuilder.Entity<LocationInventoryData>()
                .HasOne(l => l.Location)
                .WithMany()
                .HasForeignKey(l => l.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationInventoryData>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationInventoryData>()
                .HasOne(l => l.CreatedByUser)
                .WithMany()
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LocationInventoryData>()
                .HasOne(l => l.UpdatedByUser)
                .WithMany()
                .HasForeignKey(l => l.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RestockRequest entity
            modelBuilder.Entity<RestockRequest>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure RestockRequestItem entity
            modelBuilder.Entity<RestockRequestItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure RestockRequest relationships
            modelBuilder.Entity<RestockRequest>()
                .HasOne(rr => rr.Customer)
                .WithMany()
                .HasForeignKey(rr => rr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestockRequest>()
                .HasOne(rr => rr.Location)
                .WithMany()
                .HasForeignKey(rr => rr.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestockRequestItem>()
                .HasOne(rri => rri.RestockRequest)
                .WithMany(rr => rr.Items)
                .HasForeignKey(rri => rri.RestockRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RestockRequestItem>()
                .HasOne(rri => rri.Product)
                .WithMany()
                .HasForeignKey(rri => rri.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestockRequestItem>()
                .HasOne(rri => rri.ProductVariant)
                .WithMany()
                .HasForeignKey(rri => rri.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure LocationInventoryData relationships for ProductVariant
            modelBuilder.Entity<LocationInventoryData>()
                .HasOne(l => l.ProductVariant)
                .WithMany()
                .HasForeignKey(l => l.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure FollowupRequest entity
            modelBuilder.Entity<FollowupRequest>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure FollowupRequest relationships
            modelBuilder.Entity<FollowupRequest>()
                .HasOne(fr => fr.Customer)
                .WithMany()
                .HasForeignKey(fr => fr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FollowupRequest>()
                .HasOne(fr => fr.Location)
                .WithMany()
                .HasForeignKey(fr => fr.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure LocationOutwardInventory entity
            modelBuilder.Entity<LocationOutwardInventory>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure LocationOutwardInventory relationships
            modelBuilder.Entity<LocationOutwardInventory>()
                .HasOne(l => l.Location)
                .WithMany()
                .HasForeignKey(l => l.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationOutwardInventory>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationOutwardInventory>()
                .HasOne(l => l.CreatedByUser)
                .WithMany()
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationOutwardInventory>()
                .HasOne(l => l.UpdatedByUser)
                .WithMany()
                .HasForeignKey(l => l.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure LocationUnlistedInventory entity
            modelBuilder.Entity<LocationUnlistedInventory>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure LocationUnlistedInventory relationships
            modelBuilder.Entity<LocationUnlistedInventory>()
                .HasOne(l => l.Location)
                .WithMany()
                .HasForeignKey(l => l.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationUnlistedInventory>()
                .HasOne(l => l.CreatedByUser)
                .WithMany()
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationUnlistedInventory>()
                .HasOne(l => l.UpdatedByUser)
                .WithMany()
                .HasForeignKey(l => l.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
