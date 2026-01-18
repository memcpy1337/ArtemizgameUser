using Application.Common.Interfaces;
using Contracts.Events.MatchMakingEvents;
using Contracts.Events.UserEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class UserPublisher : IUserPublisher
{
    private readonly IBus _publishEndpoint;
    public UserPublisher(IBus bus)
    {
        _publishEndpoint = bus;
    }

    public async Task PlayerDisconnected(string userId)
    {
        await _publishEndpoint.Publish(new UserExitQueuePlayEvent() { UserId = userId });
    }
}
