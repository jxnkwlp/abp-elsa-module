FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine

EXPOSE 80

# 
WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN apk add --no-cache icu-libs icu-data-full krb5-libs libgcc libintl libssl1.1 libstdc++ zlib tzdata

# 
COPY app /app

ENTRYPOINT ["dotnet", "Passingwind.WorkflowApp.Web.dll"]