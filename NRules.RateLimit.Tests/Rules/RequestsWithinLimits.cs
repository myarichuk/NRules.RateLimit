using Microsoft.AspNetCore.Http;
using NRules.Fluent.Dsl;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RateLimit.Tests.Rules
{
    public class RequestsWithinLimits : Rule
    {
        public override void Define()
        {
            RateLimitContext context = default;
            IGrouping<PathString, RequestContext> groupedRequests = default;
            IEnumerable<RequestContext> requests = default;
            int count = 0;
            int groupedCount = 0;
            Dependency()
              .Resolve(() => context);

            When()
                  .Query(() => groupedRequests,
                      q => q.Match<RequestContext>(rq => true) //some additional filtering?
                            .GroupBy(x => x.QueryPath))
                  .Let(() => groupedCount, () => groupedRequests.Count())
                  .Having(() => groupedCount < 3)
              .And(q => q.Query(() => requests,
                   q => q.Match<RequestContext>(rq => true)
                         .Collect())
                    .Let(() => count, () => requests.Count())
                    .Having(() => count < 5));

            Then().Do(ctx => context.OnUnblockRequests());
        }
    }
}
