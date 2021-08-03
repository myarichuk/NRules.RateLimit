using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NRules.RateLimit.Tests;
using System;
using System.Collections.Generic;

namespace NRules.RateLimit.Benchmark
{
    [MemoryDiagnoser]
    public class Program
    {
        private static readonly List<RequestContext> _requests = new();
        private static RateLimitContext _context;

        static Program() => InitRules();

        public static void InitRules()
        {
            for(int i = 0; i < 1000; i++)
                _requests.Add(new RequestContext(Guid.NewGuid(), "/foo"));
            _context = new RateLimitContext(typeof(TooMuchRequests).Assembly);
            foreach (var req in _requests)
                _context.OnRequestStarting(req);
        }

        [Benchmark]
        public void Resolve_limit_rules() => 
            _context.ShouldBlockRequest();

        static void Main(string[] args) => 
            BenchmarkRunner.Run<Program>();
    }
}
