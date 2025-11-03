using System;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using TransferFilesApp.Models;

namespace TransferFilesApp.Services
{
    public class SftpService
    {
        private readonly SftpConfig _config;
        private SftpClient? _sftpClient;

        public SftpService(SftpConfig config)
        {
            _config = config;
        }

        public async Task<bool> UploadFileAsync(string localFilePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Conectar se não estiver conectado
                    if (_sftpClient == null || !_sftpClient.IsConnected)
                    {
                        if (!Connect())
                        {
                            return false;
                        }
                    }

                    var fileName = Path.GetFileName(localFilePath);
                    var remotePath = _config.SftpRemotePath.TrimEnd('/') + "/" + fileName;

                    // Criar diretório remoto se não existir
                    try
                    {
                        if (!_sftpClient.Exists(_config.SftpRemotePath))
                        {
                            _sftpClient.CreateDirectory(_config.SftpRemotePath);
                        }
                    }
                    catch
                    {
                        // Diretório já existe ou erro ao criar
                    }

                    // Enviar arquivo
                    using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                    {
                        _sftpClient.UploadFile(fileStream, remotePath);
                    }

                    LoggingService.LogInfo($"Arquivo enviado com sucesso: {fileName} -> {remotePath}");
                    return true;
                }
                catch (Exception ex)
                {
                    LoggingService.LogError($"Erro ao enviar arquivo {Path.GetFileName(localFilePath)}: {ex.Message}");
                    return false;
                }
            });
        }

        private bool Connect()
        {
            try
            {
                var connectionInfo = new ConnectionInfo(
                    _config.SftpHost,
                    _config.SftpPort,
                    _config.SftpUsername,
                    new PasswordAuthenticationMethod(_config.SftpUsername, _config.SftpPassword)
                );

                _sftpClient = new SftpClient(connectionInfo);
                _sftpClient.Connect();
                
                LoggingService.LogInfo($"Conectado ao SFTP: {_config.SftpHost}:{_config.SftpPort}");
                return true;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro ao conectar ao SFTP: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_sftpClient != null && _sftpClient.IsConnected)
                {
                    _sftpClient.Disconnect();
                    LoggingService.LogInfo("Desconectado do SFTP");
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro ao desconectar do SFTP: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Disconnect();
            _sftpClient?.Dispose();
        }
    }
}

