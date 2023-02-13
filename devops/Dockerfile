FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal as base

# change source
# RUN sed -i "s@archive.ubuntu.com@mirrors.aliyun.com@g" /etc/apt/sources.list

#
# RUN apt-get update -y && apt-get install -y libgdiplus && apt-get clean

#
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1.0/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1.0/g' /usr/lib/ssl/openssl.cnf

# 
COPY app /app

# 
WORKDIR /app

ENTRYPOINT ["dotnet", "Passingwind.WorkflowApp.Web.dll"] 