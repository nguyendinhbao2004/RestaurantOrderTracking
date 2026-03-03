@echo off
echo Starting Restaurant Order Tracking Backend Server...
echo.

cd /d "%~dp0"

echo Restoring packages...
dotnet restore RestaurantOrderTracking.slnx

echo Building solution...
dotnet build RestaurantOrderTracking.slnx --configuration Release

echo.
echo Starting server on https://localhost:7260 and http://localhost:5015...
cd RestaurantOrderTracking
dotnet run --project RestaurantOrderTracking.WebApi.csproj --launch-profile https

pause
