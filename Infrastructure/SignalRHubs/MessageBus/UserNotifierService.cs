using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Infrastructure.SignalRHubs;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Application.Services;

public interface IUserNotifierService
{
    Task NotifyUserQueueUpdate(string userId, QueueStatusEnum statusEnum);
    Task NotifyUserMatchReady(string userId, string address, int port, string ticket);
    Task NotifyUserMatchCancel(string userId, MatchCancelEnum reason);
    Task NotifyUserQueueRemove(string userId, QueuePlayerRemoveEnum reason);
}

public class UserNotifierService : IUserNotifierService
{
    private readonly IHubContext<UserHub, IUserHubClient> _hubContext;
    private readonly IDatabase _signalrStore;

    private const string KEY = "SignalRConnection:";
    public UserNotifierService(IHubContext<UserHub, IUserHubClient> hubContext, IConnectionMultiplexer redis)
    {
        _hubContext = hubContext;
        _signalrStore = redis.GetDatabase();
    }

    public async Task NotifyUserQueueUpdate(string userId, QueueStatusEnum statusEnum)
    {
        var connectionId = (await _signalrStore.StringGetAsync($"{KEY}{userId}"));

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).MatchmakingStatusUpdate(statusEnum);
    }

    public async Task NotifyUserMatchReady(string userId, string address, int port, string ticket)
    {
        var connectionId = (await _signalrStore.StringGetAsync($"{KEY}{userId}"));

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).MatchReady(address, port, ticket);
    }

    public async Task NotifyUserMatchCancel(string userId, MatchCancelEnum reason)
    {
        var connectionId = (await _signalrStore.StringGetAsync($"{KEY}{userId}"));

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).MatchCancel(reason);
    }

    public async Task NotifyUserQueueRemove(string userId, QueuePlayerRemoveEnum reason)
    {
        var connectionId = (await _signalrStore.StringGetAsync($"{KEY}{userId}"));

        if (connectionId == RedisValue.Null)
            return;

        await _hubContext.Clients.Client(connectionId.ToString()).QueueRemove(reason);
    }
}
