using System;

namespace SkillForge.Application.Common.Interfaces
{
    public interface ICacheableQuery<TResponse>
    {
        bool BypassCache { get; }
        string CacheKey { get; }
        TimeSpan? CacheExpiration { get; }
    }
} 