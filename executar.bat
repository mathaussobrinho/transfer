@echo off
cd /d "%~dp0"

REM Verificar se existe executável compilado
if exist "bin\Release\net6.0\TransferFiles.exe" (
    echo Executando versao compilada...
    "bin\Release\net6.0\TransferFiles.exe"
) else (
    echo Executando via dotnet run...
    dotnet run
)

pause

