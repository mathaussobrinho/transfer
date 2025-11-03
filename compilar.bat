@echo off
echo ========================================
echo COMPILANDO TRANSFER FILES
echo ========================================
echo.

echo Restaurando dependencias...
call dotnet restore

if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Nao foi possivel restaurar as dependencias
    pause
    exit /b 1
)

echo.
echo Compilando projeto...
call dotnet build -c Release

if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Nao foi possivel compilar o projeto
    pause
    exit /b 1
)

echo.
echo ========================================
echo Compilacao concluida com sucesso!
echo.
echo Executavel: bin\Release\net6.0\TransferFiles.exe
echo ========================================
pause

