using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRules.RateLimit.Tests
{
    public class Basics
    {
        [Fact]
        public void Can_limit_total_requests()
        {
            var ctx = new RateLimitContext(typeof(Basics).Assembly);
            var requests = new List<RequestContext>();
            for(int i = 0; i < 5; i++) //each request happens on DIFFERENT endpoint
                requests.Add(new RequestContext(Guid.NewGuid(), "/foobar_" + i));

            foreach(var request in requests)
                ctx.OnRequestStarting(request);

            Assert.True(ctx.ShouldBlockRequest());

            ctx.OnRequestEnding(requests.First());

            Assert.False(ctx.ShouldBlockRequest());
        }

        [Fact]
        public void Can_limit_requests_per_endpoint()
        {
            var ctx = new RateLimitContext(typeof(Basics).Assembly);
            
            //sanity check
            Assert.False(ctx.ShouldBlockRequest());

            var request1 = new RequestContext(Guid.NewGuid(), "/foobar");
            var request2 = new RequestContext(Guid.NewGuid(), "/foobar");
            var request3 = new RequestContext(Guid.NewGuid(), "/foobar");
            var request4 = new RequestContext(Guid.NewGuid(), "/barfoo");
            var request5 = new RequestContext(Guid.NewGuid(), "/foobar");

            ctx.OnRequestStarting(request1);
            ctx.OnRequestStarting(request2);
            ctx.OnRequestStarting(request4);

            //sanity check 2
            Assert.False(ctx.ShouldBlockRequest());

            ctx.OnRequestStarting(request3);
            Assert.True(ctx.ShouldBlockRequest());

            ctx.OnRequestEnding(request2);
            Assert.False(ctx.ShouldBlockRequest());

            ctx.OnRequestEnding(request4);
            Assert.False(ctx.ShouldBlockRequest());

            ctx.OnRequestStarting(request5);
            Assert.True(ctx.ShouldBlockRequest());
        }
    }
}
