using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AhaTech.Cqs.ServerTest
{
    public class TestCommand : ICommand
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public long LongProp { get; set; }
        public DateTimeOffset DateTimeProp { get; set; }
        public StringComparison EnumProp { get; set; }
        public IReadOnlyList<int> ListProp { get; set; }
    }
    
    public class TestCommandWithResult : ICommand<string>
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public long LongProp { get; set; }
        public DateTimeOffset DateTimeProp { get; set; }
        public StringComparison EnumProp { get; set; }
        public IReadOnlyList<int> ListProp { get; set; }
    }
    
    public class TestQuery : IQuery<string>
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public long LongProp { get; set; }
        public DateTimeOffset DateTimeProp { get; set; }
        public StringComparison EnumProp { get; set; }
        public IReadOnlyList<int> ListProp { get; set; }
    }

    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task Handle(TestCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class TestCommandWithResultHandler : ICommandHandler<TestCommandWithResult, string>
    {
        public Task<string> Handle(TestCommandWithResult command, CancellationToken cancellationToken)
        {
            return Task.FromResult("Hello");
        }
    }

    public class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> Handle(TestQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult("World");
        }
    }

    namespace SubNs
    {
        public class NSCommand : ICommand
        {
            public string? Optional { get; set; }
        }

        public class NSCommandHandler : ICommandHandler<NSCommand>
        {
            public Task Handle(NSCommand command, CancellationToken cancellationToken)
            {
                throw new ArgumentException("Argex");
            }
        }
    }
}