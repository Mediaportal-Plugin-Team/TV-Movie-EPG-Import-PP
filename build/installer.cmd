@echo off
cls

Title Creating TVMovie EPG Installer

IF "%programfiles(x86)%XXX"=="XXX" GOTO 32BIT
    :: 64-bit
    SET PROGS=%programfiles(x86)%
    GOTO CONT
:32BIT
    SET PROGS=%ProgramFiles%
:CONT

IF NOT EXIST "%PROGS%\Team MediaPortal\MediaPortal\" SET PROGS=C:

:: Get version from DLL
FOR /F "tokens=*" %%i IN ('..\Tools\Tools\sigcheck.exe /accepteula /nobanner /n "..\TvMovie++\bin\Release\TvMovie++.dll"') DO (SET version=%%i)

:: Temp xmp2 file
COPY "..\MPEI\TV Movie EPG Import++.xmp2" "..\MPEI\TV Movie EPG Import++ Temp.xmp2"

:: Build MPE1
CD ..\MPEI
"%PROGS%\Team MediaPortal\MediaPortal\MPEMaker.exe" "TV Movie EPG Import++ Temp.xmp2" /B /V=%version% /UpdateXML
CD ..\build

:: Cleanup
del "..\MPEI\TV Movie EPG Import++ Temp.xmp2"
