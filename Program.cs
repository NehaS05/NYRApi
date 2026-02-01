using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NYR.API.Data;
using NYR.API.Mappings;
using NYR.API.Models.Configuration;
using NYR.API.Repositories;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services;
using NYR.API.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NYR API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Memory Cache for PIN service
builder.Services.AddMemoryCache();

// Repository Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IVanRepository, VanRepository>();
    builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
    builder.Services.AddScoped<IWarehouseInventoryRepository, WarehouseInventoryRepository>();
    builder.Services.AddScoped<IDriverAvailabilityRepository, DriverAvailabilityRepository>();
    builder.Services.AddScoped<IScannerRepository, ScannerRepository>();
    builder.Services.AddScoped<IVariationRepository, VariationRepository>();
    builder.Services.AddScoped<ITransferInventoryRepository, TransferInventoryRepository>();
    builder.Services.AddScoped<IVariationOptionRepository, VariationOptionRepository>();
    builder.Services.AddScoped<IRequestSupplyRepository, RequestSupplyRepository>();
    builder.Services.AddScoped<IRequestSupplyItemRepository, RequestSupplyItemRepository>();
    builder.Services.AddScoped<IRouteRepository, RouteRepository>();
    builder.Services.AddScoped<IRouteStopRepository, RouteStopRepository>();
    builder.Services.AddScoped<ILocationInventoryDataRepository, LocationInventoryDataRepository>();
    builder.Services.AddScoped<ILocationOutwardInventoryRepository, LocationOutwardInventoryRepository>();
    builder.Services.AddScoped<ILocationUnlistedInventoryRepository, LocationUnlistedInventoryRepository>();
    builder.Services.AddScoped<IVanInventoryRepository, VanInventoryRepository>();
    builder.Services.AddScoped<IRestockRequestRepository, RestockRequestRepository>();
    builder.Services.AddScoped<IFollowupRequestRepository, FollowupRequestRepository>();

// Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IVanService, VanService>();
    builder.Services.AddScoped<IWarehouseService, WarehouseService>();
    builder.Services.AddScoped<IWarehouseInventoryService, WarehouseInventoryService>();
    builder.Services.AddScoped<IDriverAvailabilityService, DriverAvailabilityService>();
    builder.Services.AddScoped<IScannerService, ScannerService>();
    builder.Services.AddScoped<IVariationService, VariationService>();
    builder.Services.AddScoped<ITransferInventoryService, TransferInventoryService>();
    builder.Services.AddScoped<IRequestSupplyService, RequestSupplyService>();
    builder.Services.AddScoped<IVanInventoryService, VanInventoryService>();
    builder.Services.AddScoped<IRouteService, RouteService>();
    builder.Services.AddScoped<ILocationInventoryDataService, LocationInventoryDataService>();
    builder.Services.AddScoped<ILocationOutwardInventoryService, LocationOutwardInventoryService>();
    builder.Services.AddScoped<ILocationUnlistedInventoryService, LocationUnlistedInventoryService>();
    builder.Services.AddScoped<IRestockRequestService, RestockRequestService>();
    builder.Services.AddScoped<IFollowupRequestService, FollowupRequestService>();
    builder.Services.AddScoped<ITransferService, TransferService>();
    builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Configure Spoke API Settings
builder.Services.Configure<SpokeApiSettings>(builder.Configuration.GetSection("SpokeApi"));

// Register HttpClient for Spoke API
builder.Services.AddHttpClient<ISpokeApiService, SpokeApiService>();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

// Enable static file serving
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply pending EF Core migrations
    await context.Database.MigrateAsync();

    // Seed initial data
    await SeedData.SeedAsync(context);
}

app.Run();
