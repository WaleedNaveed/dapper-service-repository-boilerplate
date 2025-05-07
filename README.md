# Dapper Service-Repository Pattern Boilerplate

A modern, lightweight, and extensible **.NET 8** boilerplate for building RESTful APIs using the **Service Repository Pattern**, **Dapper**, and best practices. This boilerplate provides a clean foundation for scalable backend services, saving you time on setup and configuration.

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Dapper](https://img.shields.io/badge/Dapper-ORM-green)
![DbUp](https://img.shields.io/badge/DbUp-Migrations-orange)

---

## 🚀 Features

This boilerplate comes packed with production-ready features to kickstart your API development:

- **RESTful API Structure**: Clean controller-service-repository pattern for maintainable code.
- **Dapper ORM**: Lightweight and high-performance data access with SQL Server.
- **DbUp Migrations**: Automated database migrations run on startup, no manual execution needed.
- **JWT Authentication**: Secure token-based authentication with role-based authorization and flexible key length support.
- **Rate Limiting**: Configurable rate limits to prevent abuse and ensure fair usage.
- **Fluent Validation**: Declarative validation for request DTOs.
- **AutoMapper**: Simplified object mapping between DTOs and models.
- **Quartz Jobs**: Background job scheduling for tasks like email sending or data processing.
- **Email Service**: Configurable SMTP-based email sending with template support.
- **Request/Response Logging**: Middleware to log HTTP requests and responses for debugging.
- **Exception Handling**: Global exception middleware for consistent error responses.
- **API Versioning**: Support for multiple API versions with fallback to default.
- **Swagger/OpenAPI**: Interactive API documentation for easy testing.
- **Dependency Injection**: Organized DI setup with extension methods for clarity.
- **Unit & Integration Testing**: xUnit-based tests with Moq for mocking dependencies.

---

## 🛠 Tech Stack

- **.NET 8**: Latest .NET framework for performance and modern features.
- **Dapper**: Micro-ORM for fast and flexible database queries.
- **DbUp**: Lightweight database migration framework for SQL Server.
- **SQL Server**: Default database (configurable for others).
- **Redis**: For caching purpose.
- **AutoMapper**: Object-to-object mapping.
- **FluentValidation**: Validation for request models.
- **Serilog**: Structured logging (configurable).
- **Quartz.NET**: Background job scheduling.
- **Swashbuckle**: Swagger for API documentation.
- **xUnit & Moq**: Unit and integration testing framework.

---

## 📋 Prerequisites

Before running the boilerplate, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or any compatible database)
- [Redis](https://redis.io/download) 
- An SMTP server (for email functionality, e.g., Gmail, SendGrid)
- [Git](https://git-scm.com/downloads) for cloning the repo

---

## 🏁 Getting Started

Follow these steps to get the boilerplate up and running:

### 1. Clone the Repository
```bash
git clone https://github.com/WaleedNaveed/dappersrp-boilerplate.git
```

### 2. Configure Settings
Update the ```appsettings.json``` file in ```src/DapperSRP.WebApi``` with your configuration:
- **Database**: Set up SQL Server. Migrations are automatically applied on startup using DbUp.
- **JWT**: Use any secret key length (auto-normalized to 256 bits for security).
- **SMTP**: Configure your email provider credentials.
- **Redis**: For black-listing JWT.

### 3. Build and Run
```bash
dotnet build
dotnet run --project src/DapperSRP.WebApi
```
The API will be available at ```https://localhost:7285``` (or your configured port). Swagger UI will open at ```https://localhost:7285/swagger/index.html``` for interactive testing. If ```Super Admin``` credentials are not removed from ```appsettings```, you can try to login with them.


## 🗂 Project Structure
The boilerplate follows a modular, Service-Repository-Pattern based structure:
```bash
DapperSRP/
├── DapperSRP.sln                # Solution file
├── src/                         # Source projects
│   ├── DapperSRP.Common/        # Configurations, enums, helpers
│   ├── DapperSRP.Dto/           # Data Transfer Objects (DTOs)
│   ├── DapperSRP.Logging/       # Logging logic and providers
│   ├── DapperSRP.Persistence/   # DbUp migrations, database models
│   ├── DapperSRP.Quartz/        # Quartz job logic and scheduling
│   ├── DapperSRP.Repository/    # Generic repository, custom repositories, unit of work
│   ├── DapperSRP.Service/       # Business logic and services
│   ├── DapperSRP.WebApi/        # Controllers, middleware, DI extensions
├── tests/                       # Test projects
│   ├── UnitTests/               # Unit test projects
│   ├── IntegrationTests/        # Integration test projects
```


## 🔐 Security
- **JWT Authentication**: Tokens are signed with a normalized key (any length accepted, hashed/truncated to 256 bits for HmacSha256).
- **Role-Based Authorization**: Supports SuperAdmin, Admin, and custom roles.
- **Rate Limiting**: Prevents API abuse with configurable limits.

## 🛠 Customization

### Adding a New Entity
Add an entity like `Order`:
- Define model in `src/DapperSRP.Persistence/Models`.
- Define migration in `src/DapperSRP.Persistence/Migrations`.
- Define DTOs in `src/DapperSRP.Dto`.
- Add service interface in `src/DapperSRP.Service/Interface`. Add an implementation of this interface in `src/DapperSRP.Service/Service`.
- Add controller in `src/DapperSRP.WebApi/Controllers`.
- Register in `src/DapperSRP.WebApi/Extension/ServiceCollectionExtensions.cs`.

### Configuring Quartz Jobs
Schedule tasks:
- Define job in `src/DapperSRP.Quartz/Jobs`.
- Add the cron expression in `src/DapperSRP.Quartz/JobScheduleConfig.cs`

### Extending Middleware
Add middleware:
- Create in `src/DapperSRP.WebApi/Middleware`.
- Register in `src/DapperSRP.WebApi/Extension/MiddlewareConfiguration.cs`.

### API Versioning
Support API versions:
- Add `[ApiVersion("{VERSION_NUMBER}")]` in `src/DapperSRP.WebApi/Controllers`.

### Creating Custom Repository
Extend repository:
- Create an interface in `src/DapperSRP.Repository/Interface` and implement this interface in `src/DapperSRP.Repository/Repository`.
- Register in `src/DapperSRP.WebApi/Extension/ServiceExtensions.cs`.

### Changing Log Provider
Switch logging:
- Update in `src/DapperSRP.Logging`.
- Register in `src/DapperSRP.WebApi/Extension/ServiceCollectionExtensions.cs`.

## 🧪 Running Tests
Unit and integration tests ensure code reliability:

```bash
dotnet test
```
Tests use **xUnit** and **Moq** to cover controllers, services, repositories, and integration scenarios.


## 📝 Configuration
Key settings in `appsettings.json`:

| Section             | Description                              |
|---------------------|------------------------------------------|
| `ConnectionStrings` | Database and Redis connection strings    |
| `Jwt`               | Secret key and token expiry settings     |
| `Smtp`              | Email server credentials                 |
| `RateLimiting`      | API rate limit configuration             |


## 🤝 Contributing
Contributions are welcome! To contribute:

- Fork the repository.
- Create a feature branch (`git checkout -b feature/your-feature`).
- Commit your changes (`git commit -m "Add your feature"`).
- Push to the branch (`git push origin feature/your-feature`).
- Open a Pull Request.


## 🙌 Acknowledgements
- [Dapper](https://github.com/DapperLib/Dapper) for lightweight ORM.
- [DbUp](https://dbup.github.io/) for seamless migrations.
- [AutoMapper](https://automapper.org/) for object mapping.
- [FluentValidation](https://fluentvalidation.net/) for clean validations.
- [Quartz.NET](https://www.quartz-scheduler.net/) for job scheduling.


## 📬 Contact
Have questions or suggestions? Open an issue on GitHub.

Happy coding!
