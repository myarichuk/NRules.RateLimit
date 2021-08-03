using NRules.Extensibility;
using NRules.Fluent;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NRules.RateLimit
{
    public class RateLimitContext : IDependencyResolver
    {
        private readonly ISession _rules;
        private readonly object _sessionLock = new object();

        private static readonly string TypeName = typeof(RateLimitContext).FullName;

        private bool _shouldBlockRequest;

        public RateLimitContext(params Assembly[] ruleAssemblies)
        {
            var repository = new RuleRepository();
            repository.Load(x => x.From(ruleAssemblies));

            var sessionFactory = repository.Compile();
            sessionFactory.DependencyResolver = this;

            _rules = sessionFactory.CreateSession();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnBlockRequests() => _shouldBlockRequest = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUnblockRequests() => _shouldBlockRequest = false;

        public bool ShouldBlockRequest()
        {
            lock (_sessionLock)
                _rules.Fire();
            
            return _shouldBlockRequest;
        }

        public void OnRequestStarting(in RequestContext requestContext)
        {
            lock(_sessionLock)
                _rules.TryInsert(requestContext);
        }

        public void OnRequestEnding(in RequestContext requestContext)
        {
            lock (_sessionLock)
                _rules.TryRetract(requestContext);
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            if (serviceType.FullName != TypeName)
                throw new ArgumentException("Not supported resolution type");

            return this;
        }
    }
}
