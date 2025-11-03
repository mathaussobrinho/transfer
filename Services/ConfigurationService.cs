using System;
using System.IO;
using Newtonsoft.Json;
using TransferFilesApp.Models;

namespace TransferFilesApp.Services
{
    public static class ConfigurationService
    {
        // Config está na mesma pasta do programa (NAO MECHER)
        private static string ConfigPath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json"
        );

        public static SftpConfig? LoadConfiguration()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    CreateDefaultConfig();
                    return null;
                }

                var json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<SftpConfig>(json);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro ao carregar configuração: {ex.Message}");
                return null;
            }
        }

        private static void CreateDefaultConfig()
        {
            var defaultConfig = new SftpConfig
            {
                SftpHost = "seu-servidor-sftp.com",
                SftpPort = 22,
                SftpUsername = "seu_usuario",
                SftpPassword = "sua_senha",
                SftpRemotePath = "/"
            };

            var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);

            LoggingService.LogWarning($"Arquivo de configuração criado em: {ConfigPath}");
            LoggingService.LogWarning("Por favor, edite o arquivo config.json com suas credenciais SFTP");
        }
    }
}
