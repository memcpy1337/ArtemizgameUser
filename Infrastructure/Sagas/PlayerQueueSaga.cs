using Contracts.Events.MatchMakingEvents;
using Contracts.Events.ServerEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Sagas;

public sealed class PlayerQueueSaga : MassTransitStateMachine<PlayerQueueSagaData>
{

    public State Queued { get; set; }
    public State MatchFound { get; set; }
    public State GameReady { get; set; }

    public Event<QueuePlayerAddEvent> MatchPlayerQueued { get; set; }
    public Event<MatchPlayerAddEvent> MatchPlayerAdd { get; set; }
    public Event<ServerUpdateEvent> ServerUpdate { get; set; }


    public PlayerQueueSaga(ILogger<PlayerQueueSaga> logger)
    {

        InstanceState(x => x.CurrentState);

        Event(() => MatchPlayerQueued, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));
        Event(() => MatchPlayerAdd, e => e.CorrelateById(m => Guid.Parse(m.Message.UserId)));

        Event(() => ServerUpdate, e =>
        {
            e.CorrelateBy((saga, context) => saga.MatchId == context.Message.MatchId);
            e.SelectId(context => Guid.NewGuid());
        });

        Initially(
            When(MatchPlayerQueued)
            .Then((context) => {
                logger.LogInformation($"Player {context.Message.UserId} queued");
                context.Saga.UserId = context.Message.UserId;
                context.Saga.Ticket = context.Message.TicketId;
                context.Saga.AddedToQueue = true;
            })
            .TransitionTo(Queued));

        During(Queued, When(MatchPlayerAdd)
            .Then(context =>
            {
                logger.LogInformation($"Match for player {context.Message.UserId} found");

                context.Saga.MatchFound = true;

                context.Saga.MatchId = context.Message.MatchId;
                context.Saga.AddedToQueue = true;

                if (context.Message.ServerWasReady)
                {
                    context.Saga.ServerForMatchInit = true;
                    context.TransitionToState(GameReady);
                }
                else
                {
                    context.TransitionToState(MatchFound);
                }
            }));

        During(MatchFound,
            When(ServerUpdate, context => context.Message.IsReady)
                .Then(context =>
                {
                    context.Saga.ServerForMatchInit = true;
                }));

        
    }
}
