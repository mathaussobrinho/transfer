using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TransferFilesApp.Services;
using TransferFilesApp.Models;

namespace TransferFilesApp
{
    class Program
    {
        private static FileWatcherService? _fileWatcherService;
        private static CancellationTokenSource? _cancellationTokenSource;

        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("SISTEMA DE TRANSFERENCIA DE ARQUIVOS VIA SFTP");
            Console.WriteLine("==================================================");
            Console.WriteLine();

            try
            {
                // Criar pastas necessárias
                DirectoryService.CreateDirectories();
                Console.WriteLine("Pastas verificadas/criadas em C:\\Transfer");

                // Carregar configuração
                var config = ConfigurationService.LoadConfiguration();
                
                if (config == null || string.IsNullOrEmpty(config.SftpHost) || config.SftpHost == "seu-servidor-sftp.com")
                {
                    Console.WriteLine();
                    Console.WriteLine("ERRO: Configure as credenciais SFTP no arquivo config.json");
                    Console.WriteLine("Local: C:\\Transfer\\config.json");
                    Console.WriteLine();
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    return;
                }

                // Criar serviços
                var sftpService = new SftpService(config);
                
                // Processar arquivos existentes
                Console.WriteLine("Processando arquivos existentes na pasta 'A enviar'...");
                await ProcessExistingFiles(sftpService);

                // Iniciar monitoramento
                _cancellationTokenSource = new CancellationTokenSource();
                _fileWatcherService = new FileWatcherService(sftpService, _cancellationTokenSource.Token);
                _fileWatcherService.Start();

                Console.WriteLine();
                Console.WriteLine($"Monitorando pasta: C:\\Transfer\\A enviar");
                Console.WriteLine("Pressione Ctrl+C para parar...");
                Console.WriteLine();

                // Aguardar até receber sinal de cancelamento
                await WaitForCancel();
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro fatal: {ex.Message}");
                Console.WriteLine($"Erro fatal: {ex.Message}");
            }
            finally
            {
                _fileWatcherService?.Stop();
                _cancellationTokenSource?.Cancel();
                Console.WriteLine();
                Console.WriteLine("Sistema encerrado");
            }
        }

        private static async Task ProcessExistingFiles(SftpService sftpService)
        {
            var enviarDir = Path.Combine("C:\\Transfer", "A enviar");
            
            if (!Directory.Exists(enviarDir))
                return;

            var files = Directory.GetFiles(enviarDir);
            foreach (var file in files)
            {
                // Ignorar arquivos temporários e ocultos
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith(".") || fileName.StartsWith("~"))
                    continue;

                await ProcessFile(file, sftpService);
            }
        }

        private static async Task ProcessFile(string filePath, SftpService sftpService)
        {
            var fileName = Path.GetFileName(filePath);
            LoggingService.LogInfo($"Processando arquivo: {fileName}");

            try
            {
                // Enviar arquivo via SFTP
                var success = await sftpService.UploadFileAsync(filePath);

                // Mover arquivo para pasta Enviados
                var enviadosDir = Path.Combine("C:\\Transfer", "Enviados");
                var destinoPath = Path.Combine(enviadosDir, fileName);

                // Se arquivo já existir, adicionar timestamp
                if (File.Exists(destinoPath))
                {
                    var nomeSemExtensao = Path.GetFileNameWithoutExtension(fileName);
                    var extensao = Path.GetExtension(fileName);
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    destinoPath = Path.Combine(enviadosDir, $"{nomeSemExtensao}_{timestamp}{extensao}");
                }

                File.Move(filePath, destinoPath);

                if (success)
                {
                    LoggingService.LogInfo($"Arquivo movido para Enviados: {Path.GetFileName(destinoPath)}");
                }
                else
                {
                    LoggingService.LogWarning($"Arquivo movido para Enviados (com erro): {Path.GetFileName(destinoPath)}");
                    ErrorReportService.GenerateErrorReport(fileName, "Erro ao enviar arquivo via SFTP");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Erro ao processar arquivo {fileName}: {ex.Message}";
                LoggingService.LogError(errorMsg);

                // Mover arquivo para Enviados mesmo com erro
                try
                {
                    var enviadosDir = Path.Combine("C:\\Transfer", "Enviados");
                    var destinoPath = Path.Combine(enviadosDir, fileName);

                    if (File.Exists(destinoPath))
                    {
                        var nomeSemExtensao = Path.GetFileNameWithoutExtension(fileName);
                        var extensao = Path.GetExtension(fileName);
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        destinoPath = Path.Combine(enviadosDir, $"{nomeSemExtensao}_{timestamp}{extensao}");
                    }

                    File.Move(filePath, destinoPath);
                    ErrorReportService.GenerateErrorReport(fileName, ex.Message);
                }
                catch (Exception moveEx)
                {
                    LoggingService.LogError($"Erro ao mover arquivo após falha: {moveEx.Message}");
                }
            }
        }

        private static async Task WaitForCancel()
        {
            var tcs = new TaskCompletionSource<bool>();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                tcs.SetResult(true);
            };

            await tcs.Task;
        }
    }
}

