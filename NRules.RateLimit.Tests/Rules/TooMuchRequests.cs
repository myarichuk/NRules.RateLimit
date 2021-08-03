using NRules.Fluent.Dsl;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RateLimit.Tests
{
    public class TooMuchRequests : Rule
    {
        public override void Define()
        {
            RateLimitContext context = default;
            IEnumerable<RequestContext> contexts = default;
            int count = 0;

            Dependency()
              .Resolve(() => context);

            When()
              .Query(() => contexts,
                  q => q.Match<RequestContext>(rq => true)
                        .Collect())
              .Let(() => count, () => contexts.Count())
              .Having(() => count >= 5);

            Then().Do(ctx => context.OnBlockRequests());
        }
    }
}
