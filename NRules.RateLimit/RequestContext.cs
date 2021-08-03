using Microsoft.AspNetCore.Http;
using System;

namespace NRules.RateLimit
{
#if NETSTANDARD2_0
    public readonly struct RequestContext : IEquatable<RequestContext>
#else
    public record RequestContext 
#endif
    {
        public Guid Id { get; }

        public PathString QueryPath { get; }

        public RequestContext(Guid id, PathString queryPath)
        {
            Id = id;
            QueryPath = queryPath;
        }

#if NETSTANDARD2_0
        #region IEquatable Implementation

        public override bool Equals(object obj) =>
            obj is RequestContext context && Equals(context);

        public bool Equals(RequestContext other) =>
            Id.Equals(other.Id) &&
                   QueryPath.Equals(other.QueryPath);

        public override int GetHashCode()
        {
            int hashCode = 1763280214;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + QueryPath.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(RequestContext left, RequestContext right) => left.Equals(right);
        public static bool operator !=(RequestContext left, RequestContext right) => !(left == right);

        #endregion
#endif
    }
}
