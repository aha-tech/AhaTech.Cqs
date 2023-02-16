using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AhaTech.Cqs.AspnetCore
{
    [ApiController]
    [Route("")]
    public class CommandController<TCommand> : ControllerBase where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly ILogger<CommandController<TCommand>> _logger;

        public CommandController(ICommandHandler<TCommand> handler, ILogger<CommandController<TCommand>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpPost]
        public Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            return _handler.Handle(command, cancellationToken);
        }
    }
    [ApiController]
    [Route("")]
    public class CommandController<TCommand, TResult> : ControllerBase where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _handler;
        private readonly ILogger<CommandController<TCommand, TResult>> _logger;

        public CommandController(ICommandHandler<TCommand, TResult> handler, ILogger<CommandController<TCommand, TResult>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpPost]
        public Task<TResult> Handle(TCommand command, CancellationToken cancellationToken)
        {
            return _handler.Handle(command, cancellationToken);
        }
    }
}