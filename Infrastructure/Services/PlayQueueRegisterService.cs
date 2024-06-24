using Application.Common.DTOs.Match;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Events.UserEvents;
using Mapster;

namespace Infrastructure.Services;

internal sealed class PlayQueueRegisterService : IPlayQueueRegisterService
{
    private readonly IPublishEndpoint _publishEndpoint;
    public PlayQueueRegisterService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task AddUserToQueue(PlayQueueRegisterDto request, CancellationToken cancellationToken)
    {
        var userRegEventData = request.Adapt<UserEnterQueuePlayEvent>();


        await _publishEndpoint.Publish(userRegEventData, cancellationToken);
    }
}
