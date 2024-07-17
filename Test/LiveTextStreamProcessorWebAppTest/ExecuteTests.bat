@echo off

REM Step 1: Run dotnet test with coverage collection
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="testresults/" /p:Exclude="[LiveTextStreamProcessorWebAppTest]*"

REM Step 2: Navigate to the coverage output directory
cd testresults

REM Step 3: Run ReportGenerator to generate HTML report
ReportGenerator.exe "-reports:coverage.cobertura.xml" "-targetdir:coverage-report" -reporttypes:Html

REM Step 4: Open the generated HTML report in the default browser (Windows specific)
start coverage-report\index.html
