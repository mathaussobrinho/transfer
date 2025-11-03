using System;
using System.IO;

namespace TransferFilesApp.Services
{
    public static class ErrorReportService
    {
        public static void GenerateErrorReport(string fileName, string error)
        {
            try
            {
                var reportDirectory = @"C:\Transfer";
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var reportPath = Path.Combine(reportDirectory, $"erro_{timestamp}.txt");

                var reportContent = new System.Text.StringBuilder();
                reportContent.AppendLine("=".PadRight(50, '='));
                reportContent.AppendLine("RELATORIO DE ERRO DE TRANSFERENCIA");
                reportContent.AppendLine("=".PadRight(50, '='));
                reportContent.AppendLine();
                reportContent.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                reportContent.AppendLine($"Arquivo: {fileName}");
                reportContent.AppendLine($"Erro: {error}");
                reportContent.AppendLine($"Destino: C:\\Transfer\\Enviados\\{fileName}");
                reportContent.AppendLine();
                reportContent.AppendLine("=".PadRight(50, '='));

                File.WriteAllText(reportPath, reportContent.ToString(), System.Text.Encoding.UTF8);
                LoggingService.LogInfo($"Relatório de erro gerado: {reportPath}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro ao gerar relatório: {ex.Message}");
            }
        }
    }
}

