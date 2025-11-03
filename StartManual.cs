using System;
using System.IO;
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
                // Verificar pasta de configuração
                var configDir = AppContext.BaseDirectory;
                var configPath = Path.Combine(configDir, "config.json");
                
                Console.WriteLine($"Procurando config.json em: {configPath}");
                
                if (!File.Exists(configPath))
                {
                    Console.WriteLine();
                    Console.WriteLine("==================================================");
                    Console.WriteLine("ERRO: ARQUIVO CONFIG.JSON NAO ENCONTRADO!");
                    Console.WriteLine("==================================================");
                    Console.WriteLine();
                    Console.WriteLine($"O arquivo config.json deveria estar em:");
                    Console.WriteLine($"{configPath}");
                    Console.WriteLine();
                    Console.WriteLine("O arquivo sera criado automaticamente quando");
                    Console.WriteLine("voce executar o programa pela primeira vez.");
                    Console.WriteLine();
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    System.Threading.Thread.Sleep(1000);
                    return;
                }

                // Carregar configuração
                Console.WriteLine("Carregando configuração...");
                var config = ConfigurationService.LoadConfiguration();
                
                if (config == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERRO: Não foi possível carregar a configuração!");
                    Console.WriteLine($"Verifique o arquivo: {configPath}");
                    Console.WriteLine();
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    return;
                }
                
                if (string.IsNullOrEmpty(config.SftpHost) || config.SftpHost == "seu-servidor-sftp.com")
                {
                    Console.WriteLine();
                    Console.WriteLine("==================================================");
                    Console.WriteLine("ATENCAO: CONFIGURACAO NECESSARIA");
                    Console.WriteLine("==================================================");
                    Console.WriteLine();
                    Console.WriteLine("O arquivo config.json precisa ser configurado!");
                    Console.WriteLine($"Local: {configPath}");
                    Console.WriteLine();
                    Console.WriteLine("Abra o arquivo config.json e configure:");
                    Console.WriteLine("  - sftp_host: endereco do servidor SFTP");
                    Console.WriteLine("  - sftp_username: seu usuario");
                    Console.WriteLine("  - sftp_password: sua senha");
                    Console.WriteLine("  - sftp_port: porta (geralmente 22)");
                    Console.WriteLine();
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    System.Threading.Thread.Sleep(1000); // Garantir que deu tempo de ler
                    return;
                }
                
                Console.WriteLine($"Configuração carregada: Host={config.SftpHost}, Porta={config.SftpPort}");

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
                Console.WriteLine("==================================================");
                Console.WriteLine("ERRO!");
                Console.WriteLine("==================================================");
                Console.WriteLine($"Erro: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine($"Detalhes: {ex}");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para fechar...");
                try { Console.ReadKey(); } catch { }
            }
        }
    }
}

