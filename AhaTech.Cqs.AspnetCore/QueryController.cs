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
    public class QueryController<TQuery, TResult> : ControllerBase where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger _logger;

        public QueryController(IQueryHandler<TQuery, TResult> handler, ILoggerFactory loggerFactory)
        {
            _handler = handler;
            _logger = loggerFactory.CreateLogger(nameof(AhaTech.Cqs.AspnetCore)+"QueryController");
        }

        [HttpGet]
        public async Task<TResult> Handle([FromQuery]TQuery query, CancellationToken cancellationToken)
        {
            var sw = LogStart(query);
            try
            {

                var result = await _handler.Handle(query, cancellationToken);
                LogSuccess(sw, result);
                return result;
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        private Stopwatch? LogStart<T>(T query)
        {
            
            Stopwatch? sw = null;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                sw = Stopwatch.StartNew();
                _logger.LogInformation("Received query: {command}", JsonSerializer.Serialize(query));
            }

            return sw;
        }

        private void LogSuccess<T>(Stopwatch? stopwatch, T result)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Query completed in {time}ms. Return value: {result}",
                    stopwatch!.ElapsedMilliseconds, JsonSerializer.Serialize(result));
            }
        }

        private void LogError(Exception exception)
        {
            _logger.LogError("Command processing failed: {exception}", exception);
        }
    }
}