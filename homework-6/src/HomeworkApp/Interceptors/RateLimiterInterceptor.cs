using Grpc.Core;
using Grpc.Core.Interceptors;
using HomeworkApp.Bll.Services.Interfaces;

namespace HomeworkApp.Interceptors;

public class RateLimiterInterceptor : Interceptor
{
    private const string keyUserIpHeader = "X-R256-USER-IP";
    private readonly IRateLimiterService _rateLimiterService;

    public RateLimiterInterceptor(IRateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var clientIp = context.RequestHeaders.GetValue(keyUserIpHeader);

        if (string.IsNullOrEmpty(clientIp))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Ip was not found"));
        }

        if (!await _rateLimiterService.Allow(clientIp, context.CancellationToken))
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted, "Request limit exceeded"));
        }

        return await continuation(request, context);
    }
}