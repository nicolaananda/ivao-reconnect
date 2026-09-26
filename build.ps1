$ErrorActionPreference = 'Stop'
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained true -o publish
Write-Host "Built: $PSScriptRoot\publish\IvaoAuto.exe"
