using Application.Services;
using Contracts.Common.Models.Enums;
using Contracts.Events.MatchMakingEvents;
using Contracts.Events.ServerEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Sagas;

public sealed class PlayerQueueSaga : MassTransitStateMachine<PlayerQueueSagaData>
{
    private readonly IUserNotifierService _notifierService;

    public State Queued { get; set; }
    public State MatchFound { get; set; }
    public State ConnectionDataRecieved { get; set; }
    public State GameReady { get; set; }
    public State Complete { get; set; }
    public State Cancel { get; set; }

    public Event<QueuePlayerAddEvent> MatchPlayerQueued { get; set; }
    public Event<MatchPlayerAddEvent> MatchPlayerAdd { get; set; }
    public Event<ServerConnectionDataUpdateEvent> ServerConnectionDataUpdate { get; set; }
    public Event<MatchReadyEvent> MatchReady { get; set; }
    public Event<ServerPlayerConnectedEvent> PlayerConnected { get; set; }
    public Event<QueuePlayerRemoveEvent> PlayerRemovedFromQueue { get; set; }
    public Event<MatchPlayerRemoveEvent> MatchPlayerRemoveEvent { get; set; }
    public Event<MatchCancelEvent> MatchCancel { get; set; }

    public PlayerQueueSaga(ILogger<PlayerQueueSaga> logger, IUserNotifierService notifierService)
    {
        _notifierService = notifierService;

        InstanceState(x => x.CurrentState);

        Event(() => MatchPlayerQueued, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));
        Event(() => MatchPlayerAdd, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));
        Event(() => PlayerConnected, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));
        Event(() => PlayerRemovedFromQueue, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));
        Event(() => MatchPlayerRemoveEvent, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));

        Event(() => MatchCancel, e =>
        {
            e.CorrelateBy((saga, context) => saga.MatchId == context.Message.MatchId);
            e.SelectId(context => Guid.NewGuid());
        });

        Event(() => ServerConnectionDataUpdate, e =>
        {
            e.CorrelateBy((saga, context) => saga.MatchId == context.Message.MatchId);
            e.SelectId(context => Guid.NewGuid());
        });

        Event(() => MatchReady, e =>
        {
            e.CorrelateBy((saga, context) => saga.MatchId == context.Message.MatchId);
            e.SelectId(context => Guid.NewGuid());
        });

        Initially(
            When(MatchPlayerQueued)
            .ThenAsync(async (context) =>
            {
                logger.LogInformation($"Player {context.Message.UserId} queued");
                context.Saga.UserId = context.Message.UserId;
                context.Saga.Ticket = context.Message.TicketId;
                context.Saga.AddedToQueue = true;

                await _notifierService.NotifyUserQueueUpdate(context.Saga.UserId, QueueStatusEnum.InQueue);
            })
            .TransitionTo(Queued));

        During(Queued, When(MatchPlayerAdd)
            .ThenAsync(async (context) =>
            {
                context.Saga.MatchFound = true;

                context.Saga.MatchId = context.Message.MatchId;

                logger.LogInformation($"Match for player {context.Message.UserId} found");

                await _notifierService.NotifyUserQueueUpdate(context.Saga.UserId, QueueStatusEnum.WaitMatch);

            }).TransitionTo(MatchFound));

        During(MatchFound, When(ServerConnectionDataUpdate)
            .ThenAsync(async (context) =>
            {
                logger.LogInformation($"Server ready for match {context.Message.MatchId}");

                context.Saga.ServerForMatchInit = true;
                context.Saga.Address = context.Message.Address;
                context.Saga.Port = context.Message.Port;

#if DEBUG
                await _notifierService.NotifyUserQueueUpdate(context.Saga.UserId, QueueStatusEnum.Connecting); //Fake packet to debug
#endif

            }).TransitionTo(ConnectionDataRecieved)
        );

        During(ConnectionDataRecieved, When(MatchReady)
            .ThenAsync(async (context) =>
            {

                logger.LogInformation($"Wait connect player ${context.Saga.UserId}");

                context.Saga.GameReady = true;
                await _notifierService.NotifyUserMatchReady(context.Saga.UserId, context.Saga.Address, context.Saga.Port, context.Saga.Ticket);

            }).TransitionTo(GameReady)
        );

        During(GameReady, When(PlayerConnected).Then(context =>
        {
            logger.LogInformation($"Player ${context.Saga.UserId} connected to match. Exit saga");
        }).TransitionTo(Complete));

        SetCompleted(async instance =>
        {
            State<PlayerQueueSagaData> currentState = await this.GetState(instance);

            return Complete.Equals(currentState);
        });


        DuringAny(When(PlayerRemovedFromQueue).ThenAsync(async context =>
        {
            logger.LogInformation($"Player ${context.Saga.UserId} removed from queue. Exit saga");
            await _notifierService.NotifyUserQueueRemove(context.Saga.UserId, context.Message.Reason);
        }).TransitionTo(Complete));

        DuringAny(When(MatchCancel).ThenAsync(async context =>
        {

            logger.LogInformation($"Match ${context.Saga.MatchId} cancel. Exit saga");
            await _notifierService.NotifyUserMatchCancel(context.Saga.UserId, context.Message.Reason);
        }).TransitionTo(Complete));

        DuringAny(When(MatchPlayerRemoveEvent).ThenAsync(async context =>
        {
            logger.LogInformation($"Player ${context.Saga.UserId} removed from match. Exit saga");
        }).TransitionTo(Complete));
    }
}
