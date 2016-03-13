using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Funq;
using ServiceStack;

namespace ServiceStackRequestContextItemsRepro
{
    public interface IHaveValue { int Value { get; set; } };

    [Route("/test-v1")]
    public class GetValueV1 : IReturn<GetValueResponse>, IHaveValue
    {
        public int Value { get; set; }
    }


    [Route("/test-v2")]
    public class GetValueV2 : IReturn<GetValueResponse>, IHaveValue
    {
        public int Value { get; set; }
    }

    [Route("/test-v3")]
    public class GetValueV3 : IReturn<GetValueResponse>, IHaveValue
    {
        public int Value { get; set; }
    }

    public class GetValueResponse
    {
        public int Value { get; set; }
    }


    public class TestService : Service
    {
        public async Task<GetValueResponse> Get(GetValueV1 _)
        {
            var tmp = (int) RequestContext.Instance.Items["Value"];

            await DoSomethingAsync();

            return new GetValueResponse { Value =  tmp };
        }

        public async Task<GetValueResponse> Get(GetValueV2 _)
        {
            await DoSomethingAsync();

            return new GetValueResponse { Value = (int)RequestContext.Instance.Items["Value"] };
        }

        public async Task<GetValueResponse> Get(GetValueV3 _)
        {
            await DoSomethingAsync();

            return new GetValueResponse { Value = (int)CallContext.LogicalGetData("Value") };
        }

        private async Task DoSomethingAsync()
        {
            await Task.Delay(10);
        }
    }


    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("Test service", typeof(TestService).Assembly) { }

        public override void Configure(Container container)
        {
            GlobalRequestFilters.Add((request, response, dto) =>
            {
                var hasValue = dto as IHaveValue;
                RequestContext.Instance.Items["Value"] = hasValue.Value;
                CallContext.LogicalSetData("Value", hasValue.Value);
            });
        }
    }
}
