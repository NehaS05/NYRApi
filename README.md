# NYR Backend API

A comprehensive .NET 8 Web API project with role-based authentication, JWT tokens, and Entity Framework Core.

## Features

- **Role-based Authentication**: Admin, Customer, Staff, Driver roles
- **JWT Token Authentication**: Secure API access with token-based authentication
- **Entity Framework Core**: Code-first approach with migrations
- **Repository Pattern**: Clean architecture with repository and service layers
- **AutoMapper**: Object-to-object mapping for DTOs
- **Swagger Documentation**: Interactive API documentation
- **Seed Data**: Initial data setup with roles, customers, and locations

## Project Structure

```
NYR.API/
├── Controllers/           # API Controllers
├── Data/                 # DbContext and Seed Data
├── Models/
│   ├── Entities/         # Database entities
│   └── DTOs/            # Data Transfer Objects
├── Repositories/         # Repository pattern implementation
├── Services/            # Business logic services
├── Mappings/           # AutoMapper profiles
└── Program.cs          # Application startup configuration
```

## Database Schema

### Entities
- **User**: User management with roles, customer, and location associations
- **Role**: System roles (Admin, Customer, Staff, Driver)
- **Customer**: Company information and contact details
- **Location**: Customer locations with contact information

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd nyr-backend
   ```

2. **Update Connection String**
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NYR_Database;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Restore Packages**
   ```bash
   dotnet restore
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Navigate to `https://localhost:7000/swagger` (or the port shown in console)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration (Admin only)

### Users
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user (Admin only)
- `PUT /api/users/{id}` - Update user (Admin only)
- `DELETE /api/users/{id}` - Delete user (Admin only)

### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create customer (Admin only)
- `PUT /api/customers/{id}` - Update customer (Admin only)
- `DELETE /api/customers/{id}` - Delete customer (Admin only)
- `GET /api/customers/search?searchTerm={term}` - Search customers

### Locations
- `GET /api/locations` - Get all locations
- `GET /api/locations/{id}` - Get location by ID
- `POST /api/locations` - Create location (Admin/Customer only)
- `PUT /api/locations/{id}` - Update location (Admin/Customer only)
- `DELETE /api/locations/{id}` - Delete location (Admin only)
- `GET /api/locations/by-customer/{customerId}` - Get locations by customer

### Roles
- `GET /api/roles` - Get all roles (Admin only)
- `GET /api/roles/active` - Get active roles
- `GET /api/roles/{id}` - Get role by ID (Admin only)
- `POST /api/roles` - Create role (Admin only)
- `PUT /api/roles/{id}` - Update role (Admin only)
- `DELETE /api/roles/{id}` - Delete role (Admin only)

## Default Admin User

The application creates a default admin user:
- **Email**: admin@nyr.com
- **Password**: Admin123!

## Authentication

All endpoints (except login) require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Role Permissions

- **Admin**: Full access to all endpoints
- **Customer**: Access to customers and locations
- **Staff**: Access to locations
- **Driver**: Limited access (can be extended as needed)

## Database Migrations

The application uses Entity Framework Core with automatic database creation. The database will be created automatically on first run with seed data.

## Development

### Adding New Features
1. Create entity in `Models/Entities/`
2. Create DTOs in `Models/DTOs/`
3. Add repository interface and implementation
4. Add service interface and implementation
5. Create controller
6. Update AutoMapper profile
7. Register services in `Program.cs`

### Testing
Use Swagger UI or tools like Postman to test the API endpoints.

## Security Notes

- Change the JWT secret key in production
- Use HTTPS in production
- Implement proper password policies
- Add rate limiting for production use
- Consider implementing refresh tokens for better security
