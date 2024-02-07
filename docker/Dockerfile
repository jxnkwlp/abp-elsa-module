FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 8080
ENV TZ=Etc/UTC

# 
WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

# 
COPY app /app

ENTRYPOINT ["dotnet", "Passingwind.WorkflowApp.Web.dll"]
