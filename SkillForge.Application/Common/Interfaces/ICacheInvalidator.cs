using System.Collections.Generic;

namespace SkillForge.Application.Common.Interfaces
{
    public interface ICacheInvalidator
    {
        IEnumerable<string> CacheKeysToInvalidate { get; }
    }
} 