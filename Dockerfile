# Крок 1: Побудова програми
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо все у контейнер
COPY . ./

# Публікуємо проект
RUN dotnet publish ConsoleApp8.csproj -c Release -o out

# Крок 2: Запуск
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Копіюємо з попереднього кроку
COPY --from=build /app/out .

# Запускаємо
CMD ["dotnet", "ConsoleApp8.dll"]
