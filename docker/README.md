# ABP with Elsa Application

## Prerequisites

- SqlServer 2012+
- Redis (optional)

## Configurations

See [appsettings.json](../app/src/Passingwind.WorkflowApp.Web/appsettings.json) file

All config can be convert from environment variables
> https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#naming-of-environment-variables


## Docker compose example

[docker-compose.yml](./docker-compose.yml)

``` shell
version: '3.8'

services:
  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    restart: unless-stopped
    ports:
      - 1433:1433
    environment:
      ACCEPT_EULA: Y
      MSSQL_SA_PASSWORD: <YourStrong!Passw0rd>
      MSSQL_PID: Express
      
  app: 
    image: passingwind/abp-elsa-app
    restart: unless-stopped
    ports:
      - 10000:80
    environment:
      TZ: Asia/Shanghai
      ConnectionStrings__Default: "Server=db;Database=workflowapp;User Id=sa;Password=<YourStrong!Passw0rd>;TrustServerCertificate=true;"
      Elsa__Server__BaseUrl: "http://localhost:10000"
    # volumes:
    #   - ./appsettings.json:/app/appsettings.json 
    depends_on:
      - db
```
