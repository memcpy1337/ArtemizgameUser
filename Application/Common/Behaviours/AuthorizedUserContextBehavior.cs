using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours;

public class AuthorizedUserContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizedUserContextBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var userId = httpContext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var userName = httpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

            if (request is IAuthorizedUser userRequest)
            {
                userRequest.RequestedUserId = userId;
                userRequest.RequestedUserName = userName;
            }
        }

        return await next();
    }
}
