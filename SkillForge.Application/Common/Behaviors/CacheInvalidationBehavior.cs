using MediatR;
using Microsoft.Extensions.Logging;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Common.Behaviors
{
    public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheInvalidator
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

        public CacheInvalidationBehavior(ICacheService cacheService, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            foreach (var cacheKey in request.CacheKeysToInvalidate)
            {
                _logger.LogDebug("Invalidating cache key: {CacheKey}", cacheKey);
                await _cacheService.RemoveAsync(cacheKey);
            }

            return response;
        }
    }
} 