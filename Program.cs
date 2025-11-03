using System;
using System.Threading;
using System.Threading.Tasks;
using TransferFilesApp.Services;
using TransferFilesApp.Models;

namespace TransferFilesApp
{
    class Program
    {
        private static ScheduledService? _scheduledService;
        private static CancellationTokenSource? _cancellationTokenSource;

        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("SISTEMA DE TRANSFERENCIA DE ARQUIVOS VIA SFTP");
            Console.WriteLine("==================================================");
            Console.WriteLine();

            try
            {
                // Carregar configuração
                var config = ConfigurationService.LoadConfiguration();
                
                if (config == null || string.IsNullOrEmpty(config.SftpHost) || config.SftpHost == "seu-servidor-sftp.com")
                {
                    Console.WriteLine();
                    Console.WriteLine("ERRO: Configure as credenciais SFTP no arquivo config.json");
                    Console.WriteLine("Local: C:\\Transfer\\NAO MECHER\\config.json");
                    Console.WriteLine();
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    return;
                }

                // Verificar se é execução manual (com argumento)
                if (args.Length > 0 && args[0].ToLower() == "manual")
                {
                    // Modo manual - executar imediatamente
                    Console.WriteLine("=== MODO MANUAL - EXECUTANDO AGORA ===");
                    Console.WriteLine();
                    
                    var sftpService = new SftpService(config);
                    await FileProcessorService.ProcessAllFiles(sftpService);
                    sftpService.Dispose();
                    
                    Console.WriteLine();
                    Console.WriteLine("Processamento manual concluído!");
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    return;
                }

                // Modo automático - agendar para 17h
                Console.WriteLine("=== MODO AUTOMÁTICO - AGENDADO PARA 17:00 ===");
                Console.WriteLine();
                
                _scheduledService = new ScheduledService(config);
                _scheduledService.Start();

                Console.WriteLine("Serviço agendado iniciado.");
                Console.WriteLine("O programa ficará rodando em segundo plano.");
                Console.WriteLine("Envio automático acontecerá todos os dias às 17:00");
                Console.WriteLine();
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
                _scheduledService?.Stop();
                Console.WriteLine();
                Console.WriteLine("Sistema encerrado");
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
