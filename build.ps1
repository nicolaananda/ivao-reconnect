$ErrorActionPreference = 'Stop'
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained false -o publish
Write-Host "Built: $PSScriptRoot\publish\IvaoAuto.exe"
