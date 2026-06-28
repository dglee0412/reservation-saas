# ==== 1단계: build (작업장) ====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# csproj만 먼저 복사해서 의존성 복원 (레이어 캐싱 활용)
COPY Reservation_API.csproj .
RUN dotnet restore

# 나머지 전체 복사 후 publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ==== 2단계: runtime (손님 집) ====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# 1단계(build)에서 만든 결과물만 가져옴
COPY --from=build /app/publish .

# 컨테이너는 평문 HTTP 8080만 듣는다 (TLS Azure 인그레스가 처리)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Reservation_API.dll"]