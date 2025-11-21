@ECHO OFF

REM Install tools if not present
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

REM Clean and build solution
dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore

REM Create TestResults folder if not exists
if not exist "TestResults" mkdir TestResults

REM Run tests with coverage
dotnet test Ambev.DeveloperEvaluation.sln --no-restore --verbosity normal ^
/p:CollectCoverage=true ^
/p:CoverletOutputFormat=cobertura ^
/p:CoverletOutput=%cd%\TestResults\coverage.cobertura.xml ^
/p:Exclude="[*]*.Migrations.*" ^
/p:Exclude="[*]*.Program" ^
/p:Exclude="[*]*.Startup" ^
/p:Exclude="[Ambev.DeveloperEvaluation.ORM]*"

REM Generate coverage report
reportgenerator ^
-reports:"%cd%\TestResults\coverage.cobertura.xml" ^
-targetdir:"%cd%\TestResults\CoverageReport" ^
-reporttypes:Html ^
-assemblyfilters:"+Ambev.DeveloperEvaluation.Application;+Ambev.DeveloperEvaluation.Domain;+Ambev.DeveloperEvaluation.Common;-Ambev.DeveloperEvaluation.ORM;-*Migrations*"

REM Removing temporary files
rmdir /s /q bin 2>nul
rmdir /s /q obj 2>nul

echo.
echo Coverage report generated at TestResults/CoverageReport/index.html
pause