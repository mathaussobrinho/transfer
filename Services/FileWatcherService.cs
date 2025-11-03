using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TransferFilesApp.Services
{
    public class FileWatcherService
    {
        private readonly SftpService _sftpService;
        private readonly CancellationToken _cancellationToken;
        private FileSystemWatcher? _fileWatcher;
        private readonly object _lockObject = new object();

        public FileWatcherService(SftpService sftpService, CancellationToken cancellationToken)
        {
            _sftpService = sftpService;
            _cancellationToken = cancellationToken;
        }

        public void Start()
        {
            var enviarDir = Path.Combine("C:\\Transfer", "A enviar");

            if (!Directory.Exists(enviarDir))
            {
                DirectoryService.CreateDirectories();
            }

            _fileWatcher = new FileSystemWatcher(enviarDir)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.Renamed += OnFileRenamed;
        }

        public void Stop()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Created -= OnFileCreated;
                _fileWatcher.Renamed -= OnFileRenamed;
                _fileWatcher.Dispose();
            }

            _sftpService.Dispose();
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            ProcessFileAsync(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            ProcessFileAsync(e.FullPath);
        }

        private async void ProcessFileAsync(string filePath)
        {
            // Evitar processamento simultâneo do mesmo arquivo
            lock (_lockObject)
            {
                if (!File.Exists(filePath))
                    return;

                var fileName = Path.GetFileName(filePath);
                if (fileName.StartsWith(".") || fileName.StartsWith("~"))
                    return;
            }

            // Aguardar um pouco para garantir que o arquivo foi completamente escrito
            await Task.Delay(2000, _cancellationToken);

            if (!File.Exists(filePath))
                return;

            // Verificar se arquivo ainda está sendo usado
            if (IsFileLocked(filePath))
            {
                // Tentar novamente após alguns segundos
                await Task.Delay(3000, _cancellationToken);
            }

            if (_cancellationToken.IsCancellationRequested)
                return;

            await ProcessFileInternal(filePath);
        }

        private async Task ProcessFileInternal(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            LoggingService.LogInfo($"Processando arquivo: {fileName}");

            try
            {
                // Enviar arquivo via SFTP
                var success = await _sftpService.UploadFileAsync(filePath);

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

        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}

