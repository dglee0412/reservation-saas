# ==== 1단계: build(작업장 ====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 솔루션 파일 + 각 프로젝트의 csproj만 먼저 복사(레이어 캐싱)
COPY Reservation_Saas.slnx .
COPY Reservation.Domain/Reservation.Domain.csproj Reservation.Domain/
COPY Reservation.Application/Reservation.Application.csproj Reservation.Application/
COPY Reservation.Infrastructure/Reservation.Infrastructure.csproj Reservation.Infrastructure/
COPY Reservation_API/Reservation_API.csproj Reservation_API/
RUN dotnet restore

# 나머지 전체 소스 복사 후 API 프로젝트를 publish
COPY . .
RUN dotnet publish Reservation_API/Reservation_API.csproj -c Release -o /app/publish

# ==== 2단계: runtime (손님 집) ====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Reservation_API.dll"]