using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using FluentAssertions;
using ServiceStack;
using Xunit;

namespace ServiceStackRequestContextItemsRepro
{
    public class ReproTests : IDisposable
    {
        public const int ServicePort = 28989;

        private ServiceStackHost _appHost;

        public ReproTests()
        {
            _appHost = new AppHost()
                .Init()
                .Start($"http://*:{ServicePort}/");
        }

        [Fact]
        public async Task Each_Request_Has_Own_Items_V1()
        {
            await RunTest(i =>
                new JsonHttpClient($"http://localhost:{ServicePort}").GetAsync(new GetValueV1 { Value = i }));
        }

        [Fact]
        public async Task Each_Request_Has_Own_Items_V2()
        {
            await RunTest(i => 
                new JsonHttpClient($"http://localhost:{ServicePort}").GetAsync(new GetValueV2 {Value = i}));
        }

        [Fact]
        public async Task Each_Request_Has_Own_Items_V3()
        {
            await RunTest(i => 
                new JsonHttpClient($"http://localhost:{ServicePort}").GetAsync(new GetValueV3 {Value = i}));
        }

        private static async Task RunTest(Func<int, Task<GetValueResponse>> startCall)
        {
            var tasks = Enumerable.Range(1, 100)
                .Select(startCall)
                .ToList();

            await Task.WhenAll(tasks);

            var resultValues = tasks.Select(t => t.Result.Value).ToList();

            var expectedValues = Enumerable.Range(1, 100).ToList();

            resultValues.ShouldAllBeEquivalentTo(expectedValues);
        }

        public void Dispose()
        {
            _appHost?.Dispose();
        }
    }
}
