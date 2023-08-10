@ECHO OFF

:: This file's encoding is UTF-8, please note Visual Studio generate text file's encoding is UTF-8-BOM.
chcp 65001

net session >nul 2>&1
IF NOT %ERRORLEVEL% EQU 0 (
   ECHO ERROR: Please run Bat as Administrator.
   PAUSE
   EXIT /B 1
)

@SETLOCAL enableextensions
@CD /d "%~dp0"

ECHO Installing IndoCash.MonitorService...
ECHO ---------------------------------------------------
sc create IndoCash.MonitorService binPath= "%~dp0IndoCash.MonitorService\IndoCash.MonitorService.Host.exe" start= delayed-auto
ECHO ---------------------------------------------------
ECHO Done.

net start IndoCash.MonitorService
PAUSE