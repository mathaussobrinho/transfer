using System;
using System.Threading;
using System.Threading.Tasks;
using TransferFilesApp.Models;

namespace TransferFilesApp.Services
{
    public class ScheduledService
    {
        private readonly SftpConfig _config;
        private Timer? _timer;
        private DateTime _lastRunDate = DateTime.MinValue;

        public ScheduledService(SftpConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            // Verificar se já passou das 17h hoje e ainda não executou
            var now = DateTime.Now;
            var today1700 = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
            
            if (now >= today1700 && _lastRunDate != now.Date)
            {
                // Executar imediatamente se já passou das 17h e não executou hoje
                LoggingService.LogInfo("Já passou das 17h hoje. Executando envio agora...");
                Task.Run(async () => await ExecuteScheduledTask());
            }

            // Calcular próximo horário (17:00 hoje ou amanhã)
            var nextRun = GetNextRunTime();
            var delay = nextRun - DateTime.Now;

            if (delay.TotalMilliseconds < 0)
                delay = TimeSpan.Zero;

            LoggingService.LogInfo($"Próximo envio agendado para: {nextRun:dd/MM/yyyy HH:mm:ss}");

            // Timer a cada 24 horas
            _timer = new Timer(async _ => await ExecuteScheduledTask(), null, delay, TimeSpan.FromHours(24));
        }

        public void Stop()
        {
            _timer?.Dispose();
            LoggingService.LogInfo("Serviço agendado parado");
        }

        private DateTime GetNextRunTime()
        {
            var now = DateTime.Now;
            var today1700 = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);

            // Se já passou das 17h hoje, agendar para amanhã
            if (now >= today1700)
            {
                return today1700.AddDays(1);
            }

            return today1700;
        }

        private async Task ExecuteScheduledTask()
        {
            var now = DateTime.Now;
            var today = now.Date;

            // Se passou das 17h e ainda não executou hoje, executar agora
            // (caso o computador tenha sido ligado após as 17h)
            var horaAtual = now.Hour;
            var jaPassou1700 = horaAtual >= 17;

            // Evitar execução duplicada no mesmo dia
            if (_lastRunDate == today)
            {
                LoggingService.LogInfo("Envio já foi executado hoje. Próximo envio será amanhã às 17:00");
                return;
            }

            // Se ainda não são 17h, aguardar
            if (!jaPassou1700)
            {
                LoggingService.LogInfo($"Aguardando horário agendado (17:00). Hora atual: {now:HH:mm:ss}");
                return;
            }

            _lastRunDate = today;

            LoggingService.LogInfo("=== INICIANDO ENVIO AGENDADO (17:00) ===");

            try
            {
                var sftpService = new SftpService(_config);
                await FileProcessorService.ProcessAllFiles(sftpService);
                sftpService.Dispose();
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Erro no envio agendado: {ex.Message}");
            }

            LoggingService.LogInfo("=== ENVIO AGENDADO CONCLUÍDO ===");
        }
    }
}

