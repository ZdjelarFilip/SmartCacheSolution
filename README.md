# SmartCache API

## Overview
SmartCache API is a high-performance backend service designed for breach email tracking. It is built on Orleans for distributed and resilient caching for fast and reliable checks for compromised email addresses. The API includes authentication mechanisms and integrates with Azure Blob Storage for persistent storage.

## Features
### **Authentication**
- Secure user authentication using JWT tokens.
- Login endpoint for credential validation.

### **Email Breach Management**
- Check if an email is breached.
- Mark an email as breached.
- Uses Orleans grains for fast distributed caching.

### **Storage & Caching**
- Azure Blob Storage integration for persistent email breach records.
- In-memory caching via Orleans grains for faster lookups.
- Automatic state persistence with Orleans reminders.

### **Error Handling & Logging**
- Structured logging with ILogger
- Handles Orleans exceptions and general errors with appropriate status codes.
- Uses EmailValidator helper for email input validation.

## Architecture & Components
### **Controllers**
- `AuthController`: Handles authentication and JWT token generation.
- `BreachedEmailsController`: Exposes endpoints for checking and marking email breaches.

### **Services**
- `AuthService`: Manages user authentication and token generation.
- `EmailBreachService`: Interfaces with Orleans grains to handle email breach logic.

### **Grains**
- `BreachedEmailGrain`: Stores email breach status in Orleans and periodically persists state.

### **Repositories**
- `AzureBreachedEmailStorage`: Manages breach data storage in Azure Blob Storage.

### **Utilities**
- `EmailValidator`: Validates email format using regex.

## Setup & Deployment
### **Prerequisites**
- .NET 9 SDK
- Azure Storage Account
- Orleans Cluster Setup

### **Installation**
```sh
git clone https://github.com/ZdjelarFilip/SmartCacheSolution
cd SmartCache
dotnet restore
```

### **Running Locally** - Before running you should update appsettings.json / appsettings.Development.json in the SmartCacheAPI/TraditionalAPI project - credentials are provided in Bitwarden Vault in email.
```sh
dotnet build
dotnet run
```
The API will be accessible at `http://localhost:5147` (Swagger UI included).

## API Endpoints
### **Authentication**
| Method | Endpoint | Description |
|--------|------------|-------------|
| `POST` | `/api/auth/login` | Authenticates user and returns JWT token |

### **Breached Emails**
| Method | Endpoint | Description |
|--------|------------|-------------|
| `GET` | `/api/breaches/{email}` | Checks if an email is breached |
| `POST` | `/api/breaches` | Marks an email as breached |

## Error Handling
- **400 Bad Request**: Invalid email format.
- **401 Unauthorized**: Invalid authentication credentials.
- **404 Not Found**: Email not found in breach list.
- **503 Service Unavailable**: Orleans errors.
- **500 Internal Server Error**: Unexpected failures.

## Logging
- Logs are structured with ILogger.
- Orleans grain state changes are logged.
- Errors and exceptions are properly captured.

## Testing
- Uses xUnit and Moq for unit testing.
- Unit test coverage includes EmailBreachService, EmailBreachController, EmailHelper
- Integration test covers GET&POST EmailBreach functionalities

![Tests](https://i.imgur.com/1dX9Ajs.png)


## Performance Against Traditional API

In this project, we also implemented a Traditional API that uses EF Migrations and SQL Server (hosted in Azure). 
When the project is built and running the APIs are accessible at `http://localhost:5189` (Swagger UI included).

To evaluate its performance, we conducted load tests using Apache JMeter, ensuring all data was deleted before each test run.

The body of the script for running the load test in Apache JMeter:

```
{
  "email": "user${__Random(1,1000000)}@example.com"
}
```

We then compared its performance against SmartCache API and reached the following conclusion:

In terms of average HTTP response time and min/max response times, SmartCache API was almost twice as fast as the Traditional API.

![Average_R_T](https://i.imgur.com/g7rX5rq.png)


![MinMax_R_T](https://i.imgur.com/UaIDptL.png)
