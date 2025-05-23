@echo off
chcp 65001 > nul
cd /d "%~dp0"
cd bin\Debug\net8.0
if exist AIFOREX.exe (
    AIFOREX.exe
) else (
    echo ❌ הקובץ AIFOREX.exe לא נמצא. ודא שהפרויקט נבנה.
)
pause
