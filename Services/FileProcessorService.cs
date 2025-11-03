using System;
using System.IO;
using System.Threading.Tasks;
using TransferFilesApp.Models;

namespace TransferFilesApp.Services
{
    public static class FileProcessorService
    {
        private const string EnviarDir = @"C:\Transfer\A enviar";
        private const string EnviadosDir = @"C:\Transfer\Enviados";

        public static async Task ProcessAllFiles(SftpService sftpService)
        {
            if (!Directory.Exists(EnviarDir))
            {
                LoggingService.LogWarning($"Pasta não encontrada: {EnviarDir}");
                return;
            }

            var files = Directory.GetFiles(EnviarDir);
            
            if (files.Length == 0)
            {
                LoggingService.LogInfo("Nenhum arquivo encontrado para processar");
                return;
            }

            LoggingService.LogInfo($"Processando {files.Length} arquivo(s)...");

            foreach (var file in files)
            {
                // Ignorar arquivos temporários e ocultos
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith(".") || fileName.StartsWith("~"))
                    continue;

                await ProcessFile(file, sftpService);
            }

            LoggingService.LogInfo("Processamento concluído");
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
                if (!Directory.Exists(EnviadosDir))
                {
                    Directory.CreateDirectory(EnviadosDir);
                }

                var destinoPath = Path.Combine(EnviadosDir, fileName);

                // Se arquivo já existir, adicionar timestamp
                if (File.Exists(destinoPath))
                {
                    var nomeSemExtensao = Path.GetFileNameWithoutExtension(fileName);
                    var extensao = Path.GetExtension(fileName);
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    destinoPath = Path.Combine(EnviadosDir, $"{nomeSemExtensao}_{timestamp}{extensao}");
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
                    if (!Directory.Exists(EnviadosDir))
                    {
                        Directory.CreateDirectory(EnviadosDir);
                    }

                    var destinoPath = Path.Combine(EnviadosDir, fileName);

                    if (File.Exists(destinoPath))
                    {
                        var nomeSemExtensao = Path.GetFileNameWithoutExtension(fileName);
                        var extensao = Path.GetExtension(fileName);
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        destinoPath = Path.Combine(EnviadosDir, $"{nomeSemExtensao}_{timestamp}{extensao}");
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
    }
}

