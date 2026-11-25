# CustomerDesk

A small ASP.NET Core Web API for managing customer records. It uses Entity Framework Core with SQLite.

## Requirements

- .NET 10 SDK

## Run the API

From the repository root:

```powershell
dotnet run --project CustomerDesk.Api
```

With the included HTTP launch profile, the API runs at `http://localhost:5176`. In Development, open `http://localhost:5176/swagger` to use Swagger UI.

The API applies pending EF Core migrations at startup. By default, SQLite stores data in `CustomerDesk.Api/CustomerDesk.db`. This local database file is ignored by Git; migrations are committed as source code.

## Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/Customers` | List customers; optionally filter with `name` and/or `email` query parameters |
| `GET` | `/api/Customers/availability` | Check whether an email and/or phone number is available |
| `GET` | `/api/Customers/{id}` | Get an active customer by ID |
| `POST` | `/api/Customers` | Register a customer |
| `PUT` | `/api/Customers/{id}` | Replace a customer's name, email, and phone |
| `PATCH` | `/api/Customers/{id}` | Update only the supplied customer fields |
| `DELETE` | `/api/Customers/{id}` | Soft-delete a customer |

Email and phone numbers must be unique among active customers. Deleted customers are hidden from normal queries, and their email and phone can be reused.

## Configuration

The default SQLite connection string is in `CustomerDesk.Api/appsettings.json`. Override it with the `ConnectionStrings__DefaultConnection` environment variable when needed. For example, in PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Data Source=CustomerDesk.db"
dotnet run --project CustomerDesk.Api
```

ASP.NET Core does not load `.env` files by default, so this project does not need one.

