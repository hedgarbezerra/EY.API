

# Web API Design on Azure
*This document provides an overview of the web API design hosted on the Azure cloud.*


## Overview
This web API is designed to efficiently handle requests for IP address information by leveraging Azure services, caching, and database lookups.

## Components
### Client
- Description: The client sends a request to the .NET API.

### Azure Cloud
- Environment: All the present components are Azure Cloud, utilizing an Azure subscription for resource management.

### Azure App Service Plan
- Service Plan: The .NET API and other resources running on a single plan.

### Azure Web App
- Provides unique runtime for API, and other resources with docker.

### Azure App Configuration
- Centralized configuration management for the application. 
It stores and manages settings, feature flags, and secrets, ensuring that the application's configuration is consistent across different environments.

### Redis
- Used as a distributed, in-memory cache to store frequently accessed data.
This helps to reduce the load on the SQL Server and improve the response time of the API by providing quick data retrieval.

### SQL Server
- The primary database for storing persistent data.
It handles relational data operations and supports complex queries, ensuring data integrity and reliability for the web API.

### Seq
- Provides real-time search and analysis server for structured application logs and traces through Protobuf protocol.

### .NET API
- Function: Handles incoming requests and processes them as follows:

	- Redis Cache: Checks if the IP address is present in the cache.
	- If Found: Returns the cached information.
	- If Not Found: Proceeds to check the SQL database.
	- SQL Server: Queries the database for the IP address.
	- If Found: Returns the database information.
	- If Not Found: Fetches the information from an external service (IP2C).

### External Service (IP2C)
- Function: Delievers IP address information.

# Workflow

## Client Request: The client sends a request to the .NET API.

### Redis Cache: The API checks Redis for the IP address.

- If found, returns the information from Redis.

- If not found, proceeds to the next step.

### SQL Server: The API queries the SQL database for the IP address.

- If found, returns the information from the SQL database.

- If not found, proceeds to the next step.

### External Service (IP2C): The API fetches the IP address information from the external service.

### Cache and Database Storage: The retrieved IP information is stored in Redis and the SQL database for future use.

Response to Client: The API returns the IP address information to the client.
![Infrastructure Diagram](https://github.com/user-attachments/assets/e3edb0bf-33a4-4e5e-8f76-6f1af5b3f4a7)



**There are few requirements for running the API locally, some settings are optional such as the "Azure" Section, all the others are required.**

~*If opt to use Azure App Configuration, remove all entries except by the "Azure".*~

~*Otherwise, you may remove method invocation in EY.API > Program.cs"*~
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Tasks": {
    "IpAddressUpdater": {
      "RepeatEveryMinutes":  60
    }
  },
  "API": {
    "Version": 1
  },
  "Azure": {
    "AppConfigurations": {
      "ConnectionString": "",
      "CacheSentinel": "",
      "CacheExpiracySeconds": ""
    }
  },
  "OpenTelemetry": {
    "Endpoint": "",
    "Key": "",
    "Source": ""
  },
  "RateLimit": {
    "PermitLimit": "",
    "TimeWindowSeconds": "",
    "QueueLimit": ""
  },
  "Redis": {
    "Instance": "",
    "ConnectionString": "",
    "CacheExpiracyInSeconds": ""
  },
  "RetryPolicy": {
    "MaxRetries": "",
    "DelayInSeconds": "",
    "MaxDelaySeconds": "",
    "TimeOutSeconds": ""
  },
  "ConnectionStrings": {
    "SqlServerInstance": ""
  },
  "AllowedHosts": "*"
}


```

# Running dependencies locally with docker compose

```
version: "3.4"
services:      
  redis:
    image: redis:latest
    command: ["redis-server", "--requirepass", "your_redis_password"]
    volumes:
      - vol-redis:/data
    ports:
      - 6379:6379
      
  redisinsight:
    image: redis/redisinsight:latest
    container_name: redisinsight
    ports:
      - 5540:5540
    depends_on:
      - redis
    volumes:
      - vol-redis-insight:/data
    environment:
      - REDISINSIGHT_AUTH_USERNAME=your_username
      - REDISINSIGHT_AUTH_PASSWORD=your_password

  seq:
    image: datalust/seq:latest
    volumes: 
      - vol-seq:/data
    ports:
      - 5341:5341
      - 80:80 
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_ADMINPASSWORD=your_seq_admin_password

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrong@Password
      - MSSQL_PID=Express
    ports:
      - 1433:1433
    volumes:
      - vol-sqlserver:/var/opt/mssql

volumes:
  vol-redis:
  vol-redis-insight:
  vol-seq:
  vol-sqlserver:


```
