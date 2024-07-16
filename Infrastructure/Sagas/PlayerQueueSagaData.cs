using MassTransit;
using System;
namespace Infrastructure.Sagas;

public class PlayerQueueSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public required string CurrentState { get; set; }

    public required string UserId { get; set; }
    public bool AddedToQueue { get; set; }
    public bool MatchFound { get; set; }
    public bool ServerForMatchInit { get; set;  }
    public bool GameReady { get; set; }
    public string? MatchId { get; set; }
    public string? Ticket { get; set; }
}
