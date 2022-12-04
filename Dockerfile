FROM mcr.microsoft.com/dotnet/sdk:6.0
COPY . /app
WORKDIR /app
ENTRYPOINT ["dotnet", "WhatTheDown.dll"]