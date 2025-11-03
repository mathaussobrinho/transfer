using System;
using System.Threading.Tasks;
using TransferFilesApp.Services;
using TransferFilesApp.Models;

namespace TransferFilesApp
{
    class StartManual
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("ENVIO MANUAL DE ARQUIVOS");
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

                Console.WriteLine("Iniciando envio manual...");
                Console.WriteLine();

                var sftpService = new SftpService(config);
                await FileProcessorService.ProcessAllFiles(sftpService);
                sftpService.Dispose();
                
                Console.WriteLine();
                Console.WriteLine("==================================================");
                Console.WriteLine("Envio manual concluído!");
                Console.WriteLine("==================================================");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para fechar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"ERRO: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para fechar...");
                Console.ReadKey();
            }
        }
    }
}

