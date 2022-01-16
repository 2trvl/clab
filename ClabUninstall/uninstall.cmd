@echo off
set filepath=%1
cd ..
tasklist | find /i "Clab.exe" && taskkill /im Clab.exe /F || echo "Clab.exe" not running
echo Uninstalling Clab...
timeout 2 > NUL
rmdir /s /q %filepath%
