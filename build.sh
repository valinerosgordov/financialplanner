#!/bin/bash
echo "Сборка Financial Planner в exe файл..."
echo

dotnet publish FinancialPlanner.Console.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

echo
echo "========================================"
echo "Сборка завершена!"
echo
echo "Exe файл находится в:"
echo "bin/Release/net8.0/win-x64/publish/FinancialPlanner.Console.exe"
echo "========================================"
