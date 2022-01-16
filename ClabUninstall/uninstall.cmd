@echo off
set filepath=%1
cd ..
start cmd.exe /k "timeout 2 > NUL && rmdir /s /q %filepath%"
