using MediatR;
using Microsoft.Extensions.Logging;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Caching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery<TResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request.BypassCache)
            {
                _logger.LogDebug("Cache bypassed for {RequestName}", typeof(TRequest).Name);
                return await next();
            }

            var cacheKey = request.CacheKey;
            
            _logger.LogDebug("Checking cache for {CacheKey}", cacheKey);
            
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
                var response = await next();
                
                _logger.LogDebug("Caching result for {CacheKey}", cacheKey);
                return response;
            }, request.CacheExpiration);
        }
    }
} 