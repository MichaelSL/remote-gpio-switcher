using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace GPIO.Service.Cmd
{
    public class SwitchPinService: IDisposable
    {
        private readonly int _pin;
        private readonly ILogger _logger;
        private readonly GpioController _controller;

        public SwitchPinService(int pin, ILogger logger)
        {
            _pin = pin;
            _logger = logger;
            _controller = new GpioController();
        }


        public void SwitchOn()
        {
            if (!_controller.IsPinOpen(_pin))
            {
                _controller.OpenPin(_pin, PinMode.Output);
                _logger?.LogDebug("GPIO pin opened: {pin}", _pin);
            }

            _controller.Write(_pin, PinValue.High);
            _logger?.LogDebug("PIN off: {pin}", _pin);
        }

        public void SwitchOff()
        {
            if (!_controller.IsPinOpen(_pin))
            {
                _controller.OpenPin(_pin, PinMode.Output);
                _logger?.LogDebug("GPIO pin opened: {pin}", _pin);
            }

            _controller.Write(_pin, PinValue.Low);
            _logger?.LogDebug("PIN off: {pin}", _pin);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _controller.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SwitchPinService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
