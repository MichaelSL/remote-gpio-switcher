using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPIO.Service.Cmd
{
    public class AppService : IHostedService, IDisposable
    {
        const long FIVE_SEC = 5 * 1000;

        private readonly ILogger _logger;
        private readonly IOptions<SwitchConfig> _config;
        private int _switchOnHour;
        private int _switchOffHour;
        private SwitchPinService _switchService;
        private Timer _timer;
        private long invokationCounter = 0;
        private int _pin;

        public AppService(ILogger<AppService> logger, IOptions<SwitchConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pin = _config.Value?.Pin ?? 0;
            _logger.LogInformation("Starting GPIO switch daemon; PIN: {pin}", _pin);
            _switchOnHour = _config.Value?.SwitchOnHour ?? 9;
            _switchOffHour = _config.Value?.SwitchOffHour ?? 23;
            if (_switchOnHour > _switchOffHour)
            {
                _logger.LogWarning("Wrong switch hours order: swapping");
                var tmp = _switchOffHour;
                _switchOffHour = _switchOnHour;
                _switchOnHour = tmp;
            }
            _logger.LogInformation("Starting GPIO switch daemon; SwitchOn: {_switchOnHour}; SwitchOff: {_switchOffHour}", _switchOnHour, _switchOffHour);

            _switchService = new SwitchPinService(_pin, _logger);

            _timer = new Timer(new TimerCallback(ServiceTick), _pin, FIVE_SEC, FIVE_SEC);

            return Task.CompletedTask;
        }

        private void ServiceTick(object arg)
        {
            invokationCounter++;
            _logger.LogTrace("Service ticked {invokationCounter} times", invokationCounter);

            var now = DateTime.Now;
            double totalHours = now.TimeOfDay.TotalHours;
            _logger.LogDebug("TotalHours: {totalHours}; SwitchOn: {_switchOnHour}; SwitchOff: {_switchOffHour}", totalHours, _switchOnHour, _switchOffHour);
            if (totalHours > _switchOnHour && now.TimeOfDay.TotalHours < _switchOffHour)
                _switchService.SwitchOn();
            else
                _switchService.SwitchOff();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping GPIO switch daemon.");
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _switchService?.Dispose();
            _switchService = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _timer?.Dispose();
            _switchService?.Dispose();
        }
    }
}
