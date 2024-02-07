FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 8080

# 
WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN apk add --no-cache icu-libs icu-data-full krb5-libs libgcc libintl libssl1.1 libstdc++ zlib tzdata

# 
COPY app /app

ENTRYPOINT ["dotnet", "Passingwind.WorkflowApp.Web.dll"]
