# Database.Workshop

A sample ASP.NET Core Web API project demonstrating the use of Repository Pattern with multiple implementations (ADO.NET, Dapper, EF Core) to perform CRUD operations on a PostgreSQL database.  
The project includes a Factory Method for repository creation and migration support for EF Core.

---

## Features

- ASP.NET Core Web API targeting .NET 8.0  
- PostgreSQL as the database  
- Repository Pattern abstraction with three implementations:  
  - ADO.NET  
  - Dapper  
  - Entity Framework Core  
- Factory method to choose repository implementation at runtime  
- EF Core code-first migrations for schema management  
- User CRUD operations through RESTful API  
- Configurable via `appsettings.json`  
- Swagger/OpenAPI integration for API testing  

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [PostgreSQL](https://www.postgresql.org/download/) database server  
- Optional: [Postman](https://www.postman.com/downloads/) or any API client to test endpoints

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/Monem-Benjeddou/Database.Workshop.git
   cd Database.Workshop
