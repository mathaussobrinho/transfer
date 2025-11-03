@echo off
echo ========================================
echo COMPILANDO TODOS OS EXECUTAVEIS
echo ========================================
echo.

echo [1/2] Compilando TransferFiles (Servico Automatico)...
call dotnet build TransferFiles.csproj -c Release

if %ERRORLEVEL% NEQ 0 (
    echo ERRO ao compilar TransferFiles!
    pause
    exit /b 1
)

echo.
echo [2/2] Compilando Start.exe (Botao Manual)...
call dotnet build StartManual.csproj -c Release

if %ERRORLEVEL% NEQ 0 (
    echo ERRO ao compilar Start!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Compilacao concluida com sucesso!
echo.
echo Executaveis gerados:
echo - bin\Release\net6.0\TransferFiles.exe (Servico)
echo - bin\Release\net6.0\Start.exe (Manual)
echo.
echo Para instalar o servico automatico, copie TransferFiles.exe
echo para C:\Transfer\NAO MECHER\ e configure para iniciar com Windows.
echo.
echo Copie Start.exe para a area de trabalho como atalho.
echo ========================================
pause

