$ErrorActionPreference = "Stop"

Write-Host "    ___      _       _     _____  _____  "
Write-Host "   /   \_ __(_)_ __ | | __ \_   \/__   \ "
Write-Host "  / /\ / '__| | '_ \| |/ /  / /\/  / /\/ "
Write-Host " / /_//| |  | | | | |   </\/ /_   / /    "
Write-Host "/___,' |_|  |_|_| |_|_|\_\____/   \/     "
                                        
Write-Host
Write-Host

Write-Host "Inicializing SQL Server container..."

docker run --name "drinkit_db" -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04

Write-Host "Container inicialized."
Write-Host
Write-Host "Waiting 10 seconds to make sure it is ok."
Start-Sleep -Seconds 10
Write-Host
Write-Host "Verifying EF migrations..."

Push-Location "./src/DrinkIT.Infrastructure"

dotnet ef database update

Pop-Location

Write-Host "Inicializing API..."

Push-Location "./src/drinkit"

dotnet run .\DrinkIT.csproj