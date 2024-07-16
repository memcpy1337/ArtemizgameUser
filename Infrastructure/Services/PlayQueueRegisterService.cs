using Application.Common.DTOs.Match;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Events.UserEvents;
using Mapster;
using MassTransit.Clients;
using Contracts.Events.MatchMakingEvents;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Services;

internal sealed class PlayQueueRegisterService : IPlayQueueRegisterService
{
    private readonly IRequestClient<UserEnterQueuePlayEventRequest> _client;
    private readonly ILogger<PlayQueueRegisterService> _logger;

    public PlayQueueRegisterService(IRequestClient<UserEnterQueuePlayEventRequest> client, ILogger<PlayQueueRegisterService> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    /// <summary>
    /// return Ticket for access to server
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> AddUserToQueue(PlayQueueRegisterDto request, CancellationToken cancellationToken)
    {
        var userRegEventData = request.Adapt<UserEnterQueuePlayEventRequest>();

        try
        {
            var data = await _client.GetResponse<UserEnterQueuePlayEventResponse>(userRegEventData, cancellationToken);

            return data.Message.Token;
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError($"Request timeout: {ex.Message}");
            throw new Exception("Request timed out. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw new Exception("An error occurred while processing your request.");
        }

    }
}
