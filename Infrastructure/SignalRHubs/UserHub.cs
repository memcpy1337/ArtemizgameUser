using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SignalRHubs;

public class UserHub : Hub
{
    private readonly IConnectionMultiplexer _redis;

    public UserHub(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var connectionId = Context.ConnectionId;

        var db = _redis.GetDatabase();
        await db.StringSetAsync($"SignalRConnection:{userId}", connectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync($"SignalRConnection:{userId}");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        var db = _redis.GetDatabase();
        var connectionId = await db.StringGetAsync($"SignalRConnection:{userId}");

        if (!string.IsNullOrEmpty(connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }
}