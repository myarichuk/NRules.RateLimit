using Microsoft.AspNetCore.Http;
using NRules.Fluent.Dsl;
using System.Linq;

namespace NRules.RateLimit.Tests
{
    public class TooMuchRequestsPerEndpoint : Rule
    {
        public override void Define()
        {
            RateLimitContext context = default;
            IGrouping<PathString, RequestContext> contexts = default;
            int count = 0;

            Dependency()
              .Resolve(() => context);

            When()
              .Query(() => contexts,
                  q => q.Match<RequestContext>(rq => true) //some additional filtering?
                        .GroupBy(x => x.QueryPath))
              .Let(() => count, () => contexts.Count())
              .Having(() => count >= 3);

            Then().Do(ctx => context.OnBlockRequests());
        }
    }
}
