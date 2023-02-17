using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AhaTech.Cqs.AspnetCore
{
    [ApiController]
    [Route("")]
    public abstract class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;

        protected CommandController(ILogger<CommandController> logger)
        {
            _logger = logger;
        }

        protected Stopwatch? LogStart<T>(T command)
        {
            Stopwatch? sw = null;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                sw = Stopwatch.StartNew();
                _logger.LogInformation("Received command: {command}", JsonSerializer.Serialize(command));
            }

            return sw;
        }

        protected void LogSuccess(Stopwatch? stopwatch)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Command completed in {time}ms", stopwatch!.ElapsedMilliseconds);
            }
        }

        protected void LogSuccess<T>(Stopwatch? stopwatch, T result)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Command completed in {time}ms. Return value: {result}",
                    stopwatch!.ElapsedMilliseconds, JsonSerializer.Serialize(result));
            }
        }

        protected void LogError(Exception exception)
        {
            _logger.LogError("Command processing failed: {exception}", exception);
        }
    }

    public class CommandController<TCommand> : CommandController where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public CommandController(ICommandHandler<TCommand> handler, ILogger<CommandController> logger) : base(logger)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            var sw = LogStart(command);

            try
            {
                await _handler.Handle(command, cancellationToken);
                LogSuccess(sw);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }
    }

    public class CommandController<TCommand, TResult> : CommandController where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _handler;

        public CommandController(
            ICommandHandler<TCommand, TResult> handler,
            ILogger<CommandController> logger) : base(logger)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var sw = LogStart(command);
            try
            {
                var result = await _handler.Handle(command, cancellationToken);
                LogSuccess(sw, result);
                return result;
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }
    }
}