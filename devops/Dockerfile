FROM  mcr.microsoft.com/dotnet/aspnet:7.0-alpine

# 
WORKDIR /app

#
# RUN apt-get update -y && apt-get install -y libgdiplus && apt-get clean

#
#RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1.0/g' /etc/ssl/openssl.cnf
#RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1.0/g' /usr/lib/ssl/openssl.cnf

# 
COPY app /app

ENTRYPOINT ["dotnet", "Passingwind.WorkflowApp.Web.dll"]
