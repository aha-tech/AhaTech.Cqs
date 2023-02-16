using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AhaTech.Cqs.AspnetCore
{
    [ApiController]
    [Route("")]
    public class QueryController<TQuery, TResult> : ControllerBase where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<QueryController<TQuery, TResult>> _logger;

        public QueryController(IQueryHandler<TQuery, TResult> handler, ILogger<QueryController<TQuery, TResult>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpGet]
        public Task<TResult> Handle([FromQuery]TQuery command, CancellationToken cancellationToken)
        {
            return _handler.Handle(command, cancellationToken);
        }
    }
}