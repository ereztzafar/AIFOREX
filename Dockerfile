# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# העתק את כל קבצי הפרויקט
COPY . ./

# שחזור תלויות וקומפילציה
RUN dotnet restore
RUN dotnet publish -c Release -o out

# שלב הריצה
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/out .

# הפעלת הרובוט
ENTRYPOINT ["dotnet", "AIFOREX.dll"]
