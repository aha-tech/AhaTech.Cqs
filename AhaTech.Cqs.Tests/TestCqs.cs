using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using AhaTech.Cqs.ServerTest;
using AhaTech.Cqs.ServerTest.SubNs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace AhaTech.Cqs.Tests
{
    public class TestCqs
    {
        private readonly ITestOutputHelper _output;

        public TestCqs(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public async Task TestCommand()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            var cmd = new TestCommand
            {
                IntProp = 1,
                LongProp = 2,
                StringProp = "A",
                DateTimeProp = DateTimeOffset.Now,
                EnumProp = StringComparison.Ordinal,
                ListProp = new[] { 1, 2, 3 }
            };
            var result = await client.PostAsJsonAsync("/Api/TestCommand", cmd);
            var data = await result.Content.ReadAsStringAsync();
            _output.WriteLine(data);
            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestCommandWithResult()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            var cmd = new TestCommandWithResult
            {
                IntProp = 1,
                LongProp = 2,
                StringProp = "A",
                DateTimeProp = DateTimeOffset.Now,
                EnumProp = StringComparison.Ordinal,
                ListProp = new[] { 1, 2, 3 }
            };
            var result = await client.PostAsJsonAsync("/Api/TestCommandWithResult", cmd);
            var data = await result.Content.ReadAsStringAsync();
            _output.WriteLine(data);
            result.EnsureSuccessStatusCode();
            Assert.Equal("Hello", data);
        }

        [Fact]
        public async Task TestQuery()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            var query = new TestQuery
            {
                IntProp = 1,
                LongProp = 2,
                StringProp = "A",
                DateTimeProp = DateTimeOffset.Now,
                EnumProp = StringComparison.Ordinal,
                ListProp = new[] { 1, 2, 3 }
            };
            var q = "/Api/TestQuery" + ToUrl(query);
            _output.WriteLine(q);
            var result = await client.GetAsync("/Api/TestQuery" + ToUrl(query));
            var data = await result.Content.ReadAsStringAsync();
            _output.WriteLine(data);
            result.EnsureSuccessStatusCode();
            Assert.Equal("World", data);
        }
        
        [Fact]
        public async Task TestNSCommand()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            var cmd = new NSCommand
            {
            };
            var result = await client.PostAsJsonAsync("/Api/SubNs/NSCommand", cmd);
            var data = await result.Content.ReadAsStringAsync();
            _output.WriteLine(data);
            result.EnsureSuccessStatusCode();
        }


        private static string ToUrl(object obj)
        {
            var props = obj.GetType()
                .GetProperties()
                .SelectMany(p => SerializeParam(p.GetValue(obj)).Select(val => p.Name + "=" + WebUtility.UrlEncode(val)));
            return "?" + string.Join("&", props);

            static string[] SerializeParam(object? param)
            {
                if (param == null)
                {
                    return Array.Empty<string>();
                }
                if (param is Enum e)
                {
                    return new[] { e.ToString() };
                }

                return param switch
                {
                    int i => new[] { i.ToString() },
                    bool b => new[] { b.ToString() },
                    long l => new[] { l.ToString() },
                    double d => new[] { d.ToString(CultureInfo.InvariantCulture) },
                    DateTime d => new[] { d.ToString("O") },
                    DateTimeOffset d => new[] { d.ToString("O") },
                    string s => new[] { s },
                    IReadOnlyList<int> l => l.Select(x => x.ToString()).ToArray(),
                    IReadOnlyList<bool> l => l.Select(x => x.ToString()).ToArray(),
                    IReadOnlyList<long> l => l.Select(x => x.ToString()).ToArray(),
                    IReadOnlyList<double> l => l.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray(),
                    IReadOnlyList<DateTime> l => l.Select(x => x.ToString("O")).ToArray(),
                    IReadOnlyList<DateTimeOffset> l => l.Select(x => x.ToString("O")).ToArray(),
                    IReadOnlyList<string> l => l.ToArray(),
                    _ => throw new ArgumentException("Unsupported url type: " + param.GetType())
                };
            }
        }
    }
}