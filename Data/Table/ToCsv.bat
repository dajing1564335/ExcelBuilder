@echo off
setlocal enabledelayedexpansion

where powershell >nul 2>&1
if %errorlevel% neq 0 (
    echo PowerShell is not installed. Exiting...
    exit /b 1
)

for %%f in (*.xlsx) do (
    echo Converting %%f to CSV...

    :: Call PowerShell to do the conversion
    powershell -command ^
    "$excel = New-Object -ComObject Excel.Application;$workbook = $excel.Workbooks.Open('%%~dpnxf');$csvPath = '%%~dpnxf'.Replace('.xlsx', '.csv');$workbook.SaveAs($csvPath, 6);$workbook.Close($false);$excel.Quit();"
)

echo All conversions are done.
pause
